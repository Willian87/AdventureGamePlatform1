using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    Animator anim;
    AudioSource audioSource;

    public Transform attackPoint;
    public Transform magicFireAttackPoint;
    public Transform LightningAttackPoint;

    public float attackRange;
    public float magicFireRange;
    public float lightningRange;

    [SerializeField] private LayerMask playerLayerMask;

    [SerializeField] private PlayerCombat player;

    [Header("Advanced Settings")]
    [SerializeField] private float attackCooldown = 2f; // Melee attack cooldown
    [SerializeField] private float magicFireCooldown = 5f; // Magic fire cooldown
    [SerializeField] private float LightningCooldown = 8f;
    private bool canAttack = true;
    private bool canCastMagicFire = true;
    private bool canCastLightningAttack = true;

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
            TryMagicFireAttack();
            TryMeleeAttack();
            TryLightningAttack();
            nextAttackTime = 0f;
        }
    }

    public void TryMeleeAttack()
    {
        Vector2 direction = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        RaycastHit2D hit = Physics2D.Raycast(attackPoint.position, direction, attackRange, playerLayerMask);

        if (hit.collider != null && canAttack)
        {
            anim.SetBool("isAttacking", true);
            MeleeAttack(direction);
            canAttack = false;
            Invoke("ResetAttackCooldown", attackCooldown);

            OnEnemyAttack?.Invoke(); // Notify subscribers
        }
        else
        {
            anim.SetBool("isAttacking", false);
        }
    }

    private void MeleeAttack(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(attackPoint.position, direction, attackRange, playerLayerMask);

        if (hit.collider != null)
        {
            int damageToDeal = (Random.value < criticalChance) ? criticalDamage : baseDamage;
            player.TakingDamage(damageToDeal);

            if (audioSource != null)
                audioSource.Play();
        }
    }

    public void TryMagicFireAttack()
    {
        Vector2 direction = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        RaycastHit2D hit = Physics2D.Raycast(magicFireAttackPoint.position, direction, magicFireRange, playerLayerMask);

        if (hit.collider != null && canCastMagicFire)
        {
            anim.SetBool("MagicFire", true);
            MagicFireAttack();
            canCastMagicFire = false;
            Invoke("ResetMagicFireCooldown", magicFireCooldown);
        }
        else
        {
            anim.SetBool("MagicFire", false);
        }
    }

    private void MagicFireAttack()
    {
        Debug.Log("Casting Magic Fire!");

        RaycastHit2D hit = Physics2D.Raycast(magicFireAttackPoint.position, Vector2.right, magicFireRange, playerLayerMask);

        if (hit.collider != null)
        {
            //int damageToDeal = baseDamage * 2; // Magic fire deals more damage
            //player.TakingDamage(damageToDeal);
            //anim.SetTrigger("MagicFire");
            if (audioSource != null)
                audioSource.Play();
        }
    }

    void TryLightningAttack()
    {
        Vector2 direction = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        RaycastHit2D hit = Physics2D.Raycast(LightningAttackPoint.position, direction, lightningRange, playerLayerMask);
        if(hit.collider != null)
        {
            //anim.SetBool("LightningAttack", true);
            
            LightningAttack();
        }
        else
        {
            //anim.SetBool("LightningAttack", false);
        }
    }
    void LightningAttack()
    {
        Debug.Log("Casting Lightning Attack");

        RaycastHit2D hit = Physics2D.Raycast(LightningAttackPoint.position, Vector2.right, lightningRange, playerLayerMask);
        if(hit.collider != null)
        {
            anim.SetTrigger("LightningAttack");
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

    private void ResetMagicFireCooldown()
    {
        canCastMagicFire = true;
    }

    private void ResetMagicLightningCooldown()
    {
        canCastLightningAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 direction = spriteRenderer != null && spriteRenderer.flipX ? Vector2.left : Vector2.right;
        Gizmos.DrawRay(attackPoint.position, direction * attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(magicFireAttackPoint.position, direction * magicFireRange);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(LightningAttackPoint.position, direction * lightningRange);
    }
}



//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Boss : MonoBehaviour
//{
//    Animator anim;
//    AudioSource audioSource;

//    public Transform attackPoint;
//    public Transform magicFireAttackPoint;
//    public float attackRange;
//    public float magicFireRange;
//    [SerializeField] private LayerMask playerLayerMask;

//    [SerializeField] private PlayerCombat player;

//    [Header("Advanced Settings")]
//    [SerializeField] private float attackCooldown = 2f;
//    private bool canAttack = true;


//    [SerializeField] private int baseDamage = 5;
//    [SerializeField] private int criticalDamage = 20;
//    [SerializeField] private float criticalChance = 0.2f;

//    public delegate void EnemyEvent();
//    public static event EnemyEvent OnEnemyAttack;
//    private float nextAttackTime;

//    private SpriteRenderer spriteRenderer;

//    void Start()
//    {
//        anim = GetComponent<Animator>();
//        audioSource = GetComponent<AudioSource>();
//        player = FindObjectOfType<PlayerCombat>();
//        spriteRenderer = GetComponent<SpriteRenderer>();
//    }

//    void Update()
//    {
//        nextAttackTime += Time.deltaTime;
//        if (nextAttackTime >= 2)
//        {
//            EAttack();
//            nextAttackTime = 0f;
//        }
//    }

//    public void EAttack()
//    {
//        Vector2 direction = spriteRenderer.flipX ? Vector2.left : Vector2.right;
//        RaycastHit2D attackRangeRayCast = Physics2D.Raycast(attackPoint.position, direction, attackRange, playerLayerMask);

//        if (attackRangeRayCast.collider != null && canAttack)
//        {
//            anim.SetBool("isAttacking", true);
//            AttackPlayer(direction);
//            canAttack = false;
//            Invoke("ResetAttackCooldown", attackCooldown);

//            // Notify subscribers that the enemy is attacking
//            OnEnemyAttack?.Invoke();
//        }
//        else
//        {
//            anim.SetBool("isAttacking", false);
//        }
//    }

//    private void AttackPlayer(Vector2 direction)
//    {
//        RaycastHit2D attackRangeRayCast = Physics2D.Raycast(attackPoint.position, direction, attackRange, playerLayerMask);

//        if (attackRangeRayCast.collider != null && canAttack)
//        {
//            // Apply damage to the player with a chance for critical damage
//            int damageToDeal = (Random.value < criticalChance) ? criticalDamage : baseDamage;
//            player.TakingDamage(damageToDeal);

//            // Play attack sound
//            if (audioSource != null)
//            {
//                audioSource.Play();
//            }
//        }
//    }

//    private void ResetAttackCooldown()
//    {
//        canAttack = true;
//    }

//    private void OnDrawGizmosSelected()
//    {
//        Gizmos.color = Color.red;
//        Vector2 direction = spriteRenderer != null && spriteRenderer.flipX ? Vector2.left : Vector2.right;
//        Gizmos.DrawRay(attackPoint.position, direction * attackRange);

//    }
//}
