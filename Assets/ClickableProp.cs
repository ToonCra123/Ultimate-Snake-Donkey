using UnityEngine;

public class ClickableProp : MonoBehaviour
{
    private GameManager gameManager;

    public void Init(GameManager gm)
    {
        gameManager = gm;
    }

    void OnMouseDown()
    {
        if (gameManager != null)
            gameManager.OnPropSelected(gameObject);
    }


}
