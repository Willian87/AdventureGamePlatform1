using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls player movement, jumping, and climbing.
/// </summary>
public class PlayerController : MonoBehaviour
{
    private static PlayerController instance;

    [Range(0, 5)] [SerializeField] private float groundCheckRadius = 1f;
    [SerializeField] private float speed = 10f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float jumpForce = 50f;

    [SerializeField] private Transform ladderCheck;
    [SerializeField] private LayerMask ropeLayerMask;
    [SerializeField] private float climbSpeed = 5f;
    [SerializeField] private float ladderCheckDistance = 1f;

    private Animator anim;
    private Rigidbody2D rb;
    private AudioSource audioSource;

    private float horizontalInput;
    [SerializeField] private bool isGrounded;
    private bool isJumping;
    private float jumpTimeCounter;
    [SerializeField] private bool isClimbing;
    private bool isFacingRight = true;

    [SerializeField] private float jumpTime = 0.5f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        HandleJump();
        HandleClimb();
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    /// <summary>
    /// Handles player movement.
    /// </summary>
    public void HandleMovement()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);

        if (horizontalInput > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (horizontalInput < 0 && isFacingRight)
        {
            Flip();
        }
    }

    /// <summary>
    /// Handles player jumping.
    /// </summary>
    private void HandleJump()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rb.velocity = Vector2.up * jumpForce;
        }

        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }

        if (isJumping && Input.GetButton("Jump"))
        {
            if (jumpTimeCounter > 0)
            {
                rb.velocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }
    }

    /// <summary>
    /// Handles player climbing ladders.
    /// </summary>
    private void HandleClimb()
    {
        float verticalInput = Input.GetAxisRaw("Vertical");
        RaycastHit2D hitRope = Physics2D.Raycast(ladderCheck.position, Vector2.up, ladderCheckDistance, ropeLayerMask);

        if (hitRope.collider != null && verticalInput != 0)
        {
                isClimbing = true; 
        }
        else
        {
            isClimbing = false;
        }

       
        if (isClimbing)
        {
            rb.gravityScale = 0;
            rb.velocity = new Vector2(rb.velocity.x, verticalInput * climbSpeed);
        }
        else
        {
            rb.gravityScale = 3; // Set gravity scale to default value
        }
    }

    /// <summary>
    /// Flips the player's facing direction.
    /// </summary>
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    /// <summary>
    /// Updates player animations.
    /// </summary>
    private void UpdateAnimations()
    {
        bool isWalking = Mathf.Abs(horizontalInput) > 0.1f;

        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);

        if (isJumping)
        {
            if (isWalking)
            {
                anim.SetTrigger("WalkingJump");
            }
            else
            {
                anim.SetTrigger("Jump");
            }
        }
        else
        {
            anim.ResetTrigger("Jump");
            anim.ResetTrigger("WalkingJump");
        }

        anim.SetBool("isClimbing", isClimbing);
    }

    /// <summary>
    /// Draws debug gizmos.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(ladderCheck.position, Vector2.up * ladderCheckDistance);
    }
}

