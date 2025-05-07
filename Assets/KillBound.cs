using UnityEngine;

public class KillBound : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player1 = collision.GetComponent<PlayerMovment>();
        var player2 = collision.GetComponent<PlayerMovmentP2>();

        if (player1 != null)
        {
            player1.isDead = true;
        }

        if (player2 != null)
        {
            player2.isDead = true;
        }
    }
}
