using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum GameState
{
    StartMenu,
    Game,
    Pause,
    Win,
    Lose
}

public class GameManager : MonoBehaviour
{

    public LockstepIOComponent lockstep;
    public GameObject player;
    public GameObject mainCamera;
    public GameObject map;
    public int ID;

    private JSONObject j;
    private bool debugMode = false;

    // bool to check if player can connect.
    private bool canPlay = false;

    // Keep the gamestate in a constant state of rotation. 
    public GameState gameState;
    private GameState currentState;
    private GameState previousState;

    // menus & buttons (No need to change these except...)
    private Dictionary<GameState, GameObject> menus;
    private GameObject[] startButton;
    private GameObject[] menuButton;
    private GameObject[] resumeButton;

    // Use this for initialization
    void Start()
    {
        // Menus Handler
        menus = new Dictionary<GameState, GameObject>();
        menus.Add(GameState.StartMenu, GameObject.Find("MainMenu"));
        menus.Add(GameState.Pause, GameObject.Find("PauseMenu"));
        menus.Add(GameState.Win, GameObject.Find("VictoryMenu")); // Placeholder
        menus.Add(GameState.Lose, GameObject.Find("DeadMenu"));

        foreach (GameObject m in menus.Values)
        {
            m.GetComponent<RectTransform>().localPosition = Vector3.zero;
        }

        gameState = GameState.StartMenu;

        // Start --> Game
        startButton = GameObject.FindGameObjectsWithTag("StartButton");
        foreach (GameObject button in startButton)
        {
            button.GetComponent<Button>().onClick.AddListener(() =>
            { // anonymous (delegate) function!
                ChangeState(GameState.Game);
                canPlay = true;
                GameObject.Find("MainMenu").SetActive(false);
                Debug.Log("Click!");
            });
        }

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


    }

    // Update is called once per frame
    void Update()
    {

        currentState = gameState;

        if (lockstep.LockstepReady && currentState == GameState.Game)
        {

            // Reset JSON
            j = new JSONObject();

            if (!player)
            {
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
        if (gameState == GameState.StartMenu)
        {
            ChangeState(GameState.StartMenu);
        }
        // Check for pause state
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (gameState == GameState.Game)
            {
                ChangeState(GameState.Pause);
            }
        }
        previousState = currentState;
    }
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

        menus[state].SetActive(true);
    }
    public void ExecuteCommand(JSONObject Command)
    {

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
