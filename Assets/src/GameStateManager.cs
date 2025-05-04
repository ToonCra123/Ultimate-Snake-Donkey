using UnityEngine;
using UnityEngine.VFX;

public class GameStateManager : MonoBehaviour
{
    [Header("GraphSelectorHandler")]
    public GraphHandlerScript graphHandler;
    public Transform spawnLocation;

    public GameObject playerPrefab;

    private bool placingState;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        graphHandler.StartPlacingBlock(0); // Replace With a system to pick a block
        placingState = true; // Simple wwtf
    }

    // Update is called once per frame
    void Update()
    {
        if (!graphHandler.isPlacing() && placingState)
        {
            placingState = false; // god kill me

            // spawn player
            Instantiate(playerPrefab, spawnLocation.position, Quaternion.identity);
            return; // exit for some reason fixed something
        } else
        {

        }
    }
}
