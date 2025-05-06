using UnityEngine;

public class MoveableTile : MonoBehaviour
{
    public GameStateManager state;

    // Update is called once per frame
    void Update()
    {
        if (state == null) return;
        if (state.shouldObjectsMove())
        {
            Debug.Log("Sick");
        }
    }
}
