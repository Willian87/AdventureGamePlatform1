using System.Collections;
using UnityEngine;

public class BossPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints; // Patrol points for boss
    [SerializeField] private float patrolSpeed = 2f;  // Speed of patrolling
    [SerializeField] private float idleTimeAtPatrolPoint = 2f; // Idle time at each patrol point
    private int currentPatrolIndex;                   // Current patrol point index

    [Header("Attack Settings")]
    [SerializeField] private float detectionRange = 1.5f;       // Range for detecting player
    [SerializeField] private LayerMask playerLayerMask;         // Layer mask for player detection
    [SerializeField] private float attackRange = 1f;            // Range for attacking player
    [SerializeField] private float attackCooldown = 2f;         // Cooldown time between attacks
    [SerializeField] private Transform attackRangePos;
    private bool canAttack = true;                              // To control attack cooldown

    private bool isWalking = false;                             // Patrol state
    private bool isIdleAtPoint = false;                         // Idle state at patrol point
    private Rigidbody2D rb;                                     // Reference to Rigidbody2D
    private Animator animator;                                  // Reference to Animator
    private SpriteRenderer spriteRenderer;                      // Reference to SpriteRenderer

    private void Start()
    {
        if (patrolPoints.Length < 2)
        {
            Debug.LogError("Not enough patrol points assigned. Please assign at least two patrol points.");
            enabled = false;
            return;
        }

        currentPatrolIndex = 0;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (PlayerInRange(attackRange))
        {
            StopAndAttack();
        }
        else if (!isIdleAtPoint)
        {
            Patrol();
        }

        animator.SetBool("isWalking", isWalking);
    }

    private void Patrol()
    {
        Transform targetPatrolPoint = patrolPoints[currentPatrolIndex];
        MoveTowards(targetPatrolPoint.position, patrolSpeed);

        // Check if the boss has reached the patrol point
        if (Vector2.Distance(transform.position, targetPatrolPoint.position) < 0.2f)
        {
            isWalking = false;
            isIdleAtPoint = true;
            animator.SetBool("isWalking", false);

            FlipTowardsNextPoint();
            StartCoroutine(IdleAtPatrolPoint());
        }
    }

    private void MoveTowards(Vector2 targetPosition, float speed)
    {
        isWalking = true;
        Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, speed * Time.deltaTime);
        rb.MovePosition(newPosition);
    }

    private void FlipTowardsNextPoint()
    {
        // Flip the boss to face the next patrol point
        Transform nextPatrolPoint = patrolPoints[(currentPatrolIndex + 1) % patrolPoints.Length];
        spriteRenderer.flipX = (nextPatrolPoint.position.x < transform.position.x);
    }

    private IEnumerator IdleAtPatrolPoint()
    {
        animator.SetTrigger("Idle");
        yield return new WaitForSeconds(idleTimeAtPatrolPoint);

        isIdleAtPoint = false;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    private bool PlayerInRange(float range)
    {
        Collider2D playerInRange = Physics2D.OverlapCircle(transform.position, range, playerLayerMask);
        return playerInRange != null;
    }

    private void StopAndAttack()
    {
        if (canAttack)
        {
            animator.SetTrigger("Attack");
            canAttack = false;
            Invoke("ResetAttackCooldown", attackCooldown);
        }
        isWalking = false;
    }

    private void ResetAttackCooldown()
    {
        canAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}






//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class BossPatrol : MonoBehaviour
//{
//    [Header("Patrol Settings")]
//    [SerializeField] private Transform[] patrolPoints; // Points for patrolling
//    [SerializeField] private float patrolSpeed = 2f;  // Speed of patrolling
//    private int currentPatrolIndex;                   // Current target patrol point

//    [Header("Chase Limits Settings")]
//    [SerializeField] private Transform chaseLimitPointA;
//    [SerializeField] private Transform chaseLimitPointB;
//    private int currentLimitPoint;

//    [Header("Detection Settings")]
//    public float detectionRange = 1.5f;       // Range to detect the player
//    [SerializeField] private LayerMask playerLayerMask;       // Layer mask for detecting the player
//    float originalDetectionRange;

//    [Header("Attack Settings")]
//    [SerializeField] private float attackRange = 1f;          // Range to stop and attack the player
//    [SerializeField] private float chaseSpeed = 3.5f;         // Speed when chasing the player

//    private Transform player;
//    private bool isChasingPlayer = false;
//    private bool isWalking = false;
//    private Rigidbody2D rb;
//    private Animator animator;
//    private SpriteRenderer spriteRenderer;

//    bool isFacingRight = true;

//    private void Start()
//    {
//        if (patrolPoints.Length < 2)
//        {
//            Debug.LogError("Not enough patrol points assigned. Please assign at least two patrol points.");
//            enabled = false; // Disable the script if there aren't enough patrol points
//            return;
//        }

//        currentPatrolIndex = 0;
//        player = GameObject.FindGameObjectWithTag("Player").transform;
//        rb = GetComponent<Rigidbody2D>();
//        animator = GetComponent<Animator>();
//        spriteRenderer = GetComponent<SpriteRenderer>();
//        originalDetectionRange = detectionRange;
//    }

//    private void Update()
//    {
//        if (isChasingPlayer)
//        {
//            ChasePlayer();
//        }
//        else
//        {
//            Patrol();
//            CheckForPlayer();
//        }

//        animator.SetBool("isWalking", isWalking);
//    }

//    private void Patrol()
//    {
//        Transform targetPatrolPoint = patrolPoints[currentPatrolIndex];
//        MoveTowards(targetPatrolPoint.position, patrolSpeed);

//        if (Vector2.Distance(transform.position, targetPatrolPoint.position) < 0.2f)
//        {
//            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
//            detectionRange = 1.5f;
//        }
//    }

//    private void CheckForPlayer()
//    {
//        Vector2 directionToPlayer = (player.position - transform.position).normalized;
//        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, detectionRange, playerLayerMask);
//        Debug.DrawRay(transform.position, directionToPlayer * detectionRange, Color.red);

//        if (hit.collider != null && hit.collider.CompareTag("Player") && !IsBeyondChasingLimits())
//        {
//            isChasingPlayer = true;
//        }
//    }

//    private void ChasePlayer()
//    {
//        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

//        if (IsBeyondChasingLimits())
//        {
//            isChasingPlayer = false;
//            detectionRange = 0; // Set detection range to zero when beyond the chase limits
//        }
//        else if (distanceToPlayer > detectionRange)
//        {
//            detectionRange = originalDetectionRange; // Restore the detection range when within chase limits
//            isChasingPlayer = false;
//        }

//        if (distanceToPlayer > detectionRange /*|| IsBeyondChasingLimits()*/)
//        {
//            isChasingPlayer = false;
//        }
//        else if (distanceToPlayer > attackRange)
//        {
//            Vector2 direction = (player.position - transform.position).normalized;
//            Vector2 targetPosition = new Vector2(player.position.x, rb.position.y); // Keep the enemy on the ground
//            MoveTowards(targetPosition, chaseSpeed);
//        }
//        else
//        {
//            // Stop and prepare to attack the player
//            isWalking = false;
//            AttackPlayer();
//        }
//    }


//    bool IsBeyondChasingLimits()
//    {
//        float enemyPositionX = transform.position.x;
//        float limitPointAX = chaseLimitPointA.position.x;
//        float limitPointBX = chaseLimitPointB.position.x;

//        bool isBeyondLimits = enemyPositionX < Mathf.Min(limitPointAX, limitPointBX) || enemyPositionX > Mathf.Max(limitPointAX, limitPointBX);
//        //if (isBeyondLimits)
//        //{
//        //    detectionRange = 0; // Set detection range to zero when beyond the chase limits
//        //}
//        //else
//        //{
//        //    detectionRange = 1.5f;
//        //}
//        return isBeyondLimits;
//    }
//    private void AttackPlayer()
//    {
//        // Implement attack logic here
//        Debug.Log("Attacking Player");
//    }

//    private void MoveTowards(Vector2 targetPosition, float speed)
//    {
//        isWalking = true;
//        Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, speed * Time.deltaTime);
//        rb.MovePosition(newPosition);

//        //if (targetPosition.x > transform.position.x) // Moving to the right
//        //{
//        //    transform.rotation = Quaternion.Euler(0, 0, 0); // Face right (no Y rotation)

//        //}
//        //else if (targetPosition.x < transform.position.x) // Moving to the left
//        //{
//        //    transform.rotation = Quaternion.Euler(0, 180, 0); // Rotate 180 degrees to face left
//        //}

//        //Flip the enemy based on the direction
//        if (targetPosition.x > transform.position.x)
//        {

//            spriteRenderer.flipX = true; // Face right
//            targetPosition = Vector2.right;
//        }
//        else if (targetPosition.x < transform.position.x)
//        {

//            spriteRenderer.flipX = false; // Face left
//            targetPosition = Vector2.left;

//        }
//    }


//}

