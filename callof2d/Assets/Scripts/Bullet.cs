using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    public GameObject player;
    public Vector2 direction;
    public float moveSpeed;
    public string ID;
    public string SocketID;

    private Rigidbody2D body;
    private CircleCollider2D col;
    private GameManager GM;
    private JSONObject j;

    // Use this for initialization
    void Start () {
        moveSpeed = 300f;

        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0;
        body.drag = 10;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.freezeRotation = true;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        col = GetComponent<CircleCollider2D>();
        col.radius = 0.09f;
        col.isTrigger = true;

        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void AssignID(string objID)
    {
        ID = objID;
        gameObject.StoreID(ID);
    }

    // Update is called once per frame
    void Update () {

        // Ensure lockstep is ready before issuing commands
        if (GM.lockstep.CommandReady && SocketID == GM.SocketID)
        {
            // Reset JSON
            j = new JSONObject();

            // Move
            if (direction != Vector2.zero)
            {
                j.AddField("move", true);
            }

            // issue the command above
            // Only issue commands if there are commands to issue
            if (j.Count > 0)
            {
                j.AddField("gameobject", ID);
                GM.lockstep.issuedCommands.Enqueue(j);
            }

        }

    }

    public void ExecuteCommand(JSONObject Command) {

        if (Command.HasField("move"))
        {
            body.AddForce(new Vector2(direction.x * moveSpeed, direction.y * moveSpeed), ForceMode2D.Force);
            body.velocity = Vector2.ClampMagnitude(body.velocity, moveSpeed);
        }

    }

    void OnTriggerEnter2D(Collider2D col)
    {

        if (col.gameObject.tag == "Wall")
        {
            gameObject.RemoveID(ID);
            Destroy(gameObject);
        }

        if (col.gameObject.tag == "Player")
        {
            if(col.gameObject != player)
            {
                // reduce health and have the fill amount show it.
                player.gameObject.GetComponent<Player>().health--;
                player.gameObject.GetComponent<Player>().newImage.fillAmount -= 0.15f;
                Destroy(gameObject);
            }
        }

    }

}
