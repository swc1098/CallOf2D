﻿using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{

    public GameObject player;
    public string playerID;
    public Vector2 direction;
    public float moveSpeed;
    public string ID;
    public string SocketID;

    private Vector2 lastPos;
    private Rigidbody2D body;
    private BoxCollider2D col;
    private GameManager GM;
    private JSONObject j;
    private bool destroy;

    // Use this for initialization
    void Start()
    {
        moveSpeed = 300f;

        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0;
        body.drag = 10;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.freezeRotation = true;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;

        GM = GameObject.Find("GameManager").GetComponent<GameManager>();

		// rotate bullet to direction
		float angle = Mathf.Atan2 (direction.y, direction.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
    }

    public void AssignID(string objID)
    {
        ID = objID;
        gameObject.StoreID(ID);
    }

    // Update is called once per frame
    void Update()
    {
        // Ensure lockstep is ready before issuing commands
        if (GM.lockstep.CommandReady && SocketID == GM.SocketID)
        {
            // Reset JSON
            j = new JSONObject();

            // Set gameObject if syncing
            if (!player)
            {
                player = Extensions.idToObject[playerID];
            }

            // Move
            if (direction != Vector2.zero)
            {
                j.AddField("move", true);
            }

            // Create and send basic JSON
            j.AddField("gameobject", ID);
            j.AddField("player", SocketID);
            j.AddField("bulletobj", true);
            j.AddField("playerID", playerID);
            j.AddField("posX", transform.position.x);
            j.AddField("posY", transform.position.y);
            j.AddField("dirX", direction.x);
            j.AddField("dirY", direction.y);
            GM.lockstep.issuedCommands.Enqueue(j);
        }

        if (destroy)
        {
            ThisDestroy();
        }

        // Smoothly move towards correct position
        body.AddForce((lastPos - (Vector2)transform.position), ForceMode2D.Force);
    }

    public void ExecuteCommand(JSONObject Command)
    {
        lastPos = new Vector2((float)Command.GetField("posX").n, (float)Command.GetField("posY").n);

        if (Command.HasField("move"))
        {
            body.AddForce(new Vector2(direction.x * moveSpeed, direction.y * moveSpeed), ForceMode2D.Force);
            body.velocity = Vector2.ClampMagnitude(body.velocity, moveSpeed);
        }

        // Failsafe against missync
        if (Command.HasField("destroy"))
        {
            ThisDestroy();
        }

    }

    void ThisDestroy() {
        gameObject.RemoveID(ID);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {

        if (col.gameObject.tag == "Wall")
        {
            if (SocketID == GM.SocketID)
            {
                j.AddField("destroy", true);
                destroy = true;
            }
            else
            {
                ThisDestroy();
            }
        }

        if (col.gameObject.tag == "Player")
        {
            if (col.gameObject != player)
            {
                //col.gameObject.GetComponent<Player>().TakeDamage();
                if (SocketID == GM.SocketID)
                {
                    j.AddField("destroy", true);
                    destroy = true;
                }
                else
                {
                    ThisDestroy();
                }
            }
        }

    }

}
