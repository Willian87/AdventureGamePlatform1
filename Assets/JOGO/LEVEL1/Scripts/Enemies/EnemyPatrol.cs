using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints; // Points for patrolling
    [SerializeField] private float patrolSpeed = 2f;  // Speed of patrolling
    private int currentPatrolIndex;                   // Current target patrol point

    [Header("Chase Limits Settings")]
    [SerializeField] private Transform chaseLimitPointA;
    [SerializeField] private Transform chaseLimitPointB;
    private int currentLimitPoint;

    [Header("Detection Settings")]
    public float detectionRange = 1.5f;       // Range to detect the player
    [SerializeField] private LayerMask playerLayerMask;       // Layer mask for detecting the player
    float originalDetectionRange;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 1f;          // Range to stop and attack the player
    [SerializeField] private float chaseSpeed = 3.5f;         // Speed when chasing the player
    
    private Transform player;
    private bool isChasingPlayer = false;
    private bool isWalking = false;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        if (patrolPoints.Length < 2)
        {
            Debug.LogError("Not enough patrol points assigned. Please assign at least two patrol points.");
            enabled = false; // Disable the script if there aren't enough patrol points
            return;
        }

        currentPatrolIndex = 0;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalDetectionRange = detectionRange;
    }

    private void Update()
    {
        if (isChasingPlayer)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
            CheckForPlayer();
        }

        animator.SetBool("isWalking", isWalking);
    }

    private void Patrol()
    {
        Transform targetPatrolPoint = patrolPoints[currentPatrolIndex];
        MoveTowards(targetPatrolPoint.position, patrolSpeed);

        if (Vector2.Distance(transform.position, targetPatrolPoint.position) < 0.2f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            detectionRange = 1.5f;
        }
    }

    private void CheckForPlayer()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, detectionRange, playerLayerMask);
        Debug.DrawRay(transform.position, directionToPlayer * detectionRange, Color.red);

        if (hit.collider != null && hit.collider.CompareTag("Player") && !IsBeyondChasingLimits())
        {
            isChasingPlayer = true;
        }
    }

    private void ChasePlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (IsBeyondChasingLimits())
        {
            isChasingPlayer = false;
            detectionRange = 0; // Set detection range to zero when beyond the chase limits
        }
        else if (distanceToPlayer > detectionRange)
        {
            detectionRange = originalDetectionRange; // Restore the detection range when within chase limits
            isChasingPlayer = false;
        }

        if (distanceToPlayer > detectionRange /*|| IsBeyondChasingLimits()*/)
        {
            isChasingPlayer = false;
        }
        else if (distanceToPlayer > attackRange)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            Vector2 targetPosition = new Vector2(player.position.x, rb.position.y); // Keep the enemy on the ground
            MoveTowards(targetPosition, chaseSpeed);
        }
        else
        {
            // Stop and prepare to attack the player
            isWalking = false;
            AttackPlayer();
        }
    }


    bool IsBeyondChasingLimits()
    {
        float enemyPositionX = transform.position.x;
        float limitPointAX = chaseLimitPointA.position.x;
        float limitPointBX = chaseLimitPointB.position.x;

        bool isBeyondLimits = enemyPositionX < Mathf.Min(limitPointAX, limitPointBX) || enemyPositionX > Mathf.Max(limitPointAX, limitPointBX);
        //if (isBeyondLimits)
        //{
        //    detectionRange = 0; // Set detection range to zero when beyond the chase limits
        //}
        //else
        //{
        //    detectionRange = 1.5f;
        //}
        return isBeyondLimits;
    }
    private void AttackPlayer()
    {
        // Implement attack logic here
        Debug.Log("Attacking Player");
    }

    private void MoveTowards(Vector2 targetPosition, float speed)
    {
        isWalking = true;
        Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, speed * Time.deltaTime);
        rb.MovePosition(newPosition);

        // Flip the enemy based on the direction
        if (targetPosition.x > transform.position.x)
        {
            spriteRenderer.flipX = true; // Face right
            targetPosition = Vector2.right;
        }
        else if (targetPosition.x < transform.position.x)
        {
            spriteRenderer.flipX = false; // Face left
            targetPosition = Vector2.left;

        }
    }

    
}



//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class EnemyPatrol : MonoBehaviour
//{
//    [SerializeField] Transform[] patrolPoint;
//    [SerializeField] float patrolSpeed = 0.5f;
//    [SerializeField] int patrolDestination;
//    Animator anim;

//    private void Start()
//    {
//        anim = GetComponent<Animator>();
//    }
//    // Update is called once per frame
//    void Update()
//    {
//        if(patrolDestination == 0)
//        {
//            transform.position = Vector2.MoveTowards(transform.position, patrolPoint[0].position, patrolSpeed * Time.deltaTime);
//            if (Vector2.Distance(transform.position, patrolPoint[0].position) < .2f)
//            {
//                anim.SetBool("isWalking", true);
//                transform.localScale = new Vector3(1, 1, 1);
//                patrolDestination = 1;

//            }
//        }

//        if (patrolDestination == 1)
//        {
//            transform.position = Vector2.MoveTowards(transform.position, patrolPoint[1].position, patrolSpeed * Time.deltaTime);
//            if (Vector2.Distance(transform.position, patrolPoint[1].position) < .2f)
//            {
//                anim.SetBool("isWalking", true);
//                transform.localScale = new Vector3(-1, 1, 1);
//                patrolDestination = 0;
//            }
//        }
//    }
//}
