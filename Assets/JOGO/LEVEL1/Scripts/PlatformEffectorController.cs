using System.Collections;
using UnityEngine;

public class PlatformEffectorController : MonoBehaviour
{
    private Collider2D playerCollider;
    private PlatformEffector2D effector;

    [SerializeField] private float fallThroughDuration = 1f;  // Time the player can fall through the platform

    void Start()
    {
        playerCollider = GetComponent<Collider2D>();  // Reference the player's collider
        effector = GetComponent<PlatformEffector2D>(); // Assuming there's only one Platform Effector
    }

    void Update()
    {
        HandlePlatformEffector();
    }

    private void HandlePlatformEffector()
    {
        // Press Down key to fall through the platform
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(FallThroughPlatform());
        }

        // Press Jump key to quickly reset and allow climbing back up
        if (Input.GetKeyDown(KeyCode.Space))
        {
            effector.rotationalOffset = 0f; // Reset the effector to allow jumping back up
        }
    }

    private IEnumerator FallThroughPlatform()
    {
        // Temporarily disable the platform effector for the player
        effector.rotationalOffset = 180f; // Rotate the effector to allow falling through

        yield return new WaitForSeconds(fallThroughDuration);

        effector.rotationalOffset = 0f; // Reset to original state after falling through
    }
}

