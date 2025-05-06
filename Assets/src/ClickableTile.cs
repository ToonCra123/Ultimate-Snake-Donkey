using UnityEngine;

public class ClickableTile : MonoBehaviour
{
    public SelectionHandler selectionManager; // assign from main script

    private void OnMouseDown()
    {
        if (selectionManager != null)
        {
            selectionManager.BlockClicked(gameObject);
        }
    }
}
