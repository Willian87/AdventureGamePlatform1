using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    Animator anim;
    AudioSource audioSource;

    public Transform attackPoint;
    
    [SerializeField] private LayerMask playerLayerMask;

    [SerializeField] private PlayerCombat player;

    [Header("Advanced Settings")]
    [SerializeField] private float attackCooldown = 2f;
    private bool canAttack = true;
    [SerializeField] private float attackRange;
    [SerializeField] private int baseDamage = 5;
    [SerializeField] private int criticalDamage = 20;
    [SerializeField] private float criticalChance = 0.2f;

    public delegate void EnemyEvent();
    public static event EnemyEvent OnEnemyAttack;
    private float nextAttackTime;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        player = FindObjectOfType<PlayerCombat>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        nextAttackTime += Time.deltaTime;
        if (nextAttackTime >= 2)
        {
            EAttack();
            nextAttackTime = 0f;
        }
    }

    public void EAttack()
    {
        Vector2 direction = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        RaycastHit2D attackRangeRayCast = Physics2D.Raycast(attackPoint.position, direction, attackRange, playerLayerMask);

        if (attackRangeRayCast.collider != null && canAttack)
        {
            anim.SetBool("isAttacking", true);
            AttackPlayer(direction);
            canAttack = false;
            Invoke("ResetAttackCooldown", attackCooldown);

            // Notify subscribers that the enemy is attacking
            OnEnemyAttack?.Invoke();
        }
        else
        {
            anim.SetBool("isAttacking", false);
        }
    }

    private void AttackPlayer(Vector2 direction)
    {
        RaycastHit2D attackRangeRayCast = Physics2D.Raycast(attackPoint.position, direction, attackRange, playerLayerMask);

        if (attackRangeRayCast.collider != null && canAttack)
        {
            // Apply damage to the player with a chance for critical damage
            int damageToDeal = (Random.value < criticalChance) ? criticalDamage : baseDamage;
            player.TakingDamage(damageToDeal);

            // Play attack sound
            if (audioSource != null)
            {
                audioSource.Play();
            }
        }
    }

    private void ResetAttackCooldown()
    {
        canAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 direction = spriteRenderer != null && spriteRenderer.flipX ? Vector2.left : Vector2.right;
        Gizmos.DrawRay(attackPoint.position, direction * attackRange);
    }
}


//using System.Collections;
//using UnityEngine;

//public class Boss : MonoBehaviour
//{
//    [Header("Patrol Settings")]
//    [SerializeField] private Transform[] patrolPoints;
//    [SerializeField] private float walkSpeed = 2f;
//    [SerializeField] private float runSpeed = 4f;
//    private int currentPatrolIndex;

//    [Header("Chase Settings")]
//    [SerializeField] private float chaseSpeed = 3.5f;
//    [SerializeField] private float detectionRange = 3f;
//    [SerializeField] private LayerMask playerLayerMask;
//    private Transform player;
//    private bool isChasingPlayer = false;

//    [Header("Attack Settings")]
//    [SerializeField] private Transform attackPoint;
//    [SerializeField] private float attackRange = 1f;
//    [SerializeField] private LayerMask playerLayer;
//    [SerializeField] private int baseDamage = 5;
//    [SerializeField] private int criticalDamage = 20;
//    [SerializeField] private float criticalChance = 0.2f;
//    [SerializeField] private float attackCooldown = 2f;
//    private bool canAttack = true;

//    [Header("Health Settings")]
//    [SerializeField] private int maxHealth = 100;
//    private int currentHealth;

//    private Rigidbody2D rb;
//    private Animator animator;
//    private bool isWalking, isRunning, isAttacking, isHurt, isDead;
//    private SpriteRenderer spriteRenderer;
//    private float nextAttackTime;

//    private void Start()
//    {
//        if (patrolPoints.Length < 2)
//        {
//            Debug.LogError("Not enough patrol points assigned. Please assign at least two patrol points.");
//            enabled = false;
//            return;
//        }

//        currentPatrolIndex = 0;
//        player = GameObject.FindGameObjectWithTag("Player").transform;
//        rb = GetComponent<Rigidbody2D>();
//        animator = GetComponent<Animator>();
//        spriteRenderer = GetComponent<SpriteRenderer>();
//        currentHealth = maxHealth;
//    }

//    private void Update()
//    {
//        if (isDead) return;

//        if (isChasingPlayer)
//        {
//            ChasePlayer();
//        }
//        else
//        {
//            Patrol();
//            CheckForPlayer();
//        }

//        UpdateAnimator();
//    }

//    private void Patrol()
//    {
//        Transform targetPatrolPoint = patrolPoints[currentPatrolIndex];
//        MoveTowards(targetPatrolPoint.position, walkSpeed);

//        if (Vector2.Distance(transform.position, targetPatrolPoint.position) < 0.2f)
//        {
//            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
//        }
//    }

//    private void CheckForPlayer()
//    {
//        Vector2 directionToPlayer = (player.position - transform.position).normalized;
//        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, detectionRange, playerLayerMask);
//        if (hit.collider != null && hit.collider.CompareTag("Player"))
//        {
//            isChasingPlayer = true;
//        }
//    }

//    private void ChasePlayer()
//    {
//        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
//        if (distanceToPlayer > detectionRange)
//        {
//            isChasingPlayer = false;
//            return;
//        }

//        if (distanceToPlayer > attackRange)
//        {
//            MoveTowards(player.position, runSpeed);
//            isRunning = true;
//        }
//        else
//        {
//            if (canAttack)
//            {
//                StartCoroutine(PerformRandomAttack());
//            }
//        }
//    }

//    private void MoveTowards(Vector2 targetPosition, float speed)
//    {
//        Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, speed * Time.deltaTime);
//        rb.MovePosition(newPosition);
//        isWalking = speed == walkSpeed;

//        if (targetPosition.x > transform.position.x)
//        {
//            transform.rotation = Quaternion.Euler(0, 0, 0); // Face right
//        }
//        else if (targetPosition.x < transform.position.x)
//        {
//            transform.rotation = Quaternion.Euler(0, 180, 0); // Face left
//        }
//    }

//    private IEnumerator PerformRandomAttack()
//    {
//        // Stomp to signal an attack
//        animator.SetTrigger("Stomp");
//        yield return new WaitForSeconds(0.5f); // Add a delay before attack to allow stomp animation to play

//        // Perform a random attack
//        int attackType = Random.Range(0, 3); // Choose random attack: 0 = Sword, 1 = Magic Fire, 2 = Magic Lightning, 3 = Magic Blade

//        switch (attackType)
//        {
//            case 0:
//                SwordAttack();
//                break;
//            case 1:
//                MagicFireAttack();
//                break;
//            case 2:
//                MagicLightningAttack();
//                break;
//            case 3:
//                MagicBladeAttack();
//                break;
//        }

//        canAttack = false;
//        Invoke(nameof(ResetAttackCooldown), attackCooldown);
//    }

//    private void SwordAttack()
//    {
//        isAttacking = true;
//        animator.SetTrigger("SwordAttack");
//        ApplyDamage();
//    }

//    private void MagicFireAttack()
//    {
//        isAttacking = true;
//        animator.SetTrigger("MagicFireAttack");
//        // Add fire effect and logic
//    }

//    private void MagicLightningAttack()
//    {
//        isAttacking = true;
//        animator.SetTrigger("MagicLightningAttack");
//        // Add lightning effect and logic
//    }

//    private void MagicBladeAttack()
//    {
//        isAttacking = true;
//        animator.SetTrigger("MagicBladeAttack");
//        // Add blade effect and logic
//    }

//    private void ApplyDamage()
//    {
//        Vector2 direction = spriteRenderer.flipX ? Vector2.left : Vector2.right;
//        RaycastHit2D attackRangeRayCast = Physics2D.Raycast(attackPoint.position, direction, attackRange, playerLayer);

//        if (attackRangeRayCast.collider != null)
//        {
//            int damageToDeal = (Random.value < criticalChance) ? criticalDamage : baseDamage;
//            PlayerCombat playerCombat = attackRangeRayCast.collider.GetComponent<PlayerCombat>();
//            if (playerCombat != null)
//            {
//                playerCombat.TakingDamage(damageToDeal);
//            }
//        }
//    }

//    private void ResetAttackCooldown()
//    {
//        canAttack = true;
//        isAttacking = false;
//    }

//    public void TakeDamage(int damage)
//    {
//        currentHealth -= damage;
//        if (currentHealth <= 0)
//        {
//            isDead = true;
//            animator.SetTrigger("Die");
//        }
//        else
//        {
//            isHurt = true;
//            animator.SetTrigger("Hurt");
//        }
//    }

//    private void UpdateAnimator()
//    {
//        animator.SetBool("isWalking", isWalking);
//        animator.SetBool("isRunning", isRunning);
//        animator.SetBool("isAttacking", isAttacking);
//        animator.SetBool("isHurt", isHurt);
//        animator.SetBool("isDead", isDead);
//    }

//    private void OnDrawGizmosSelected()
//    {
//        Gizmos.color = Color.red;
//        //Vector2 direction = spriteRenderer.flipX ? Vector2.left : Vector2.right;
//        Gizmos.DrawWireSphere(attackPoint.position,  attackRange);
//    }
//}

