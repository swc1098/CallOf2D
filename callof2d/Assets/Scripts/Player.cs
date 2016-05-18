﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Slider healthSlide;

    public float moveSpeed;
    public GameObject reticle;
    public string ID;
    public string SocketID;

    // Health, keeps track of player life. (5 default) 
    private int maxHealth = 7;
    public int health;

    bool moveUp;
    bool moveDown;
    bool moveLeft;
    bool moveRight;
    bool shoot;

    private Vector2 lastPos;
    private Rigidbody2D body;
    private BoxCollider2D col;
    private JSONObject j;
    private GameManager GM;

    public GameObject healthImage;
    public Image newImage;

    public GameObject bullet;

	
	private Team teamState;
	private SpriteRenderer rend;
	private bool teamAssigned = false;

    // Use this for initialization
    void Start()
    {

        moveSpeed = 80f;

        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0;
        body.drag = 10;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.freezeRotation = true;
        col = GetComponent<BoxCollider2D>();
        col.size = new Vector2(0.14f, 0.14f);

        health = maxHealth;

        // create a new image and spawn it inside GUICanvas
        // set it up so it tracks health.
        healthImage = new GameObject();
        newImage = healthImage.AddComponent<Image>();
        newImage.sprite = Resources.Load("Health", typeof(Sprite)) as Sprite;
        newImage.material = Resources.Load("Green", typeof(Material)) as Material;
        newImage.name = "HealthBar";
        newImage.rectTransform.sizeDelta = new Vector2(2.0f, 0.5f);
        newImage.type = Image.Type.Filled;
        newImage.fillMethod = Image.FillMethod.Horizontal;
        healthImage.transform.parent = GameObject.Find("GUICanvas").transform;

        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void AssignID(string objID)
    {
        ID = objID;
        gameObject.StoreID(ID);
    }

    // Update is called once per frame
    void Update()
    {
        // constantly update position
        healthImage.transform.position = gameObject.transform.position + new Vector3(0, 1.0f, 0);

        // Ensure lockstep is ready before issuing commands
        if (GM.lockstep.CommandReady && GM.gameState == GameState.Game && SocketID == GM.SocketID)
        {
            // Reset JSON
            j = new JSONObject();

            // Handle Input
            HandleInput();

            // Handle respawn
            if (health <= 0)
            {
                GM.ChangeState(GameState.Lose);
                Vector2 respawnPos = GM.FindRandomSpawnLocation();
                j.AddField("respawn", true);
                j.AddField("setX", respawnPos.x);
                j.AddField("setY", respawnPos.y);

                JSONObject stats = new JSONObject();
                stats.AddField("deaths", 1);
                GM.lockstep.GetSocket().Emit("deathCount", stats);
            }

            // Create and send basic JSON
            j.AddField("gameobject", ID);
            j.AddField("player", SocketID);
            j.AddField("playerobj", true);
            j.AddField("health", health);
            j.AddField("posX", transform.position.x);
            j.AddField("posY", transform.position.y);
            GM.lockstep.issuedCommands.Enqueue(j);

        }

		DetermineTeam ();

        // Handle health bar
        HandleHealth();

        // Smoothly move towards correct position
        body.AddForce((lastPos - (Vector2)transform.position), ForceMode2D.Force);

    }

    void FixedUpdate()
    {
        //
    }

    public void ExecuteCommand(JSONObject Command)
    {
        lastPos = new Vector2((float)Command.GetField("posX").n, (float)Command.GetField("posY").n);

        if (Command.HasField("respawn"))
        {
            transform.position = new Vector2((float)Command.GetField("setX").n, (float)Command.GetField("setY").n);
            lastPos = transform.position;
            health = maxHealth;
            newImage.material = Resources.Load("Green", typeof(Material)) as Material;
        }

        if (Command.HasField("takedamage"))
        {
            TakeDamage();
            return;
        }

        if (Command.HasField("move"))
        {
            float x = 0;
            float y = 0;

            // Parse Movement
            if (Command.HasField("moveX"))
            {
                x = (float)Command.GetField("moveX").n;
            }
            if (Command.HasField("moveY"))
            {
                y = (float)Command.GetField("moveY").n;
            }

            body.AddForce(new Vector2(x, y), ForceMode2D.Force);
            body.velocity = Vector2.ClampMagnitude(body.velocity, moveSpeed);
        }
        else
        {
            // Immediately decelerate
            body.velocity = Vector2.zero;
        }

        if (Command.HasField("spawnbullet") && !Extensions.idToObject.ContainsKey(Command.GetField("spawnbullet").str))
        {
            GameObject bullet = (GameObject)Instantiate(Resources.Load("Bullet"), transform.position, Quaternion.identity);
            bullet.GetComponent<Bullet>().AssignID(Command.GetField("spawnbullet").str);
            bullet.GetComponent<Bullet>().SocketID = SocketID;

            bullet.GetComponent<Bullet>().player = gameObject;
            bullet.GetComponent<Bullet>().direction = new Vector2((float)Command.GetField("dirX").n, (float)Command.GetField("dirY").n);
        }

    }

    void HandleInput()
    {
        // Input
        moveUp = Input.GetKey(KeyCode.W);
        moveDown = Input.GetKey(KeyCode.S);
        moveLeft = Input.GetKey(KeyCode.A);
        moveRight = Input.GetKey(KeyCode.D);
        shoot = Input.GetMouseButtonDown(0);    // Left Mouse Button

        // Movement
        if (moveUp || moveDown || moveLeft || moveRight)
        {
            j.AddField("move", true);

            if (moveUp)
            {
                j.AddField("moveY", moveSpeed);
            }
            else if (moveDown)
            {
                j.AddField("moveY", -moveSpeed);
            }
            if (moveLeft)
            {
                j.AddField("moveX", -moveSpeed);
            }
            else if (moveRight)
            {
                j.AddField("moveX", moveSpeed);
            }
        }

        // Shooting
        if (shoot)
        {
            j.AddField("spawnbullet", Extensions.GenerateID());

            Vector2 direction = (reticle.transform.position - transform.position).normalized;
            j.AddField("dirX", direction.x);
            j.AddField("dirY", direction.y);
        }
    }

    void HandleHealth()
    {
        // check for player death and destroy appropriate objects
        newImage.fillAmount = (float)health / maxHealth;

        /*
        if (health <= 0)
        {
            //Destroy(gameObject);
            //Destroy(healthImage);
            health = maxHealth;
            newImage.material = Resources.Load("Green", typeof(Material)) as Material;
        }
        */

        // check if health is less than and change color accordingly
        if (health <= 2)
        {
            newImage.material = Resources.Load("Red", typeof(Material)) as Material;
        }
        else if (health <= 5)
        {
            newImage.material = Resources.Load("Yellow", typeof(Material)) as Material;
        }
    }

    public void TakeDamage()
    {
        // reduce health and have the fill amount show it.
        health--;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Bullet")
        {
            if (col.gameObject.GetComponent<Bullet>().player != gameObject)
            {
                j.AddField("takedamage", true);
            }
        }
    }
	
	public void DetermineTeam()
	{
		// set team\
		if (teamAssigned == false) 
		{
			teamState = GetRandomType();
			
			rend = GameObject.FindWithTag("Player").GetComponent<SpriteRenderer> ();
			
			if (teamState == Team.Red) 
			{
				rend.color = Color.red;
			}
			if(teamState == Team.Blue)
			{
				rend.color = Color.blue;
			}
			Debug.Log(teamState);
			teamAssigned = true;
		}
		
	}

	/// <summary>
	///  // Gets a random enum by using the values existing within the enum
	/// </summary>
	/// <returns></returns>
	public static Team GetRandomType ()
	{
		// create an array that holds the values of each ElementType
		System.Array a = System.Enum.GetValues (typeof(Team));
		// cycle through the array and then return a random enum type
		return (Team)a.GetValue (Random.Range (0, a.Length));
	}


}
