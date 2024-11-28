using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Animator anim;
    AudioSource audioSource;

    public Transform attackPoint;
    public float attackRange;
    [SerializeField] private LayerMask playerLayerMask;

    [SerializeField] private PlayerCombat player;

    [Header("Advanced Settings")]
    [SerializeField] private float attackCooldown = 2f;
    private bool canAttack = true;

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

        if (player == null)
        {
            Debug.LogError("No PlayerCombat found in the scene!");
        }
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











