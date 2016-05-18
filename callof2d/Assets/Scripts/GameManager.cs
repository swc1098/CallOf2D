using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum GameState
{
    MainMenu,
    Game,
    Pause,
    Win,
    Lose
}

public class GameManager : MonoBehaviour
{

    public LockstepIOComponent lockstep;
    public GameObject player;
    public string playerID;
    public GameObject mainCamera;
    public GameObject map;
    public string ID;
    public string SocketID;
    public int IDCount;

    private JSONObject j;
    private bool debugMode = false;

    // Keep the gamestate in a constant state of rotation. 
    public GameState gameState;
    //private GameState lastState;

    // menus & buttons (No need to change these except...)
    private Dictionary<GameState, GameObject> menus;
    private GameObject[] startButton;
    private GameObject[] menuButton;
    private GameObject[] resumeButton;

    public GameObject[] spawnLocations;

    // Use this for initialization
    void Start()
    {
        // Menus Handler

        menus = new Dictionary<GameState, GameObject>();
        menus.Add(GameState.MainMenu, GameObject.Find("MainMenu"));
        menus.Add(GameState.Pause, GameObject.Find("PauseMenu"));
        menus.Add(GameState.Win, GameObject.Find("VictoryMenu"));
        menus.Add(GameState.Lose, GameObject.Find("DeadMenu"));
        foreach (GameObject m in menus.Values)
        {
            m.GetComponent<RectTransform>().localPosition = Vector3.zero;
        }

        // Buttons

        // Start --> Game
        startButton = GameObject.FindGameObjectsWithTag("StartButton");
        foreach (GameObject button in startButton)
        {
            button.GetComponent<Button>().onClick.AddListener(() =>
            { // anonymous (delegate) function!
                ChangeState(GameState.Game);

                // New Game
                Connect();
            });
        }

        // Pause --> Game
        resumeButton = GameObject.FindGameObjectsWithTag("ResumeButton");
        foreach (GameObject button in resumeButton)
        {
            button.GetComponent<Button>().onClick.AddListener(() =>
            { // anonymous (delegate) function!
                ChangeState(GameState.Game);
            });
        }

        // ___ --> Start
        menuButton = GameObject.FindGameObjectsWithTag("MenuButton");
        foreach (GameObject button in menuButton)
        {
            button.GetComponent<Button>().onClick.AddListener(() =>
            { // anonymous (delegate) function!
                ChangeState(GameState.MainMenu);
                //Disconnect();
            });
        }

        // Networking

        lockstep = GameObject.Find("NetworkScripts").GetComponent<LockstepIOComponent>();
        //lockstep.GetSocket().AutoConnect = false; // SET IN EDITOR
        lockstep.GetSocket().UseLocal = false;
        lockstep.GetSocket().CloudURL = "ws://zlb3507-lockstep-io-server.herokuapp.com/socket.io/?EIO=4&transport=websocket";
        lockstep.GetSocket().LocalURL = "ws://127.0.0.1:3000/socket.io/?EIO=4&transport=websocket";
        SocketID = "";
        ID = "0";
        gameObject.StoreID(ID);

        // World Setup

        mainCamera = GameObject.Find("Main Camera");
        map = GameObject.Find("Map");
        spawnLocations = GameObject.FindGameObjectsWithTag("SpawnPoint");

        // Hide wall colliders

        if (!debugMode)
        {
            GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
            foreach (GameObject wall in walls)
            {
                Color tmp = wall.GetComponent<SpriteRenderer>().color;
                tmp.a = 0f;
                wall.GetComponent<SpriteRenderer>().color = tmp;
            }
        }

        // Start game
        ChangeState(GameState.MainMenu);

    }

    void Connect()
    {
        lockstep.GetSocket().Connect();
    }

    void Disconnect()
    {
        lockstep.GetSocket().Close();
        player.RemoveID(ID);
        Destroy(player);
    }

    // Update is called once per frame
    void Update()
    {
        // Check for pause state
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (gameState == GameState.Game) ChangeState(GameState.Pause);
            else if (gameState == GameState.Pause) ChangeState(GameState.Game);
        }

        // Queue commands
        if (lockstep.CommandReady && gameState == GameState.Game)
        {
            // Reset JSON
            j = new JSONObject();

            // Add new player
            if (SocketID == "")
            {
                SocketID = lockstep.GetSocket().SocketID;
                playerID = Extensions.GenerateID();
            }
            // Will result in potentially multiple calls to ensure player spawns
            if (!player) {
                j.AddField("spawnplayer", playerID);
            }

            // Only issue commands if there are commands to issue
            if (j.Count > 0)
            {
                j.AddField("gameobject", ID);
                j.AddField("player", SocketID);
                lockstep.issuedCommands.Enqueue(j);
                //Debug.Log("Sent: " + j.ToString());
            }
        }

    }

    public void ExecuteCommand(JSONObject Command)
    {
        // Debug.Log("Received: " + Command.ToString());
        if (Command.HasField("spawnplayer") && !Extensions.idToObject.ContainsKey(Command.GetField("spawnplayer").str))
        {
            GameObject p = (GameObject)Instantiate(Resources.Load("Player"), FindSpawnLocation(), Quaternion.identity);
            p.GetComponent<Player>().AssignID(Command.GetField("spawnplayer").str);
            p.GetComponent<Player>().SocketID = Command.GetField("player").str;

            if (p.GetComponent<Player>().SocketID == SocketID)
            {
                player = p;
                mainCamera.GetComponent<SmoothFollow>().target = player;
                player.GetComponent<Player>().reticle = (GameObject)Instantiate(Resources.Load("Reticle"));
            }
        }

    }

    /*
    void RepeatCommand(string name) {
        switch (name) {
            case "spawnplayer":
                j.AddField("spawnplayer", Extensions.GenerateID());
                break;
        }
    }
    */

    /// <summary>
    /// Change Gamestate based on current menu active
    /// </summary>
    /// <param name="state"></param>
    public void ChangeState(GameState state)
    {
        gameState = state;

        // safety. Deactivate all menus before activating the one we want
        foreach (GameObject m in menus.Values)
        {
            m.SetActive(false);
        }

        if (menus.ContainsKey(state))
        {
            menus[state].SetActive(true);
        }
    }

    public Vector3 FindSpawnLocation()
    {
        foreach (GameObject s in spawnLocations)
        {
            if (s.GetComponent<SpawnPoint>().free)
            {
                return s.transform.position;
            }
        }
        return Vector3.zero;
    }

    public Vector3 FindRandomSpawnLocation()
    {
        List<Vector3> emptyPos = new List<Vector3>();
        foreach (GameObject s in spawnLocations)
        {
            if (s.GetComponent<SpawnPoint>().free)
            {
                emptyPos.Add(s.transform.position);
            }
        }

        if (emptyPos.Count > 0) {
            return emptyPos[Random.Range(0, emptyPos.Count)];
        }
        return Vector3.zero;
    }

}
