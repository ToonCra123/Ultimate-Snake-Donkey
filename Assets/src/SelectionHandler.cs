using UnityEngine;

public class SelectionHandler : MonoBehaviour
{
    public GameObject[] placeablePrefabs;

    // this is constant they should never change
    // only four spawn points
    public GameObject[] spawnChoices;

    private GameObject[] choices;
    

    bool placing;
    
    void Start()
    {
        placing = false;
        choices = new GameObject[spawnChoices.Length];
    }

    void Update()
    {
        if (!placing) return;
    }


    public void StartSelectingBlock()
    {
        if (placing) Start(); 
        placing = true;
        int i = 0;
        foreach (GameObject spawn in spawnChoices)
        {
            GameObject choice = Instantiate(placeablePrefabs[GetRandomPrefabIndex()], spawn.transform.position, Quaternion.identity);
            choice.transform.position.z = -7;
            choice.transform.localScale = new Vector3(5, 5, 5);
            choices[i] = choice;
            i++;
        }
    }

    public int GetRandomPrefabIndex()
    {
        return Random.Range(0, placeablePrefabs.Length);
    }
}
