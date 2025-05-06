using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.VFX;

public class GameStateManager : MonoBehaviour
{
    [Header("GraphSelectorHandler")]
    public GraphHandlerScript graphHandler;
    public Transform spawnLocation;


    [Header("Camera Shit")]
    public Camera mainCam;


    [Header("Selection Box")]
    public GameObject selectionBox;


    [Header("Player Prefab")]
    public GameObject playerPrefab;
    private GameObject playerShit;


    [Header("Selection Handler")]
    public SelectionHandler selectionHandler;


    private GAME_STATES gameState;
    private enum GAME_STATES
    {
        PLACING = 0,
        SELECTING = 1,
        PLAYING = 2
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selectionBox.SetActive(false);

        startSelecting();
    }

    // Update is called once per frame
    void Update()
    {

        // I have no idea what is happening
        if (!graphHandler.isPlacing() && gameState == GAME_STATES.PLACING)
        {
            gameState = GAME_STATES.PLAYING; // god kill me

            // spawn player
            playerShit = Instantiate(playerPrefab, spawnLocation.position, Quaternion.identity);

            PlayerMovement2D pm2d = playerShit.GetComponent<PlayerMovement2D>();

            if(pm2d != null)
            {
                pm2d.state = this;
            }

            return; // exit for some reason fixed something
        } else if (gameState == GAME_STATES.SELECTING)
        {
            return;
        } else if (gameState == GAME_STATES.PLAYING)
        {
            CameraScriptShit cameraUtil = mainCam.GetComponent<CameraScriptShit>();
            if (cameraUtil != null)
            {
                if (!cameraUtil.isFollowing())
                {
                    cameraUtil.StartFollowCameraTransition(playerShit.transform);
                }
            }
        }
    }

    void startPlacing(int i)
    {
        graphHandler.StartPlacingBlock(i); // Replace With a system to pick a block
        gameState = GAME_STATES.PLACING; // Simple wwtf
    }

    void startPlacing(GameObject gmobj)
    {
        graphHandler.StartPlacingBlock(gmobj); // Replace With a system to pick a block
        gameState = GAME_STATES.PLACING; // Simple wwtf
    }

    public void playerDied()
    {
        CameraScriptShit cameraUtil = mainCam.GetComponent<CameraScriptShit>();
        if (cameraUtil != null)
        {
            if (cameraUtil.isFollowing())
            {
                cameraUtil.StartSceneViewTransition();
            }
        }
        startSelecting();
    }

    public void playerWon()
    {
        CameraScriptShit cameraUtil = mainCam.GetComponent<CameraScriptShit>();
        if (cameraUtil != null)
        {
            if (cameraUtil.isFollowing())
            {
                cameraUtil.StartSceneViewTransition();
            }
        }
        startSelecting();
    }

    public void SelectedObjct(int index)
    {
        stopPlacing();
        startPlacing(index);
    }

    public void SelectedObject(GameObject gameObjectIn)
    {
        stopPlacing();
        startPlacing(gameObjectIn);
    }

    public void startSelecting()
    {
        // Clean up any previous player object
        if (playerShit != null)
        {
            Destroy(playerShit);
            playerShit = null;
        }
        
        gameState = GAME_STATES.SELECTING;
        selectionBox.SetActive(true);
        selectionHandler.StartSelectingBlock();
    }

    public void stopPlacing()
    {
        selectionBox.SetActive(false);
    }

    public bool shouldObjectsMove()
    {
        return gameState == GAME_STATES.PLAYING;
    }
}
