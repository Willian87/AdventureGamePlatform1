using System.Collections;
using UnityEngine;

public class PlantTrap : MonoBehaviour
{
    [SerializeField] private int damage = 20; // Damage dealt to the player
    [SerializeField] private float damageInterval = 1f; // Time interval between consecutive damage
    [SerializeField] private Animator animator; // Reference to the plant's animator
    [SerializeField] private AudioSource audioSource; // Reference to the plant's audio source
    [SerializeField] private AudioClip trapActivateSound; // Sound played when the trap is activated

    private bool isPlayerInTrap = false; // Boolean to check if the player is in the trap
    private Coroutine damageCoroutine; // Reference to the damage coroutine

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerCombat player = collision.GetComponent<PlayerCombat>();
            if (player != null)
            {
                isPlayerInTrap = true;
                ActivateTrap();
                damageCoroutine = StartCoroutine(DealDamageOverTime(player));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInTrap = false;
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
            }
        }
        StopTrap();
    }

    private void ActivateTrap()
    {
        if (animator != null)
        {
            animator.SetBool("isAttacking", true);
        }

        if (audioSource != null && trapActivateSound != null)
        {
            audioSource.PlayOneShot(trapActivateSound);
        }
    }

    private void StopTrap()
    {
        if(animator != null)
        {
            animator.SetBool("isAttacking", false);
        }
    }
    private IEnumerator DealDamageOverTime(PlayerCombat player)
    {
        ActivateTrap();
        while (isPlayerInTrap)
        {
            player.TakingDamage(damage);
            yield return new WaitForSeconds(damageInterval);
        }
        StopTrap();
    }
}


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PlantTrap : MonoBehaviour
//{
//    [SerializeField] private int damage = 20; // Damage dealt to the player
//    [SerializeField] private float activationCooldown = 2f; // Cooldown time before the trap can be activated again
//    [SerializeField] private Animator animator; // Reference to the plant's animator
//    [SerializeField] private AudioSource audioSource; // Reference to the plant's audio source
//    [SerializeField] private AudioClip trapActivateSound; // Sound played when the trap is activated
//    [SerializeField] private float damageDelay = 0.5f; // Delay before the trap deals damage to the player

//    private bool canActivate = true; // Boolean to check if the trap can be activated
//    private bool isPlayerInTrap = false; // Boolean to check if the player is in the trap

//    private Coroutine damageCoroutine;

//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        if (collision.CompareTag("Player") && canActivate)
//        {
//            PlayerCombat player = collision.GetComponent<PlayerCombat>();
//            if (player != null)
//            {
//                isPlayerInTrap = true;
//                ActivateTrap(player);
//                damageCoroutine = StartCoroutine(DealDamageAfterDelay(player));
//            }
//        }
//    }

//    private void OnTriggerExit2D(Collider2D collision)
//    {
//        if (collision.CompareTag("Player"))
//        {
//            isPlayerInTrap = false;
//        }
//    }

//    private void ActivateTrap(PlayerCombat player)
//    {
//        if (animator != null)
//        {
//            animator.SetTrigger("Activate");
//            Debug.Log("Cade vc planta?");
//        }

//        if (audioSource != null && trapActivateSound != null)
//        {
//            audioSource.PlayOneShot(trapActivateSound);
//        }

//        canActivate = false;
//        StartCoroutine(DealDamageAfterDelay(player));
//        StartCoroutine(ResetTrap());
//    }

//    private IEnumerator DealDamageAfterDelay(PlayerCombat player)
//    {
        

//        while (isPlayerInTrap)
//        {
//            player.TakingDamage(damage);
//            yield return new WaitForSeconds(damageDelay);
//        }
//    }

//    private IEnumerator ResetTrap()
//    {
//        yield return new WaitForSeconds(activationCooldown);
//        canActivate = true;
//    }
    ////////////////////////////////////
    //Animator anim;

    //private void Start()
    //{
    //    anim = GetComponent<Animator>();
    //}

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if(collision.gameObject.CompareTag("Player"))
    //    {
    //        anim.SetBool("isAttacking", true);
    //        Debug.Log("VEM PLANTA!");
    //    }
    //    else
    //    {
    //        anim.SetBool("isAttacking", false);
    //    }
    //}
//}
