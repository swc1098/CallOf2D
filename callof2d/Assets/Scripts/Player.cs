using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public float moveSpeed;
    private float deltaTime;

    private float lerpTime;
    private float lerpSpeed;
    private Vector2 startPosition;
    private Vector2 endPosition;

    private LockstepIOComponent lockstep;
    private JSONObject j;

    // Use this for initialization
    void Start()
    {
        moveSpeed = 20f;

        lerpTime = 0;
        lerpSpeed = 15;
        startPosition = transform.position;
        endPosition = transform.position;

        lockstep = GameObject.Find("NetworkScripts").GetComponent<LockstepIOComponent>();
        gameObject.StoreID();
    }

    // Update is called once per frame
    void Update()
    {
        deltaTime = Time.deltaTime;

        // Ensure lockstep is ready before issuing commands
        if (lockstep.LockstepReady)
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
                lockstep.IssueCommand(j);
            }
            else
            {
                //endPosition = startPosition;
            }
        }

        //increment timer once per frame and lerp
        if (startPosition != endPosition)
        {
            lerpTime += deltaTime;
            transform.position = Vector2.Lerp(startPosition, endPosition, lerpSpeed * lerpTime / 1.0f);
        }
    }

    public void ExecuteCommand(JSONObject Command) {

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

            startPosition = transform.position;
            endPosition = new Vector2(transform.position.x + x, transform.position.y + y);
            lerpTime = 0;
        }

    }

    void HandleKeyInput() {
        
        // on up arrow 
        if (Input.GetKey(KeyCode.W))
        {
            j.AddField("setY", moveSpeed * deltaTime);
            j.AddField("move", true);
        }
        // on down arrow
        else if (Input.GetKey(KeyCode.S))
        {
            j.AddField("setY", -moveSpeed * deltaTime);
            j.AddField("move", true);
        }
        // on left arrow
        if (Input.GetKey(KeyCode.A))
        {
            j.AddField("setX", -moveSpeed * deltaTime);
            j.AddField("move", true);
        }
        // on right arrow
        else if (Input.GetKey(KeyCode.D))
        {
            j.AddField("setX", moveSpeed * deltaTime);
            j.AddField("move", true);
        }

    }

}
