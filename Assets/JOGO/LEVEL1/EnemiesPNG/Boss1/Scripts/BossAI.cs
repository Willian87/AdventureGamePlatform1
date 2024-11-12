using System.Collections;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    private enum BossState { Idle, Walk, Attack, Angry, Defeated }
    private BossState currentState;

    private Animator animator;
    private Transform player;

    [Header("Attack Settings")]
    public float attackRange = 5f;
    private bool isAttacking = false;
    private bool isAngry = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentState = BossState.Idle;
    }

    void Update()
    {
       
        //HandleStateMachine();
    }

    private void HandleStateMachine()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case BossState.Idle:
                HandleIdleState(distanceToPlayer);
                break;
            case BossState.Walk:
                HandleWalkState(distanceToPlayer);
                break;
            case BossState.Attack:
                if (!isAttacking)
                {
                    StartCoroutine(ExecuteRandomAttack());
                }
                break;
            case BossState.Angry:
                if (!isAttacking)
                {
                    StartCoroutine(ExecuteRandomAttack());
                }
                break;
            case BossState.Defeated:
                HandleDefeatedState();
                break;
        }
    }

    private void HandleIdleState(float distanceToPlayer)
    {
        animator.SetBool("IsWalking", false);

        if (distanceToPlayer < attackRange)
        {
            ChangeState(BossState.Attack);
        }
        else if (distanceToPlayer < attackRange * 2)
        {
            ChangeState(BossState.Walk);
        }
    }

    private void HandleWalkState(float distanceToPlayer)
    {
        animator.SetBool("IsWalking", true);

        if (distanceToPlayer < attackRange)
        {
            ChangeState(BossState.Attack);
        }
        else if (distanceToPlayer > attackRange * 2)
        {
            ChangeState(BossState.Idle);
        }
    }

    private IEnumerator ExecuteRandomAttack()
    {
        isAttacking = true;
        animator.SetBool("IsWalking", false);

        int randomAttack = Random.Range(0, 3);
        switch (randomAttack)
        {
            case 0:
                animator.SetTrigger("SwordAttack");
                break;
            case 1:
                animator.SetTrigger("MagicFire");
                break;
            case 2:
                animator.SetTrigger("MagicBlade");
                break;
            case 3:
                animator.SetTrigger("MagicLightning");
                break;
        }

        yield return new WaitForSeconds(1.5f);  // Example attack duration

        isAttacking = false;
        currentState = isAngry ? BossState.Angry : BossState.Idle;
    }

    private void ChangeState(BossState newState)
    {
        currentState = newState;
    }

    private void HandleDefeatedState()
    {
        animator.SetTrigger("Defeated");
        Destroy(gameObject, 2f);
    }

    public void TakeDamage(int damage)
    {
        if (currentState == BossState.Defeated) return;

        // Adjust health here...
        //if (currentHealth <= maxHealth * 0.5f && !isAngry)
        //{
        //    isAngry = true;
        //    ChangeState(BossState.Angry);
        //}
        //else if (currentHealth <= 0)
        //{
        //    ChangeState(BossState.Defeated);
        //}
    }
}


