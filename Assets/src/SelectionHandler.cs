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
        if (placing) Start(); 
        placing = true;
        int i = 0;
        foreach (GameObject spawn in spawnChoices)
        {
            GameObject choice = Instantiate(placeablePrefabs[GetRandomPrefabIndex()], spawn.transform.position, Quaternion.identity);
            choice.transform.position = new Vector3(choice.transform.position.x, choice.transform.position.y, -7f);
            choice.transform.localScale = new Vector3(5, 5, 5);

            ClickableTile cb = choice.AddComponent<ClickableTile>();
            cb.selectionManager = this;

            choices[i] = choice;
            i++;
        }
    }

    public void BlockClicked(GameObject gameObj)
    {
        foreach (GameObject choice in choices) 
            choice.SetActive(false);

        gameObj.GetComponent<ClickableTile>().selectionManager = null;
        gameObj.transform.localScale = new Vector3(1, 1, 1);
        state.SelectedObject(gameObj);
    }

    public int GetRandomPrefabIndex()
    {
        return Random.Range(0, placeablePrefabs.Length);
    }
}
