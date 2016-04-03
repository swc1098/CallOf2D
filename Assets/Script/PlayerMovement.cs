using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    public float moveSpeed = 0f;
    private float movex = 0f;
    private float movey = 0f;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        movex = Input.GetAxis("Horizontal");
        movey = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(movex, movey, 0.0f);
        gameObject.GetComponent<Rigidbody2D>().velocity = movement * moveSpeed;

    }
}
