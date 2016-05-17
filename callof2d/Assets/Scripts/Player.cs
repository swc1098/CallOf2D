using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Slider healthSlide;

    public float moveSpeed;
    public GameObject reticle;
    public int ID;

    // Health, keeps track of player life. (5 default) 
    private int maxHealth = 5;
    public int health = 5;

    bool moveUp;
    bool moveDown;
    bool moveLeft;
    bool moveRight;
    bool shoot;

    private Rigidbody2D body;
    private BoxCollider2D col;
    private JSONObject j;
    private GameManager GM;

    public GameObject healthImage;
    private Image newImage;

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

        // create a new image and spawn it inside GUICanvas
        healthImage = new GameObject();
        newImage = healthImage.AddComponent<Image>();
        newImage.sprite = Resources.Load("Health", typeof(Sprite)) as Sprite;
        newImage.material = Resources.Load("Green", typeof(Material)) as Material;
        newImage.name = "HealthBar";
        newImage.rectTransform.sizeDelta = new Vector2(3.0f, 0.5f);
        healthImage.transform.parent = GameObject.Find("GUICanvas").transform;


        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
        ID = Extensions.GenerateID();
        gameObject.StoreID(ID);

        maxHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        // constantly update position
        healthImage.transform.position = gameObject.transform.position + new Vector3(0, 1.0f, 0);

        // Ensure lockstep is ready before issuing commands
        if (GM.lockstep.LockstepReady && GM.gameState == GameState.Game)
        {
            // Reset JSON
            j = new JSONObject();

            // Handle Input
            HandleInput();

            // issue the command above
            // Only issue commands if there are commands to issue
            if (j.Count > 0)
            {
                j.AddField("gameobject", ID);
                GM.lockstep.issuedCommands.Enqueue(j);
            }
            else
            {
                // Immediately decelerate
                body.velocity = Vector2.zero;
            }
        }

        // check for player death
        if (health <= 0)
        {
            Destroy(gameObject);
            GM.gameState = GameState.Lose;
        }

        // check if health is less than and change color accordingly
        if(health <= 3)
        {
            newImage.material = Resources.Load("Yellow", typeof(Material)) as Material;
        }
        else if(health <= 1)
        {
            newImage.material = Resources.Load("Red", typeof(Material)) as Material;
        }
    }

    void FixedUpdate()
    {
        //
    }

    public void ExecuteCommand(JSONObject Command)
    {

        if (Command.HasField("move"))
        {
            float x = 0;
            float y = 0;

            // Parse Movement
            if (Command.HasField("setX"))
            {
                x = (float)Command.GetField("setX").n;
            }
            if (Command.HasField("setY"))
            {
                y = (float)Command.GetField("setY").n;
            }

            body.AddForce(new Vector2(x, y), ForceMode2D.Force);
            body.velocity = Vector2.ClampMagnitude(body.velocity, moveSpeed);
        }

        if (Command.HasField("shoot") && reticle)
        {
            GameObject bullet = (GameObject)Instantiate(Resources.Load("Bullet"), transform.forward, Quaternion.identity);
            bullet.GetComponent<Bullet>().player = gameObject;
            bullet.GetComponent<Bullet>().direction = (reticle.transform.position - transform.position).normalized;

            bullet.transform.position = gameObject.transform.position;
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
                j.AddField("setY", moveSpeed);
            }
            else if (moveDown)
            {
                j.AddField("setY", -moveSpeed);
            }
            if (moveLeft)
            {
                j.AddField("setX", -moveSpeed);
            }
            else if (moveRight)
            {
                j.AddField("setX", moveSpeed);
            }
        }

        // Shooting
        if (shoot) {
            j.AddField("shoot", true);
        }
    }

    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.tag == "Bullet")
        {
            // Take damage and show health bar change
            Debug.Log("Damaged!");
            health--;
            newImage.fillAmount -= 0.20f;
        }
    }

}
