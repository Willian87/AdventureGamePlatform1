using UnityEngine;

public class Boss : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public Transform player;
    public Animator animator;
    public GameObject healthBarUI;

    private enum State { Idle, Patrolling, Chasing, Stomping, Attacking, Hurt, Dead }
    private State currentState;

    private void Start()
    {
        currentHealth = maxHealth;
        currentState = State.Idle;
        healthBarUI.SetActive(false);
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                Idle();
                if (IsPlayerInRange())
                {
                    currentState = State.Chasing;
                }
                break;
            case State.Patrolling:
                Patrol();
                if (IsPlayerInRange())
                {
                    currentState = State.Chasing;
                }
                break;
            case State.Chasing:
                ChasePlayer();
                if (IsPlayerInAttackRange())
                {
                    currentState = State.Stomping;
                }
                break;
            case State.Stomping:
                Stomp();
                break;
            case State.Attacking:
                ExecuteAttack();
                break;
            case State.Hurt:
                HandleHurt();
                break;
            case State.Dead:
                HandleDeath();
                break;
        }
    }

    private void Idle()
    {
        // Play Idle animation
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
    }

    private void Patrol()
    {
        // Implement patrol logic here
    }

    private void ChasePlayer()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", true);
        // Logic to chase the player
        transform.position = Vector2.MoveTowards(transform.position, player.position, Time.deltaTime * 3f);
    }

    private void Stomp()
    {
        animator.SetTrigger("Stomp");
        // Signal player by stomping
        currentState = State.Attacking;
    }

    private void ExecuteAttack()
    {
        // Select a random attack
        int attackType = Random.Range(0, 3);
        switch (attackType)
        {
            case 0:
                animator.SetTrigger("SwordAttack");
                break;
            case 1:
                animator.SetTrigger("MagicFire");
                break;
            case 2:
                animator.SetTrigger("MagicLightning");
                break;
            case 3:
                animator.SetTrigger("MagicBlade");
                break;
        }
        currentState = State.Idle;
    }

    private void HandleHurt()
    {
        if (currentHealth <= 0)
        {
            currentState = State.Dead;
        }
    }

    private void HandleDeath()
    {
        animator.SetTrigger("Death");
        healthBarUI.SetActive(false);
        // Add logic for the death state
    }

    private bool IsPlayerInRange()
    {
        return Vector3.Distance(transform.position, player.position) < detectionRange;
    }

    private bool IsPlayerInAttackRange()
    {
        return Vector3.Distance(transform.position, player.position) < attackRange;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            currentState = State.Dead;
        }
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

