using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private bool destroyOnHit = true;
    [SerializeField] private LayerMask collisionLayers;

    [Header("Visual Effects")]
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private GameObject trailEffectPrefab;
    [SerializeField] private bool rotateTowardsDirection = true;

    [Header("Damage Settings")]
    [SerializeField] private float explosionRadius = 0f; // 0 for single target, >0 for AoE
    [SerializeField] private bool canDamageEnemy = false; // Set true if projectile should damage enemies

    private int damage;
    private TrailRenderer trailRenderer;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveDirection;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();

        // Create trail effect if specified
        if (trailEffectPrefab != null)
        {
            Instantiate(trailEffectPrefab, transform);
        }

        // Destroy after lifetime
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (rotateTowardsDirection)
        {
            // Calculate rotation based on velocity
            if (GetComponent<Rigidbody2D>().velocity != Vector2.zero)
            {
                float angle = Mathf.Atan2(GetComponent<Rigidbody2D>().velocity.y,
                    GetComponent<Rigidbody2D>().velocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if we hit a valid target
        if (((1 << other.gameObject.layer) & collisionLayers) != 0)
        {
            HandleCollision(other);
        }
    }

    private void HandleCollision(Collider2D other)
    {
        // Handle AoE damage
        if (explosionRadius > 0)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, collisionLayers);
            foreach (Collider2D hit in hitColliders)
            {
                DealDamage(hit.gameObject);
            }
        }
        else
        {
            // Single target damage
            DealDamage(other.gameObject);
        }

        // Spawn hit effect
        if (hitEffectPrefab != null)
        {
            GameObject hitEffect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(hitEffect, 1f);
        }

        // Destroy projectile if specified
        if (destroyOnHit)
        {
            // If we have a trail, detach it before destroying
            if (trailRenderer != null)
            {
                trailRenderer.transform.parent = null;
                Destroy(trailRenderer.gameObject, trailRenderer.time);
            }

            Destroy(gameObject);
        }
    }

    private void DealDamage(GameObject target)
    {
        // Check if target is player
        if (target.CompareTag("Player"))
        {
            PlayerCombat playerHealth = target.GetComponent<PlayerCombat>();
            if (playerHealth != null)
            {
                playerHealth.TakingDamage(damage);
            }
        }
        // Check if target is enemy and projectile can damage enemies
        else if (canDamageEnemy && target.CompareTag("Enemy"))
        {
            BossBehavior enemyHealth = target.GetComponent<BossBehavior>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }
    }

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    // Visualize explosion radius in editor
    private void OnDrawGizmosSelected()
    {
        if (explosionRadius > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Bullet : MonoBehaviour
//{
//    public float speed = 10f;
//    public int damage = 20;
//    public float lifetime = 5f;

//    [SerializeField] private LayerMask playerLayerMask;

//    private void Start()
//    {
//        Destroy(gameObject, lifetime);
//    }

//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        if (collision.CompareTag("Player"))
//        {
//            // Deal damage to the player
//            collision.GetComponent<PlayerCombat>().TakingDamage(damage);
//            Destroy(gameObject);
//        }
//    }
//}

