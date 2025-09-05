using UnityEngine;

public class GravityPlayer : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    
    [Header("Gravity Settings")]
    public float gravityScale = 3f;
    public float flipCooldown = 0.5f;
    
    private Rigidbody2D rb;
    private bool isGravityFlipped = false;
    private float lastFlipTime;
    private bool isGrounded = false;
    
    [Header("Ground Detection")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
    }
    
    void Update()
    {
        HandleMovement();
        HandleGravityFlip();
        CheckGrounded();
        Debug.Log(isGrounded);
    }
    
    void HandleMovement()
    {
        // Horizontal movement
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }
    
    void HandleGravityFlip()
    {
        // Check if space is pressed and cooldown has passed
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > lastFlipTime + flipCooldown && isGrounded)
        {
            FlipGravity();
            lastFlipTime = Time.time;
        }
    }
    
    void FlipGravity()
    {
        // Flip gravity direction
        isGravityFlipped = !isGravityFlipped;
        
        // Reverse gravity scale
        rb.gravityScale = -rb.gravityScale;
        
        // Flip player sprite to match orientation
        FlipPlayerSprite();
        
        // Optional: Add a small jump effect when flipping
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 0.5f * Mathf.Sign(rb.gravityScale));
    }
    
    void FlipPlayerSprite()
    {
        // Flip the player's scale on Y axis to make them appear upside down
        Vector3 scale = transform.localScale;
        scale.y = Mathf.Abs(scale.y) * (isGravityFlipped ? -1f : 1f);
        transform.localScale = scale;
    }
    
    void CheckGrounded()
    {
        // Determine the direction to cast the ray based on gravity
        Vector2 rayDirection = isGravityFlipped ? Vector2.up : Vector2.down;
        
        // Cast a ray to check if player is grounded
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, groundCheckDistance, groundLayer);
        
        isGrounded = hit.collider != null;
        
        // Visual debug for ground check
        Debug.DrawRay(transform.position, rayDirection * groundCheckDistance, isGrounded ? Color.green : Color.red);
    }
    
    // Optional: If you want to handle jumping with gravity flip
    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // Jump in the opposite direction of gravity
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * Mathf.Sign(rb.gravityScale));
        }
    }
    
    // Optional: If you want separate keys for jump and gravity flip
    void HandleSeparateControls()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // Regular jump
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * Mathf.Sign(rb.gravityScale));
        }
        
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time > lastFlipTime + flipCooldown)
        {
            // Gravity flip with different key
            FlipGravity();
            lastFlipTime = Time.time;
        }
    }
}