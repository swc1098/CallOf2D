using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public float moveSpeed;

    private LockstepIOComponent lockstep;
    private JSONObject j;

    public Rigidbody2D body;

    private GameManager GM;

    // Use this for initialization
    void Start()
    {
        moveSpeed = 70f;

        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0;
        body.drag = 10;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.freezeRotation = true;
        GetComponent<BoxCollider2D>().size = new Vector2(0.14f, 0.14f);

        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameObject.StoreID();
    }

    // Update is called once per frame
    void Update()
    {
        // Ensure lockstep is ready before issuing commands
        if (GM.lockstep.LockstepReady)
        {
            // Reset JSON
            j = new JSONObject();

            // Handle Key Input
            HandleKeyInput();

            // issue the command above
            // Only issue commands if there are commands to issue
            if (j.Count > 0)
            {
                j.AddField("gameobject", gameObject.GetInstanceID());
                GM.lockstep.IssueCommand(j);
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

    }

    void HandleKeyInput()
    {

        // on up arrow 
        if (Input.GetKey(KeyCode.W))
        {
            j.AddField("setY", moveSpeed);
            j.AddField("move", true);
        }
        // on down arrow
        else if (Input.GetKey(KeyCode.S))
        {
            j.AddField("setY", -moveSpeed);
            j.AddField("move", true);
        }
        // on left arrow
        if (Input.GetKey(KeyCode.A))
        {
            j.AddField("setX", -moveSpeed);
            j.AddField("move", true);
        }
        // on right arrow
        else if (Input.GetKey(KeyCode.D))
        {
            j.AddField("setX", moveSpeed);
            j.AddField("move", true);
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
