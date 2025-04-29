using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float playerSpeed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GetComponent<Rigidbody2D>().AddForce(Vector3.left * playerSpeed, ForceMode2D.Impulse);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            GetComponent<Rigidbody2D>().AddForce(Vector3.right * playerSpeed, ForceMode2D.Impulse);
        }
    }
}
