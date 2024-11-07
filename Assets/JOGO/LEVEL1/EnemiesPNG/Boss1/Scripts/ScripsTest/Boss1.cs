using System.Collections;
using UnityEngine;

public class Boss1 : MonoBehaviour
{
    PlayerCombat playerCombat;
    [Header("General Settings")]
    public Transform player;                 // Reference to the player
    private Animator animator;
    private bool canAttack = true;

    [Header("Sword Attack Settings")]
    public float swordAttackRange = 1.5f;     // Range for sword attack
    public int damage = 10;
    public float swordAttackCooldown = 3f;    // Cooldown for sword attack

    [Header("Magic Fire Attack Settings")]
    public GameObject fireProjectilePrefab;   // Prefab of the fire projectile
    public Transform fireSpawnPoint;          // Spawn point for the projectile
    public float fireAttackCooldown = 5f;     // Cooldown for magic fire attack
    public float fireProjectileSpeed = 5f;    // Speed of the projectile

    private float nextSwordAttackTime = 0f;
    private float nextFireAttackTime = 0f;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Check if boss can perform a sword attack
        if (Time.time >= nextSwordAttackTime && distanceToPlayer <= swordAttackRange)
        {
            StartCoroutine(SwordAttack());
            nextSwordAttackTime = Time.time + swordAttackCooldown;
        }
        // Check if boss can perform a magic fire attack
        else if (Time.time >= nextFireAttackTime && distanceToPlayer > swordAttackRange)
        {
            StartCoroutine(MagicFireAttack());
            nextFireAttackTime = Time.time + fireAttackCooldown;
        }
    }

    private IEnumerator SwordAttack()
    {
        canAttack = false;
        animator.SetTrigger("SwordAttack");

        yield return new WaitForSeconds(0.5f); // Wait for the attack animation to hit

        // Check if player is within sword attack range
        if (Vector2.Distance(transform.position, player.position) <= swordAttackRange)
        {
            //player.GetComponent<PlayerHealth>().TakeDamage(swordDamage);
            playerCombat.TakingDamage(damage);
        }

        yield return new WaitForSeconds(swordAttackCooldown - 0.5f);
        canAttack = true;
    }

    private IEnumerator MagicFireAttack()
    {
        canAttack = false;

        animator.SetTrigger("MagicFireAttack");
        yield return new WaitForSeconds(0.5f); // Wait for the animation timing to spawn the projectile
        
        // Spawn the fire projectile
        GameObject fireProjectile = Instantiate(fireProjectilePrefab, fireSpawnPoint.position, Quaternion.identity);
        Rigidbody2D rb = fireProjectile.GetComponent<Rigidbody2D>();
        Vector2 direction = (player.position - fireSpawnPoint.position).normalized;
        rb.velocity = direction * fireProjectileSpeed;

        yield return new WaitForSeconds(fireAttackCooldown - 0.5f);
        canAttack = true;
    }
}
