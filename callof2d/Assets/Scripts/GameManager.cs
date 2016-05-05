using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    GameObject player;
    
    // Use this for initialization
	void Start () {
        player = (GameObject)Instantiate(Resources.Load("Player"), Vector3.zero, Quaternion.identity);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
