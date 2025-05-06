using UnityEngine;

public class SelectionHandler : MonoBehaviour
{
    public GameObject[] placeablePrefabs;

    // this is constant they should never change
    // only four spawn points
    public GameObject[] spawnChoices;

    [Header("State Manager")]
    public GameStateManager state;

    private GameObject[] choices;
   

    bool placing;
    
    void Start()
    {
        placing = false;
        choices = new GameObject[spawnChoices.Length];
    }


    public void StartSelectingBlock()
    {
        // Always clean up previous choices if any
        for (int i = 0; i < choices.Length; i++)
        {
            if (choices[i] != null)
            {
                Destroy(choices[i]);
                choices[i] = null;
            }
        }
        
        placing = true; // Indicates that selection choices are now being set up / are active.

        for (int i = 0; i < spawnChoices.Length; i++) // Iterate based on spawnChoices
        {
            if (i < choices.Length) // Ensure we don't go out of bounds for choices array
            {
                GameObject spawn = spawnChoices[i];
                GameObject choice = Instantiate(placeablePrefabs[GetRandomPrefabIndex()], spawn.transform.position, Quaternion.identity);
                choice.transform.position = new Vector3(choice.transform.position.x, choice.transform.position.y, -7f);
                choice.transform.localScale = new Vector3(5, 5, 5);

                ClickableTile cb = choice.AddComponent<ClickableTile>();
                cb.selectionManager = this;

                choices[i] = choice;
            }
        }
    }

    public void BlockClicked(GameObject gameObj)
    {
        GameObject shitter = Instantiate(gameObj, new Vector3(0, 10000, 10000), Quaternion.identity);
        
        // Destroy all visible choices (which includes gameObj as it's one of the choices)
        foreach (GameObject choiceInArray in choices) { 
            if (choiceInArray != null) 
            {
                Destroy(choiceInArray);
            }
        }
        for (int i = 0; i < choices.Length; i++)
        {
            choices[i] = null;
        }
        
        shitter.transform.localScale = new Vector3(1, 1, 1); // Reset scale for its role as a template
        shitter.SetActive(false); // Keep the template inactive
        state.SelectedObject(shitter);
        placing = false; // Selection process is over for SelectionHandler
    }

    public int GetRandomPrefabIndex()
    {
        return Random.Range(0, placeablePrefabs.Length);
    }
}
