using System.Collections;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    private enum BossState { Idle, Angry }
    private BossState currentState = BossState.Idle;

    [SerializeField] private float detectionRange = 1.5f;
    [SerializeField] private LayerMask playerLayerMask;

    private Animator anim;
    private Transform player;
    private BossWeapon weapon; // Reference to the BossWeapon script
    private bool hasAttacked; // Track if the boss has attacked

    private void Start()
    {
        anim = GetComponent<Animator>();
        weapon = GetComponent<BossWeapon>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player object not found. Ensure the player GameObject has the 'Player' tag.");
        }

        ChangeState(BossState.Idle);
    }

    private void Update()
    {
        if (!hasAttacked)
        {
            CheckForPlayer();
        }
        HandleStateMachine();
    }

    private void HandleStateMachine()
    {
        if (currentState == BossState.Angry && !hasAttacked)
        {
            StartCoroutine(PerformSingleRandomAttack());
        }
    }

    private void ChangeState(BossState newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        if (newState == BossState.Idle)
        {
            anim.SetTrigger("Idle");
        }
        else if (newState == BossState.Angry)
        {
            anim.SetTrigger("isAngry");
        }
    }

    private void CheckForPlayer()
    {
        if (currentState != BossState.Idle || player == null) return;

        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, detectionRange, playerLayerMask);
        Debug.DrawRay(transform.position, directionToPlayer * detectionRange, Color.red);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            ChangeState(BossState.Angry);
        }
    }

    private IEnumerator PerformSingleRandomAttack()
    {
        hasAttacked = true;

        // Choose a random attack
        int randomAttack = Random.Range(1, 4);
        switch (randomAttack)
        {
            case 1:
                yield return weapon.PerformMagicFireAttack();
                break;
            case 2:
                yield return weapon.PerformMagicBladeAttack();
                break;
            case 3:
                yield return weapon.PerformMagicLightningAttack();
                break;
        }

        yield return new WaitForSeconds(0.5f); // Small delay before returning to Idle

        // Reset to Idle state and prepare for next detection
        hasAttacked = false;
        ChangeState(BossState.Idle);
    }
}




