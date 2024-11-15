using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehaviorWithRaycast : MonoBehaviour
{
    [Header("References")]
    private Animator anim;
    private AudioSource audioSource;
    private Transform player;

    [Header("Attack Points")]
    public Transform meleeAttackPoint;
    public Transform magicFireSpawnPoint;
    public Transform magicLightningSpawnPoint;

    [Header("Attack Ranges")]
    public float meleeRange = 2f;
    public float magicFireRange = 5f;
    public float magicLightningRange = 8f;

    [Header("Prefabs")]
    public GameObject magicFireProjectile;
    public GameObject magicLightningProjectile;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask playerLayerMask;

    [Header("Cooldowns")]
    [SerializeField] private float meleeCooldown = 2f;
    [SerializeField] private float magicFireCooldown = 3f;
    [SerializeField] private float magicLightningCooldown = 5f;

    [Header("Damage Settings")]
    [SerializeField] private int meleeDamage = 10;

    private bool canMelee = true;
    private bool canMagicFire = true;
    private bool canMagicLightning = true;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (player == null) return;

        // Check detection and trigger attacks
        if (DetectPlayer(meleeRange) && canMelee)
        {
            MeleeAttack();
        }
        else if (DetectPlayer(magicFireRange) && canMagicFire)
        {
            MagicFireAttack();
        }
        else if (DetectPlayer(magicLightningRange) && canMagicLightning)
        {
            MagicLightningAttack();
        }
    }

    /// <summary>
    /// Detects the player using raycasting.
    /// </summary>
    private bool DetectPlayer(float range)
    {
        Vector2 direction = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        RaycastHit2D hit = Physics2D.Raycast(meleeAttackPoint.position, direction, range, playerLayerMask);

        Debug.DrawRay(meleeAttackPoint.position, direction * range, Color.red);

        return hit.collider != null && hit.collider.CompareTag("Player");
    }

    private void MeleeAttack()
    {
        // Trigger melee animation
        anim.SetTrigger("MeleeAttack");
        Debug.Log("Performing Melee Attack!");

        // Damage the player
        Vector2 direction = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        RaycastHit2D hit = Physics2D.Raycast(meleeAttackPoint.position, direction, meleeRange, playerLayerMask);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            hit.collider.GetComponent<PlayerCombat>().TakingDamage(meleeDamage);
        }

        canMelee = false;
        Invoke(nameof(ResetMeleeCooldown), meleeCooldown);
    }

    private void MagicFireAttack()
    {
        // Trigger magic fire animation
        anim.SetTrigger("MagicFire");
        Debug.Log("Shooting Magic Fire!");

        // Shoot magic fire projectile
        ShootProjectile(magicFireProjectile, magicFireSpawnPoint);

        canMagicFire = false;
        Invoke(nameof(ResetMagicFireCooldown), magicFireCooldown);
    }

    private void MagicLightningAttack()
    {
        // Trigger magic lightning animation
        anim.SetTrigger("MagicLightning");
        Debug.Log("Shooting Magic Lightning!");

        // Shoot magic lightning projectile
        ShootProjectile(magicLightningProjectile, magicLightningSpawnPoint);

        canMagicLightning = false;
        Invoke(nameof(ResetMagicLightningCooldown), magicLightningCooldown);
    }

    private void ShootProjectile(GameObject projectilePrefab, Transform spawnPoint)
    {
        if (projectilePrefab == null || spawnPoint == null)
        {
            Debug.LogError("Projectile or spawn point not assigned!");
            return;
        }

        // Instantiate projectile and set its direction
        GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        Vector2 direction = (player.position - spawnPoint.position).normalized;
        projectile.GetComponent<Rigidbody2D>().velocity = direction * projectile.GetComponent<Bullet>().speed;
    }

    private void ResetMeleeCooldown()
    {
        canMelee = true;
    }

    private void ResetMagicFireCooldown()
    {
        canMagicFire = true;
    }

    private void ResetMagicLightningCooldown()
    {
        canMagicLightning = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (meleeAttackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(meleeAttackPoint.position, meleeAttackPoint.position + transform.right * meleeRange);
        }

        if (magicFireSpawnPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + transform.right * magicFireRange);
        }

        if (magicLightningSpawnPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + transform.right * magicLightningRange);
        }
    }
}



//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class BossBehavior : MonoBehaviour
//{
//    [Header("References")]
//    private Animator anim;
//    private AudioSource audioSource;
//    private Transform player;

//    [Header("Attack Points & Ranges")]
//    public Transform meleeAttackPoint;
//    public Transform magicFirePoint;
//    public Transform magicLightningPoint;
//    public float meleeRange = 2f;
//    public float magicFireRange = 5f;
//    public float magicLightningRange = 8f;

//    [Header("Layer Masks")]
//    [SerializeField] private LayerMask playerLayerMask;

//    [Header("Cooldowns")]
//    [SerializeField] private float meleeCooldown = 2f;
//    [SerializeField] private float magicFireCooldown = 5f;
//    [SerializeField] private float magicLightningCooldown = 8f;

//    [Header("Damage Settings")]
//    [SerializeField] private int meleeDamage = 10;
//    [SerializeField] private int fireDamage = 20;
//    [SerializeField] private int lightningDamage = 30;

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

//        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

//        if (distanceToPlayer <= meleeRange && canMelee)
//        {
//            MeleeAttack();
//        }
//        else if (distanceToPlayer <= magicFireRange && canMagicFire)
//        {
//            MagicFireAttack();
//        }
//        else if (distanceToPlayer <= magicLightningRange && canMagicLightning)
//        {
//            MagicLightningAttack();
//        }
//    }

//    private void MeleeAttack()
//    {
//        // Trigger melee attack animation
//        anim.SetTrigger("MeleeAttack");
//        Debug.Log("Performing Melee Attack!");

//        // Perform melee damage
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
//        Debug.Log("Casting Magic Fire!");

//        // Perform magic fire damage
//        Vector2 direction = spriteRenderer.flipX ? Vector2.left : Vector2.right;
//        RaycastHit2D hit = Physics2D.Raycast(magicFirePoint.position, direction, magicFireRange, playerLayerMask);

//        if (hit.collider != null && hit.collider.CompareTag("Player"))
//        {
//            hit.collider.GetComponent<PlayerCombat>().TakingDamage(fireDamage);
//        }

//        canMagicFire = false;
//        Invoke(nameof(ResetMagicFireCooldown), magicFireCooldown);
//    }

//    private void MagicLightningAttack()
//    {
//        // Trigger magic lightning animation
//        anim.SetTrigger("MagicLightningAttack");
//        Debug.Log("Casting Magic Lightning!");

//        // Perform magic lightning damage
//        Collider2D hit = Physics2D.OverlapCircle(magicLightningPoint.position, magicLightningRange, playerLayerMask);

//        if (hit != null && hit.CompareTag("Player"))
//        {
//            hit.GetComponent<PlayerCombat>().TakingDamage(lightningDamage);
//        }

//        canMagicLightning = false;
//        Invoke(nameof(ResetMagicLightningCooldown), magicLightningCooldown);
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
//            Gizmos.DrawWireSphere(meleeAttackPoint.position, meleeRange);
//        }

//        if (magicFirePoint != null)
//        {
//            Gizmos.color = Color.blue;
//            Gizmos.DrawWireSphere(magicFirePoint.position, magicFireRange);
//        }

//        if (magicLightningPoint != null)
//        {
//            Gizmos.color = Color.yellow;
//            Gizmos.DrawRay(magicLightningPoint.position, Vector2.left * magicLightningRange);
//        }
//    }
//}
