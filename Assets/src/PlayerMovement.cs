using Unity.Jobs;
using UnityEngine;



[SerializeField]
public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;

    public bool isOnWall;

    public BoxCollider2D wallCollider;

    [Header("Game State")]
    public GameStateManager state;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private bool isGrounded;
    private bool isFlipped;
    private float moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    void Update()
    {



        // Handle input
        moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput > 0)
        {

            gameObject.transform.localScale = new Vector3(1, 1, 1);
            isFlipped = false;
        }
        else if (moveInput < 0)
        {
            gameObject.transform.localScale = new Vector3(-1, 1, 1);
            isFlipped = true;
        }
        else if (moveInput == 0) 
        {
            if (isFlipped)
            {
                gameObject.transform.localScale = new Vector3(-1, 1, 1);
            }
            else 
            {
                gameObject.transform.localScale = new Vector3(1, 1, 1);
            }
        }





        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void FixedUpdate()
    {
        // Move
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            isGrounded = true;
        }
    }



    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            isGrounded = false;
        }


    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Kill Trigger")
        {
            GameObject.Destroy(gameObject);
            state.playerDied();
            Debug.Log("Player is dead");
        }


        if (collision.gameObject.tag == "winning_square")
        {
            // do something when he wins
            GameObject.Destroy(gameObject);
            state.playerWon();
        }

    }
}