using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum GameState
{
    Start,
    Game,
    Pause,
    Win,
    Lose
}

public class GameManager : MonoBehaviour {

    public LockstepIOComponent lockstep;
    public GameObject player;
    public GameObject mainCamera;
    public GameObject map;
    public int ID;

    private JSONObject j;
	private bool debugMode = false;

    // Keep the gamestate in a constant state of rotation. 
    public GameState gameState;
    private GameState currentState;
    private GameState previousState;

    // Use this for initialization
    void Start () {
        lockstep = GameObject.Find("NetworkScripts").GetComponent<LockstepIOComponent>();
        //lockstep.GetSocket().AutoConnect = false; // SET IN EDITOR
        lockstep.GetSocket().UseLocal = false;
        lockstep.GetSocket().CloudURL = "ws://zlb3507-lockstep-io-server.herokuapp.com/socket.io/?EIO=4&transport=websocket";
        lockstep.GetSocket().LocalURL = "ws://127.0.0.1:3000/socket.io/?EIO=4&transport=websocket";

        mainCamera = GameObject.Find("Main Camera");
        map = GameObject.Find("Map");
        ID = Extensions.GenerateID();
        gameObject.StoreID(ID);

        lockstep.GetSocket().Connect();

		if (!debugMode) {
			GameObject[] walls = GameObject.FindGameObjectsWithTag ("Wall");
			foreach (GameObject wall in walls) {
				Color tmp = wall.GetComponent<SpriteRenderer> ().color;
				tmp.a = 0f;
				wall.GetComponent<SpriteRenderer> ().color = tmp;
			}
		}
    }
	
	// Update is called once per frame
	void Update () {
        if (lockstep.LockstepReady) {

            // Reset JSON
            j = new JSONObject();

            if (!player) {
                j.AddField("spawnplayer", true);
            }

            // issue the command above
            // Only issue commands if there are commands to issue
            if (j.Count > 0)
            {
                j.AddField("gameobject", ID);
                lockstep.issuedCommands.Enqueue(j);
            }
        }
	}

    public void ExecuteCommand(JSONObject Command) {

        if (Command.HasField("spawnplayer"))
        {
            if (!player)
            {
                player = (GameObject)Instantiate(Resources.Load("Player"), Vector3.zero, Quaternion.identity);
                player.GetComponent<Player>().reticle = (GameObject)Instantiate(Resources.Load("Reticle"));
                mainCamera.GetComponent<SmoothFollow>().target = player;
            }
        }

    }
}
