using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour {

    public LockstepIOComponent lockstep;
    public GameObject player;
    public GameObject mainCamera;

    // Use this for initialization
    void Start () {
        lockstep = GameObject.Find("NetworkScripts").GetComponent<LockstepIOComponent>();
        lockstep.GetSocket().UseLocal = false;
        lockstep.GetSocket().CloudURL = "ws://zlb3507-lockstep-io-server.herokuapp.com/socket.io/?EIO=4&transport=websocket";
        lockstep.GetSocket().LocalURL = "ws://127.0.0.1:3000/socket.io/?EIO=4&transport=websocket";

        mainCamera = GameObject.Find("Main Camera");
    }
	
	// Update is called once per frame
	void Update () {
        if (lockstep.LockstepReady && player == null) {
            SpawnPlayer();
        }
	}

    void SpawnPlayer() {
        player = (GameObject)Instantiate(Resources.Load("Player"), Vector3.zero, Quaternion.identity);
        mainCamera.GetComponent<SmoothFollow>().target = player;
    }
}
