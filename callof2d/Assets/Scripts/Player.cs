using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public float moveSpeed;
    public GameObject reticle;
    public string ID;
    public string SocketID;

    bool moveUp;
    bool moveDown;
    bool moveLeft;
    bool moveRight;
    bool shoot;

    private Rigidbody2D body;
    private BoxCollider2D col;
    private JSONObject j;
    private GameManager GM;

    // Use this for initialization
    void Start()
    {
        moveSpeed = 105f;

        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0;
        body.drag = 10;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.freezeRotation = true;
        col = GetComponent<BoxCollider2D>();
        col.size = new Vector2(0.14f, 0.14f);

        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void AssignID(string objID) {
        ID = objID;
        gameObject.StoreID(ID);
    }

    // Update is called once per frame
    void Update()
    {
        // Ensure lockstep is ready before issuing commands
        if (GM.lockstep.CommandReady && GM.gameState == GameState.Game && SocketID == GM.SocketID)
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

        if (Command.HasField("spawnbullet"))
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
            j.AddField("spawnbullet", Extensions.GenerateID());

            Vector2 direction = (reticle.transform.position - transform.position).normalized;
            j.AddField("dirX", direction.x);
            j.AddField("dirY", direction.y);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "")
        {
            //
        }
    }

}
