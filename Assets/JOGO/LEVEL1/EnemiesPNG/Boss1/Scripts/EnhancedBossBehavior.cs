using UnityEngine;
using System.Collections;

public class EnhancedBossBehavior : MonoBehaviour
{
    [Header("Phase Settings")]
    [SerializeField] private float phase2HealthThreshold = 0.6f; // 60% health
    [SerializeField] private float phase3HealthThreshold = 0.3f; // 30% health
    [SerializeField] private float phase2SpeedMultiplier = 1.3f;
    [SerializeField] private float phase3SpeedMultiplier = 1.5f;

    [Header("General Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float detectRange = 10f;
    [SerializeField] private int maxHealth = 1000;
    [SerializeField] private float stunDuration = 1.5f;

    [Header("Attack Patterns")]
    [SerializeField] private float meleeRange = 2f;
    [SerializeField] private int meleeDamage = 20;
    [SerializeField] private float meleeAttackDuration = 0.5f;
    [SerializeField] private int comboMaxHits = 3;

    [Header("Fire Attack")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float fireballSpeed = 8f;
    [SerializeField] private int fireballDamage = 15;
    [SerializeField] private Transform firePoint;
    [SerializeField] private int burstCount = 3;
    [SerializeField] private float burstDelay = 0.2f;

    [Header("Lightning Attack")]
    [SerializeField] private GameObject lightningWarningPrefab;
    [SerializeField] private GameObject lightningStrikePrefab;
    [SerializeField] private int lightningDamage = 25;
    [SerializeField] private float lightningWarningDuration = 1f;
    [SerializeField] private int lightningCount = 3;

    [Header("Special Attacks")]
    [SerializeField] private float specialAttackCharge = 2f;
    [SerializeField] private float specialCooldown = 15f;
    [SerializeField] private GameObject shockwavePrefab;
    [SerializeField] private float jumpForce = 15f;

    private enum BossPhase { Phase1, Phase2, Phase3 }
    private enum BossState { Idle, Pursuing, Attacking, Stunned, Charging }

    private BossPhase currentPhase = BossPhase.Phase1;
    private BossState currentState = BossState.Idle;
    private Transform player;
    private int currentHealth;
    private bool canAttack = true;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private bool isStunned = false;
    private float currentSpeed;
    private int comboCounter = 0;

    private void Start()
    {
        InitializeComponents();
        currentHealth = maxHealth;
        currentSpeed = moveSpeed;
    }

    private void InitializeComponents()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isStunned || player == null) return;

        UpdatePhase();
        UpdateBehavior();
    }

    private void UpdatePhase()
    {
        float healthPercentage = (float)currentHealth / maxHealth;

        if (healthPercentage <= phase3HealthThreshold && currentPhase != BossPhase.Phase3)
        {
            TransitionToPhase3();
        }
        else if (healthPercentage <= phase2HealthThreshold && currentPhase != BossPhase.Phase2)
        {
            TransitionToPhase2();
        }
    }

    private void UpdateBehavior()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        spriteRenderer.flipX = player.position.x < transform.position.x;

        switch (currentState)
        {
            case BossState.Idle:
                if (distanceToPlayer <= detectRange)
                {
                    currentState = BossState.Pursuing;
                }
                break;

            case BossState.Pursuing:
                PursuePlayer(distanceToPlayer);
                break;

            case BossState.Attacking:
                // Handled by coroutines
                break;

            case BossState.Charging:
                // Handled by coroutines
                break;
        }
    }

    private void PursuePlayer(float distance)
    {
        if (!canAttack) return;

        if (distance <= meleeRange)
        {
            StartCoroutine(PerformMeleeCombo());
        }
        else
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * currentSpeed, rb.velocity.y);

            // Choose ranged attack based on phase and distance
            if (Random.value < 0.7f)
            {
                if (currentPhase == BossPhase.Phase3)
                {
                    StartCoroutine(SuperAttack());
                }
                else if (distance <= detectRange * 0.6f)
                {
                    StartCoroutine(FireballBurst());
                }
                else
                {
                    StartCoroutine(LightningStorm());
                }
            }
        }
    }

    private IEnumerator PerformMeleeCombo()
    {
        currentState = BossState.Attacking;
        canAttack = false;

        for (int i = 0; i < comboMaxHits; i++)
        {
            animator?.SetTrigger("MeleeAttack");
            yield return new WaitForSeconds(meleeAttackDuration);

            // Check if player is still in range and apply damage
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer <= meleeRange)
            {
                player.GetComponent<PlayerCombat>()?.TakingDamage(meleeDamage);
                // Push player back slightly
                Vector2 knockbackDirection = (player.position - transform.position).normalized;
                player.GetComponent<Rigidbody2D>()?.AddForce(knockbackDirection * 5f, ForceMode2D.Impulse);
            }

            yield return new WaitForSeconds(0.2f);
        }

        StartCoroutine(AttackCooldown());
    }

    private IEnumerator FireballBurst()
    {
        currentState = BossState.Attacking;
        canAttack = false;

        for (int i = 0; i < burstCount; i++)
        {
            animator?.SetTrigger("FireballAttack");

            // Calculate spread angle based on phase
            float spreadAngle = currentPhase == BossPhase.Phase1 ? 0f : 15f;

            for (int j = 0; j < (currentPhase == BossPhase.Phase3 ? 3 : 1); j++)
            {
                Vector2 direction = (player.position - firePoint.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) +
                    Random.Range(-spreadAngle, spreadAngle) * Mathf.Deg2Rad;

                Vector2 spreadDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
                Rigidbody2D fireballRb = fireball.GetComponent<Rigidbody2D>();
                fireballRb.velocity = spreadDirection * fireballSpeed;
                //fireball.GetComponent<Bullet>()?.SetDamage(fireballDamage);
            }

            yield return new WaitForSeconds(burstDelay);
        }

        StartCoroutine(AttackCooldown());
    }

    private IEnumerator LightningStorm()
    {
        currentState = BossState.Attacking;
        canAttack = false;
        animator?.SetTrigger("LightningAttack");

        int strikes = currentPhase == BossPhase.Phase3 ? lightningCount * 2 : lightningCount;

        for (int i = 0; i < strikes; i++)
        {
            Vector2 targetPos = player.position;
            if (currentPhase != BossPhase.Phase1)
            {
                // Predict player movement
                Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    targetPos += playerRb.velocity * lightningWarningDuration;
                }
            }

            float randomOffset = Random.Range(-3f, 3f);
            Vector2 strikePosition = new Vector2(targetPos.x + randomOffset, targetPos.y);

            GameObject warning = Instantiate(lightningWarningPrefab, strikePosition, Quaternion.identity);
            yield return new WaitForSeconds(lightningWarningDuration);
            Destroy(warning);

            GameObject lightning = Instantiate(lightningStrikePrefab, strikePosition, Quaternion.identity);

            // Check for player hit
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(strikePosition, 1f);
            foreach (Collider2D hit in hitColliders)
            {
                if (hit.CompareTag("Player"))
                {
                    hit.GetComponent<PlayerCombat>()?.TakingDamage(lightningDamage);
                }
            }

            Destroy(lightning, 0.5f);
            yield return new WaitForSeconds(0.3f);
        }

        StartCoroutine(AttackCooldown());
    }

    private IEnumerator SuperAttack()
    {
        currentState = BossState.Charging;
        canAttack = false;
        animator?.SetTrigger("Charge");

        // Warning effect
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(specialAttackCharge);
        spriteRenderer.color = Color.white;

        // Jump and slam attack
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);

        rb.velocity = Vector2.down * jumpForce;
        yield return new WaitUntil(() => rb.velocity.y == 0);

        // Create shockwave
        Instantiate(shockwavePrefab, transform.position, Quaternion.identity);

        // Screen shake effect
        //Camera.main.GetComponent<CameraShake>()?.Shake(0.5f, 0.5f);

        // Damage nearby players
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 5f);
        foreach (Collider2D hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<PlayerCombat>()?.TakingDamage(fireballDamage * 2);
                Vector2 knockbackDir = (hit.transform.position - transform.position).normalized;
                hit.GetComponent<Rigidbody2D>()?.AddForce(knockbackDir * 10f, ForceMode2D.Impulse);
            }
        }

        yield return new WaitForSeconds(specialCooldown);
        canAttack = true;
    }

    private void TransitionToPhase2()
    {
        currentPhase = BossPhase.Phase2;
        currentSpeed = moveSpeed * phase2SpeedMultiplier;
        StartCoroutine(PhaseTransition());
    }

    private void TransitionToPhase3()
    {
        currentPhase = BossPhase.Phase3;
        currentSpeed = moveSpeed * phase3SpeedMultiplier;
        StartCoroutine(PhaseTransition());
    }

    private IEnumerator PhaseTransition()
    {
        isStunned = true;
        animator?.SetTrigger("PhaseTransition");

        // Visual effect for phase transition
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(1f);
        spriteRenderer.color = Color.white;

        // Knockback nearby player
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 5f);
        foreach (Collider2D hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                Vector2 knockbackDir = (hit.transform.position - transform.position).normalized;
                hit.GetComponent<Rigidbody2D>()?.AddForce(knockbackDir * 15f, ForceMode2D.Impulse);
            }
        }

        isStunned = false;
    }

    private IEnumerator AttackCooldown()
    {
        currentState = BossState.Pursuing;
        yield return new WaitForSeconds(currentPhase == BossPhase.Phase3 ? 1f : 2f);
        canAttack = true;
    }

    public void TakeDamage(int damage)
    {
        if (isStunned) damage = Mathf.RoundToInt(damage * 1.5f); // Take more damage while stunned

        currentHealth -= damage;
        animator?.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            StartCoroutine(DeathSequence());
        }
    }

    private IEnumerator DeathSequence()
    {
        isStunned = true;
        animator?.SetTrigger("Die");

        // Disable collisions
        GetComponent<Collider2D>().enabled = false;

        // Death effects
        for (int i = 0; i < 5; i++)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }

        // Final explosion
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 8f);
        foreach (Collider2D hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                Vector2 knockbackDir = (hit.transform.position - transform.position).normalized;
                hit.GetComponent<Rigidbody2D>()?.AddForce(knockbackDir * 20f, ForceMode2D.Impulse);
            }
        }

        // Drop rewards here

        Destroy(gameObject, 0.5f);
    }
}
