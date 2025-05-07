using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerMovmentP2 : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 14f;
    public float wallSlideSpeed = 2f;
    public float wallJumpForceX = 10f;
    public float wallJumpForceY = 12f;
    public float wallJumpTime = 0.2f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSliding;
    private float moveInput;
    private bool facingRight = true;
    private bool wallJumping;
    private float wallJumpCounter;
    private bool groundBelowWhileWalling;
    private bool shouldWallKick;

    private bool touchingLeftWall;
    private bool touchingRightWall;
    private float wallJumpCooldown = 0.5f;
    private float wallJumpTimer = 0f;
    private bool jumpHeld = false;
    private bool deathReported = false;








    [Header("Checks")]
    public Transform groundCheck;
    public Transform wallCheck;
    public LayerMask groundLayer;
    public float checkRadius = 0.2f;
    public bool isDead = false;


    [Header("Animation")]
    public SpriteRenderer spriteRenderer;
    public Sprite idleSprite;
    public Sprite walkSprite;
    public Sprite jumpSprite;
    public Sprite slideSprite;
    public Sprite deadSprite; // not used yet

    private float walkAnimTimer = 0f;
    private float walkAnimSpeed = 0.2f;
    private bool walkingFrame = false;


    [Header("Controls")]
    public KeyCode leftKey = KeyCode.LeftArrow;
    public KeyCode rightKey = KeyCode.RightArrow;
    public KeyCode jumpKey = KeyCode.UpArrow;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isDead)
        {
            if (!deathReported)
            {
                deathReported = true;
                StartCoroutine(ReportDeath());
            }

            HandleAnimation();
            return;
        }
        else
        {
            deathReported = false; // reset when player comes back to life
        }


        moveInput = 0;
        if (Input.GetKey(leftKey)) moveInput = -1;
        if (Input.GetKey(rightKey)) moveInput = 1;
        groundBelowWhileWalling = isTouchingWall && RaycastDown(0.6f);


        if (moveInput != 0 && !wallJumping)
            Flip();

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        touchingLeftWall = RaycastLeft(0.4f);
        touchingRightWall = RaycastRight(0.4f);
        isTouchingWall = touchingLeftWall || touchingRightWall;


        if (isTouchingWall && !isGrounded)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }

        if (Input.GetKeyDown(jumpKey)) jumpHeld = true;
        if (Input.GetKeyUp(jumpKey)) jumpHeld = false;

        if (Input.GetKeyDown(jumpKey))
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
            else if (isWallSliding && wallJumpTimer <= 0f)
            {
                wallJumping = true;
                wallJumpCounter = wallJumpTime;
                wallJumpTimer = wallJumpCooldown; // set cooldown

                float wallDir = touchingLeftWall ? 1 : -1;
                Vector2 jumpDir = new Vector2(wallDir, 1).normalized;

                rb.linearVelocity = new Vector2(jumpDir.x * wallJumpForceX, jumpDir.y * wallJumpForceY);
            }

            if (groundBelowWhileWalling && isTouchingWall)
            {
                shouldWallKick = true;
            }
        }



        // Decrease wall jump cooldown
        if (wallJumpTimer > 0f)
            wallJumpTimer -= Time.deltaTime;


        HandleAnimation();


    }

    void FixedUpdate()
    {
        if (isDead) return;

        if (!wallJumping)
        {
            float appliedSpeed = moveSpeed;

            // Prevent movement back toward the wall if cooldown is active
            if (wallJumpTimer > 0f)
            {
                if (touchingLeftWall && moveInput < 0)
                    moveInput = 0; // trying to move into left wall
                if (touchingRightWall && moveInput > 0)
                    moveInput = 0; // trying to move into right wall
            }

            if (groundBelowWhileWalling && isTouchingWall)
            {
                Debug.Log("Ground below while walling");
                appliedSpeed *= 0.2f;
            }
            else
            {
                Debug.Log("Ground not below while walling" + groundBelowWhileWalling + isTouchingWall);
            }

            rb.linearVelocity = new Vector2(moveInput * appliedSpeed, rb.linearVelocity.y);
        }

        if (shouldWallKick)
        {
            if (facingRight)
            {
                rb.AddForce(new Vector2(-jumpForce * 2, 0), ForceMode2D.Impulse);
            }
            else
            {
                rb.AddForce(new Vector2(jumpForce * 2, 0), ForceMode2D.Impulse);
            }
            shouldWallKick = false;
            Debug.Log("Wall kick impulse!");
        }
    }


    void Flip()
    {
        if ((facingRight && moveInput < 0) || (!facingRight && moveInput > 0))
        {
            facingRight = !facingRight;
            Vector3 scaler = transform.localScale;
            scaler.x *= -1;
            transform.localScale = scaler;
        }
    }

    void HandleAnimation()
    {
        if (!spriteRenderer) return;

        if (isDead)
        {
            spriteRenderer.sprite = deadSprite;
            Debug.Log("Player is dead ANIM");
            return;
        }

        // 1. Wall jump slide/kick override
        if (shouldWallKick)
        {
            spriteRenderer.sprite = slideSprite;
        }
        // 2. Jumping (airborne, not sliding)
        else if (!isGrounded && !isWallSliding)
        {
            spriteRenderer.sprite = jumpSprite;
        }
        // 3. Wall sliding (not actively kicking)
        else if (isWallSliding)
        {
            spriteRenderer.sprite = slideSprite;
        }
        // 4. Walking on ground
        else if (Mathf.Abs(moveInput) > 0.1f && isGrounded)
        {
            spriteRenderer.sprite = walkSprite;
        }
        // 5. Idle on ground
        else if (isGrounded)
        {
            spriteRenderer.sprite = idleSprite;
        }
    }









    public bool RaycastDown(float distance)
    {
        int groundLayerMask = LayerMask.GetMask("ground");
        float gap = 0.5f;

        Vector2 startVec1 = (Vector2)transform.position + new Vector2(gap, 0);
        Vector2 startVec2 = (Vector2)transform.position;
        Vector2 startVec3 = (Vector2)transform.position + new Vector2(-gap, 0);

        Vector2 dir = Vector2.down;

        RaycastHit2D hit1 = Physics2D.Raycast(startVec1, dir, distance, groundLayerMask);
        RaycastHit2D hit2 = Physics2D.Raycast(startVec2, dir, distance, groundLayerMask);
        RaycastHit2D hit3 = Physics2D.Raycast(startVec3, dir, distance, groundLayerMask);

        Debug.DrawRay(startVec1, dir * distance, Color.red);
        Debug.DrawRay(startVec2, dir * distance, Color.green);
        Debug.DrawRay(startVec3, dir * distance, Color.blue);

        return hit1.collider != null || hit2.collider != null || hit3.collider != null;
    }

    public bool RaycastLeft(float distance)
    {
        int groundLayerMask = LayerMask.GetMask("ground");
        float gap = 0.45f;

        Vector2 startVec1 = (Vector2)transform.position + new Vector2(0, gap);
        Vector2 startVec2 = (Vector2)transform.position;
        Vector2 startVec3 = (Vector2)transform.position + new Vector2(0, -gap);

        Vector2 dir = Vector2.left;

        RaycastHit2D hit1 = Physics2D.Raycast(startVec1, dir, distance, groundLayerMask);
        RaycastHit2D hit2 = Physics2D.Raycast(startVec2, dir, distance, groundLayerMask);
        RaycastHit2D hit3 = Physics2D.Raycast(startVec3, dir, distance, groundLayerMask);

        Debug.DrawRay(startVec1, dir * distance, Color.red);
        Debug.DrawRay(startVec2, dir * distance, Color.red);
        Debug.DrawRay(startVec3, dir * distance, Color.red);

        return hit1.collider != null || hit2.collider != null || hit3.collider != null;
    }


    public bool RaycastRight(float distance)
    {
        int groundLayerMask = LayerMask.GetMask("ground");
        float gap = 0.45f;

        Vector2 startVec1 = (Vector2)transform.position + new Vector2(0, gap);
        Vector2 startVec2 = (Vector2)transform.position;
        Vector2 startVec3 = (Vector2)transform.position + new Vector2(0, -gap);

        Vector2 dir = Vector2.right;

        RaycastHit2D hit1 = Physics2D.Raycast(startVec1, dir, distance, groundLayerMask);
        RaycastHit2D hit2 = Physics2D.Raycast(startVec2, dir, distance, groundLayerMask);
        RaycastHit2D hit3 = Physics2D.Raycast(startVec3, dir, distance, groundLayerMask);

        Debug.DrawRay(startVec1, dir * distance, Color.cyan);
        Debug.DrawRay(startVec2, dir * distance, Color.cyan);
        Debug.DrawRay(startVec3, dir * distance, Color.cyan);

        return hit1.collider != null || hit2.collider != null || hit3.collider != null;
    }


    IEnumerator ReportDeath()
    {
        string username = PlayerPrefs.GetString("Player2Username", "Unknown");

        string jsonData = JsonUtility.ToJson(new UsernameWrapper { username = username });
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest("https://api.toonhosting.net/player/die", "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Death reported successfully for " + username);
        }
        else
        {
            Debug.LogWarning("Error reporting death: " + request.error);
        }
    }

    [System.Serializable]
    public class UsernameWrapper
    {
        public string username;
    }
}
