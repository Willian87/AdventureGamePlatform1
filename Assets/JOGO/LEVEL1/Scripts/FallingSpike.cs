using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FallingSpike : MonoBehaviour
{
    public PlayerCombat player;
    [SerializeField] private int playerDamage = 10;
    [Header("Spike Settings")]
    //[SerializeField] private float fallSpeed = 5f;  // Speed at which the spike falls
    [SerializeField] private LayerMask groundLayer; // Layer for ground detection
    [SerializeField] private LayerMask playerLayer; // Layer for player detection
    [SerializeField] private float detectionRange = 10f; // Range of the raycast
    [SerializeField] private float disableDelay = 2f; // Time delay before disabling the spike

    [Header("References")]
    [SerializeField] private Animator animator;  // Reference to the Animator component
    [SerializeField] private Rigidbody2D rb;     // Reference to the Rigidbody2D component
    [SerializeField] private Collider2D spikeCollider; // Reference to the Collider2D component

    private bool hasHit = false;  // To check if the spike has already hit something

    void Awake()
    {
        player = FindObjectOfType<PlayerCombat>();
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!animator) animator = GetComponent<Animator>();
        if (!spikeCollider) spikeCollider = GetComponent<Collider2D>();

        rb.gravityScale = 0;  // Start with gravity off
    }

    void OnEnable()
    {
        ResetSpike();  // Reset the spike when enabled (useful for object pooling)
    }

    void Update()
    {
        if (!hasHit)
        {
            DetectPlayerWithRaycast();  // Check if the player is within detection range
        }
    }

    private void DetectPlayerWithRaycast()
    {
        // Cast a ray downward from the spike to detect the player
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, detectionRange, playerLayer);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            Release();  // Start the spike falling when the player is detected
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasHit) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            HandleHitPlayer();
            player.TakingDamage(playerDamage);
        }
        else if (IsGroundCollision(collision))
        {
            HandleHitGround();
        }
    }

    private bool IsGroundCollision(Collision2D collision)
    {
        // Check if the collision was with the ground layer
        return (groundLayer.value & (1 << collision.gameObject.layer)) > 0;
    }

    private void HandleHitPlayer()
    {
        hasHit = true;
        rb.velocity = Vector2.zero;
        //rb.gravityScale = 0;

        animator.SetTrigger("HitPlayer");
        
        DisableSpikeAfterDelay();
    }

    private void HandleHitGround()
    {
        hasHit = true;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;

        animator.SetTrigger("HitGround");

        DisableSpikeAfterDelay();
    }

    private void ResetSpike()
    {
        hasHit = false;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        spikeCollider.enabled = true;  // Reset the collider in case it was disabled
    }

    public void Release()
    {
        rb.gravityScale = 1;  // Enable gravity to make the spike fall
    }

    private void DisableSpikeAfterDelay()
    {
        // Disable the spike after the specified delay
        Invoke(nameof(DisableSpike), disableDelay);
    }

    private void DisableSpike()
    {
        gameObject.SetActive(false); // Disable the spike object
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the detection range (optional)
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * detectionRange);
    }
}
