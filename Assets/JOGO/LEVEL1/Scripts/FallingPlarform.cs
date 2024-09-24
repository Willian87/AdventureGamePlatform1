using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [Header("Falling Platform Settings")]
    [SerializeField] private float fallDelay = 2f;    // Delay before the platform falls
    [SerializeField] private float respawnDelay = 5f; // Time after falling when the platform is disabled or destroyed
    [SerializeField] private Rigidbody2D rb;          // Reference to the Rigidbody2D component

    private Vector3 initialPosition;   // The initial position of the platform
    private bool isFalling = false;    // To prevent multiple triggers
    private bool isResetting = false;  // Flag to prevent resetting while in the process
    private bool hasPlayerStepped = false; // To track if the player stepped on the platform
    private bool isDisabled = false;    // To check if the platform is currently disabled

    private void Awake()
    {
        // Store the initial position of the platform
        initialPosition = transform.position;

        // Ensure the Rigidbody2D is set and gravity is off initially
        if (!rb) rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static; // Platform starts as static
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player steps on the platform
        if (collision.gameObject.CompareTag("Player") && !isFalling)
        {
            hasPlayerStepped = true;  // Mark that the player has stepped on the platform
            StartCoroutine(FallAfterDelay());
        }
    }

    private IEnumerator FallAfterDelay()
    {
        isFalling = true;

        // Wait for the specified delay
        yield return new WaitForSeconds(fallDelay);

        // Change Rigidbody2D to Dynamic to enable gravity and make the platform fall
        rb.bodyType = RigidbodyType2D.Dynamic;

    }

 
    // This method is called when the platform goes off the screen
    private void OnBecameInvisible()
    {
        // Ensure that the platform only resets if it's currently falling and not already resetting
        if (isFalling && !isResetting)
        {
            ResetPlatform();
        }
    }

    private void ResetPlatform()
    {
        // Ensure the platform isn't already resetting to avoid conflicts
        isResetting = true;

        // Wait a short time before resetting to avoid immediate reactivation
        StartCoroutine(ResetAfterDelay());
    }

    private IEnumerator ResetAfterDelay()
    {
        // Delay the reset to prevent conflicts with deactivation/activation
        yield return new WaitForSeconds(respawnDelay);

        // Reset the platform's position to its initial position
        transform.position = initialPosition;

        // Restore platform properties
        rb.bodyType = RigidbodyType2D.Static;  // Make the platform static again

        // Only re-enable the platform if it was disabled
        if (isDisabled)
        {
            gameObject.SetActive(true);
            isDisabled = false;
        }

        // Reset flags
        isFalling = false;
        isResetting = false;
        
    }

    private void OnDrawGizmosSelected()
    {
        // Optional: visualize where the platform will fall in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
