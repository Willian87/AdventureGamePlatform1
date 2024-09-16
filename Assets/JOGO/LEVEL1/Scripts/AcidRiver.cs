using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidRiver : MonoBehaviour
{
    [SerializeField] private int acidDamage = 10;
    [SerializeField] private float damageInterval = 1f; // Time between health loss
    [SerializeField] private Color acidColor = Color.green; // Acid river color
    [SerializeField] private Renderer playerRenderer; // The player's material/renderer for color change
    [SerializeField] private float colorLerpSpeed = 5f;

    private bool isInAcid = false;
    private bool isTakingDamage = false;
    private Color originalColor;

    private PlayerCombat playerCombat;

    private void Start()
    {
        playerCombat = FindObjectOfType<PlayerCombat>();
        if (playerRenderer != null)
        {
            originalColor = playerRenderer.material.color;
        }
        
        PlayerCombat.OnPlayerRespawn += ResetPlayerColor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInAcid = true;
            if (!isTakingDamage)
            {
                StartCoroutine(AcidDamageRoutine());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInAcid = false;
            StopCoroutine(AcidDamageRoutine());
            ResetPlayerColor();
        }
    }

    private IEnumerator AcidDamageRoutine()
    {
        isTakingDamage = true;

        while (isInAcid && playerCombat.currentHealth > 0)
        {
            // Deal damage to the player
            playerCombat.TakingDamage(acidDamage);

            // Gradually change player's color to match acid
            ChangePlayerColor(acidColor);

            // Wait for the damage interval
            yield return new WaitForSeconds(damageInterval);
        }

        isTakingDamage = false;
    }

    private void ChangePlayerColor(Color targetColor)
    {
        if (playerRenderer != null)
        {
            playerRenderer.material.color = Color.Lerp(playerRenderer.material.color, targetColor, colorLerpSpeed * Time.deltaTime);

        }
    }

    private void ResetPlayerColor()
    {
        if (playerRenderer != null)
        {
            playerRenderer.material.color = originalColor;
            
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event when the object is destroyed
        PlayerCombat.OnPlayerRespawn -= ResetPlayerColor;
    }
}

