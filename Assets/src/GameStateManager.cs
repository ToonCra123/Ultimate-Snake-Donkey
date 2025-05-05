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


    public GameObject playerPrefab;

    private GameObject playerShit;

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
        startPlacing();
    }

    // Update is called once per frame
    void Update()
    {
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

    void startPlacing()
    {
        graphHandler.StartPlacingBlock(0); // Replace With a system to pick a block
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
        startPlacing();
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
        startPlacing();
    }
}
