using UnityEngine;

public class PlaceableTileScript : MonoBehaviour
{
    public bool canPlace;

    private void Start()
    {
        canPlace = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "ground")
        {
            canPlace = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            canPlace = true;
        }
    }
}
