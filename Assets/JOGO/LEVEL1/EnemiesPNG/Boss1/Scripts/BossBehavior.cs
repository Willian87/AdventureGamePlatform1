using UnityEngine;
using System.Collections;

public class BossBehavior : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 1000;
    [SerializeField] private float phase2Threshold = 0.7f; // 70% health
    [SerializeField] private float phase3Threshold = 0.3f; // 30% health

    [Header("Attack Timings")]
    [SerializeField] private float baseTimeBetweenAttacks = 3f;
    [SerializeField] private float warningDuration = 1.5f;
    [SerializeField] private float attackDuration = 1f;

    [Header("Melee Attack")]
    [SerializeField] private float meleeRange = 2f;
    [SerializeField] private int meleeDamage = 25;
    [SerializeField] private Transform meleeHitbox;

    [Header("Fire Attack")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireballSpeed = 8f;
    [SerializeField] private int fireballDamage = 20;

    [Header("Lightning Attack")]
    [SerializeField] private GameObject lightningPrefab;
    [SerializeField] private GameObject warningEffectPrefab;
    [SerializeField] private int lightningDamage = 40;
    [SerializeField] private float lightningWidth = 3f;

    [Header("Facing Settings")]
    [SerializeField] private bool faceRight = true; // Initial facing direction
    [SerializeField] private bool lockFacingDuringAttack = true; // Option to lock facing while attacking

    private enum BossPhase { Phase1, Phase2, Phase3 }
    private enum AttackType { Melee, Fire, Lightning }

    private BossPhase currentPhase;
    private int currentHealth;
    private bool isAttacking;
    private Animator animator;
    private Transform player;
    private SpriteRenderer spriteRenderer;
    private bool isFacingRight;

    private void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentPhase = BossPhase.Phase1;
        isFacingRight = faceRight;
        StartCoroutine(AttackRoutine());
    }

    private void Update()
    {
        UpdateFacing();
    }

    private void UpdateFacing()
    {
        // Skip facing update if we're attacking and facing is locked
        if (isAttacking && lockFacingDuringAttack) return;

        // Determine if we need to flip based on player position
        bool shouldFaceRight = player.position.x > transform.position.x;

        // Only flip if necessary
        if (shouldFaceRight != isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        // Flip the facing flag
        isFacingRight = !isFacingRight;

        // Flip the sprite
        spriteRenderer.flipX = !isFacingRight;

        // If your boss uses Transform.localScale for flipping instead of spriteRenderer.flipX,
        // uncomment and use this alternative method:
        /*
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        */

        // Adjust attack point positions if necessary
        if (firePoint != null)
        {
            Vector3 firePointPos = firePoint.localPosition;
            firePointPos.x = Mathf.Abs(firePointPos.x) * (isFacingRight ? 1 : -1);
            firePoint.localPosition = firePointPos;
        }

        if (meleeHitbox != null)
        {
            Vector3 meleePos = meleeHitbox.localPosition;
            meleePos.x = Mathf.Abs(meleePos.x) * (isFacingRight ? 1 : -1);
            meleeHitbox.localPosition = meleePos;
        }
    }

    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            if (!isAttacking)
            {
                // Choose attack based on current phase
                AttackType attackType = ChooseAttackByPhase();
                yield return StartCoroutine(ExecuteAttack(attackType));

                // Wait between attacks
                float waitTime = CalculateWaitTime();
                yield return new WaitForSeconds(waitTime);
            }
            yield return null;
        }
    }

    private AttackType ChooseAttackByPhase()
    {
        switch (currentPhase)
        {
            case BossPhase.Phase1:
                // Phase 1: Only melee and fire attacks
                return Random.value < 0.7f ? AttackType.Melee : AttackType.Fire;

            case BossPhase.Phase2:
                // Phase 2: All attacks, but lightning is rare
                float rand = Random.value;
                if (rand < 0.4f) return AttackType.Melee;
                else if (rand < 0.8f) return AttackType.Fire;
                else return AttackType.Lightning;

            case BossPhase.Phase3:
                // Phase 3: All attacks with equal probability
                return (AttackType)Random.Range(0, 3);

            default:
                return AttackType.Melee;
        }
    }

    private float CalculateWaitTime()
    {
        // Reduce time between attacks based on phase
        float phaseMultiplier = currentPhase switch
        {
            BossPhase.Phase1 => 1f,
            BossPhase.Phase2 => 0.8f,
            BossPhase.Phase3 => 0.6f,
            _ => 1f
        };

        return baseTimeBetweenAttacks * phaseMultiplier;
    }

    private IEnumerator ExecuteAttack(AttackType attackType)
    {
        isAttacking = true;

        switch (attackType)
        {
            case AttackType.Melee:
                yield return StartCoroutine(MeleeAttack());
                break;

            case AttackType.Fire:
                yield return StartCoroutine(FireAttack());
                break;

            case AttackType.Lightning:
                yield return StartCoroutine(LightningAttack());
                break;
        }

        isAttacking = false;
    }

    private IEnumerator MeleeAttack()
    {
        

        yield return new WaitForSeconds(attackDuration);

        // Check for hits
        Collider2D[] hits = Physics2D.OverlapCircleAll(meleeHitbox.position, meleeRange);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                // Play melee attack animation
                animator.SetTrigger("MeleeAttack");
                hit.GetComponent<PlayerCombat>()?.TakingDamage(meleeDamage);
            }
        }
    }

    private IEnumerator FireAttack()
    {
        // Play fire attack animation
        animator.SetTrigger("FireAttack");

        yield return new WaitForSeconds(attackDuration * 0.5f);

        // Create and launch fireball
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        Vector2 direction = (player.position - firePoint.position).normalized;
        fireball.GetComponent<Rigidbody2D>().velocity = direction * fireballSpeed;
        fireball.GetComponent<Bullet>()?.SetDamage(fireballDamage);

        yield return new WaitForSeconds(attackDuration * 0.5f);
    }

    private IEnumerator LightningAttack()
    {
        // Play warning animation
        animator.SetTrigger("LightningWarning");
        animator.SetTrigger("Anger");

        // Create warning effect at player's position
        Vector2 targetPos = player.position;
        GameObject warning = Instantiate(warningEffectPrefab, targetPos, Quaternion.identity);

        // Give player time to react
        yield return new WaitForSeconds(warningDuration);
        Destroy(warning);

        // Play lightning attack animation
        animator.SetTrigger("LightningAttack");

        // Create lightning strike
        GameObject lightning = Instantiate(lightningPrefab, targetPos, Quaternion.identity);

        // Check for hits
        Collider2D[] hits = Physics2D.OverlapBoxAll(targetPos, new Vector2(lightningWidth, 20f), 0f);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<PlayerCombat>()?.TakingDamage(lightningDamage);
            }
        }

        yield return new WaitForSeconds(attackDuration);
        Destroy(lightning);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        float healthPercentage = (float)currentHealth / maxHealth;

        // Check for phase transitions
        if (healthPercentage <= phase3Threshold && currentPhase != BossPhase.Phase3)
        {
            TransitionToPhase(BossPhase.Phase3);
        }
        else if (healthPercentage <= phase2Threshold && currentPhase != BossPhase.Phase2)
        {
            TransitionToPhase(BossPhase.Phase2);
        }

        // Play hit animation
        animator.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void TransitionToPhase(BossPhase newPhase)
    {
        currentPhase = newPhase;
        animator.SetTrigger("PhaseTransition");

        // You can add phase transition effects here
        switch (newPhase)
        {
            case BossPhase.Phase2:
                animator.SetBool("IsPhase2", true);
                break;
            case BossPhase.Phase3:
                animator.SetBool("IsPhase3", true);
                break;
        }
    }

    private void Die()
    {
        // Stop all attacks
        StopAllCoroutines();

        // Play death animation
        animator.SetTrigger("Die");

        // Disable components
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;

        // You can add death effects, drop items, etc. here
        Destroy(gameObject, 2f);
    }

    // Visualize attack ranges in editor
    private void OnDrawGizmosSelected()
    {
        if (meleeHitbox != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(meleeHitbox.position, meleeRange);
        }
    }
}



//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class BossBehaviorWithRaycast : MonoBehaviour
//{
//    [Header("References")]
//    private Animator anim;
//    private AudioSource audioSource;
//    private Transform player;

//    [Header("Attack Points")]
//    public Transform meleeAttackPoint;
//    public Transform magicFireSpawnPoint;
//    public Transform magicLightningSpawnPoint;

//    [Header("Attack Ranges")]
//    public float meleeRange = 2f;
//    public float magicFireRange = 5f;
//    public float magicLightningRange = 8f;

//    [Header("Prefabs")]
//    public GameObject magicFireProjectile;
//    public GameObject magicLightningProjectile;

//    [Header("Layer Masks")]
//    [SerializeField] private LayerMask playerLayerMask;

//    [Header("Cooldowns")]
//    [SerializeField] private float meleeCooldown = 2f;
//    [SerializeField] private float magicFireCooldown = 3f;
//    [SerializeField] private float magicLightningCooldown = 5f;

//    [Header("Damage Settings")]
//    [SerializeField] private int meleeDamage = 10;

//    private bool canMelee = true;
//    private bool canMagicFire = true;
//    private bool canMagicLightning = true;

//    private SpriteRenderer spriteRenderer;

//    private void Start()
//    {
//        anim = GetComponent<Animator>();
//        audioSource = GetComponent<AudioSource>();
//        player = GameObject.FindGameObjectWithTag("Player").transform;
//        spriteRenderer = GetComponent<SpriteRenderer>();
//    }

//    private void Update()
//    {
//        if (player == null) return;

//        // Check detection and trigger attacks
//        if (DetectPlayer(meleeRange) && canMelee)
//        {
//            MeleeAttack();
//        }
//        else if (DetectPlayer(magicFireRange) && canMagicFire)
//        {
//            MagicFireAttack();
//        }
//        else if (DetectPlayer(magicLightningRange) && canMagicLightning)
//        {
//            MagicLightningAttack();
//        }
//    }

//    /// <summary>
//    /// Detects the player using raycasting.
//    /// </summary>
//    private bool DetectPlayer(float range)
//    {
//        Vector2 direction = spriteRenderer.flipX ? Vector2.left : Vector2.right;
//        RaycastHit2D hit = Physics2D.Raycast(meleeAttackPoint.position, direction, range, playerLayerMask);

//        Debug.DrawRay(meleeAttackPoint.position, direction * range, Color.red);

//        return hit.collider != null && hit.collider.CompareTag("Player");
//    }

//    private void MeleeAttack()
//    {
//        // Trigger melee animation
//        anim.SetTrigger("MeleeAttack");
//        Debug.Log("Performing Melee Attack!");

//        // Damage the player
//        Vector2 direction = spriteRenderer.flipX ? Vector2.left : Vector2.right;
//        RaycastHit2D hit = Physics2D.Raycast(meleeAttackPoint.position, direction, meleeRange, playerLayerMask);
//        if (hit.collider != null && hit.collider.CompareTag("Player"))
//        {
//            hit.collider.GetComponent<PlayerCombat>().TakingDamage(meleeDamage);
//        }

//        canMelee = false;
//        Invoke(nameof(ResetMeleeCooldown), meleeCooldown);
//    }

//    private void MagicFireAttack()
//    {
//        // Trigger magic fire animation
//        anim.SetTrigger("MagicFire");
//        Debug.Log("Shooting Magic Fire!");

//        // Shoot magic fire projectile
//        ShootProjectile(magicFireProjectile, magicFireSpawnPoint);

//        canMagicFire = false;
//        Invoke(nameof(ResetMagicFireCooldown), magicFireCooldown);
//    }

//    private void MagicLightningAttack()
//    {
//        // Trigger magic lightning animation
//        anim.SetTrigger("MagicLightning");
//        Debug.Log("Shooting Magic Lightning!");

//        // Shoot magic lightning projectile
//        ShootProjectile(magicLightningProjectile, magicLightningSpawnPoint);

//        canMagicLightning = false;
//        Invoke(nameof(ResetMagicLightningCooldown), magicLightningCooldown);
//    }

//    private void ShootProjectile(GameObject projectilePrefab, Transform spawnPoint)
//    {
//        if (projectilePrefab == null || spawnPoint == null)
//        {
//            Debug.LogError("Projectile or spawn point not assigned!");
//            return;
//        }

//        // Instantiate projectile and set its direction
//        GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
//        Vector2 direction = (player.position - spawnPoint.position).normalized;
//        projectile.GetComponent<Rigidbody2D>().velocity = direction * projectile.GetComponent<Bullet>().speed;
//    }

//    private void ResetMeleeCooldown()
//    {
//        canMelee = true;
//    }

//    private void ResetMagicFireCooldown()
//    {
//        canMagicFire = true;
//    }

//    private void ResetMagicLightningCooldown()
//    {
//        canMagicLightning = true;
//    }

//    private void OnDrawGizmosSelected()
//    {
//        if (meleeAttackPoint != null)
//        {
//            Gizmos.color = Color.red;
//            Gizmos.DrawLine(meleeAttackPoint.position, meleeAttackPoint.position + transform.right * meleeRange);
//        }

//        if (magicFireSpawnPoint != null)
//        {
//            Gizmos.color = Color.blue;
//            Gizmos.DrawLine(transform.position, transform.position + transform.right * magicFireRange);
//        }

//        if (magicLightningSpawnPoint != null)
//        {
//            Gizmos.color = Color.yellow;
//            Gizmos.DrawLine(transform.position, transform.position + transform.right * magicLightningRange);
//        }
//    }
//}




