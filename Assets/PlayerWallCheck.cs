using UnityEngine;

public class PlayerWallCheck : MonoBehaviour
{
    public GameObject player;


    private Rigidbody2D playerRB;

    // Update is called once per frame
    void Update()
    {
        playerRB = player.GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            Debug.Log("Trigger hit");
        }
    }
}
