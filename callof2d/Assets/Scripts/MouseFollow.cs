using UnityEngine;
using System.Collections;

public class MouseFollow : MonoBehaviour {

    
    Ray ray;
    RaycastHit hit;
    Vector2 mouse;
    
    // Use this for initialization
    void Start () {
        // NEED A BOX COLLIDER 3D ON GROUND SURFACE IN ORDER FOR RAYCAST TO WORK
        // WORKS FOR 2D & 3D
	}
	
	// Update is called once per frame
	void Update () {
        
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            transform.position = new Vector3(hit.point.x, hit.point.y, transform.position.z);
        }
        
    }
}
