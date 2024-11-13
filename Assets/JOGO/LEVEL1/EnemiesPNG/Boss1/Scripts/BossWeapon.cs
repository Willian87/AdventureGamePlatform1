

using System.Collections;
using UnityEngine;

public class BossWeapon : MonoBehaviour
{
    [Header("Player Reference")]
    public Transform player;

    private Animator animator;
    private AudioSource audioSource;

    [Header("Sword Attack Settings")]
    public float swordAttackRange = 2f;
    public int swordDamage = 20;
    public float swordCooldown = 3f;
    public AudioClip swordSound;
    public ParticleSystem swordEffect;

    [Header("Magic Fire Attack Settings")]
    public GameObject fireProjectilePrefab;
    public Transform fireSpawnPoint;
    public float fireCooldown = 5f;
    public float fireProjectileSpeed = 8f;
    //public AudioClip fireSound;
    //public ParticleSystem fireCastEffect;

    [Header("Magic Blade Attack Settings")]
    public GameObject bladeProjectilePrefab;
    public Transform bladeSpawnPoint;
    public float bladeCooldown = 7f;
    public float bladeProjectileSpeed = 6f;
    //public AudioClip bladeSound;
    //public ParticleSystem bladeCastEffect;

    [Header("Magic Lightning Attack Settings")]
    public GameObject lightningProjectilePrefab;
    public Transform lightningSpawnPoint;
    public float lightningCooldown = 10f;
    public float lightningProjectileSpeed = 10f;
    //public AudioClip lightningSound;
    //public ParticleSystem lightningCastEffect;

    private float nextSwordAttackTime;
    private float nextFireAttackTime;
    private float nextBladeAttackTime;
    private float nextLightningAttackTime;

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {

    }

    public IEnumerator PerformSwordAttack()
    {
        animator.SetBool("SwordAttack", true);
        //audioSource.PlayOneShot(swordSound);
        //swordEffect.Play();

        yield return new WaitForSeconds(0.3f);

        if (Vector2.Distance(transform.position, player.position) <= swordAttackRange)
        {
            //player.GetComponent<PlayerHealth>().TakeDamage(swordDamage);
        }

        yield return new WaitForSeconds(swordCooldown);
    }

    public IEnumerator PerformMagicFireAttack()
    {
        animator.SetTrigger("MagicFireAttack");
        //audioSource.PlayOneShot(fireSound);
        //fireCastEffect.Play();

        yield return new WaitForSeconds(0.5f);

        GameObject fireProjectile = Instantiate(fireProjectilePrefab, fireSpawnPoint.position, Quaternion.identity);
        Rigidbody2D rb = fireProjectile.GetComponent<Rigidbody2D>();
        Vector2 direction = (player.position - fireSpawnPoint.position).normalized;
        rb.velocity = direction * fireProjectileSpeed;

        Destroy(fireProjectile, 3f);
    }

    public IEnumerator PerformMagicBladeAttack()
    {
        animator.SetTrigger("MagicBladeAttack");
        //audioSource.PlayOneShot(bladeSound);
        //bladeCastEffect.Play();

        yield return new WaitForSeconds(0.5f);

        GameObject bladeProjectile = Instantiate(bladeProjectilePrefab, bladeSpawnPoint.position, Quaternion.identity);
        Rigidbody2D rb = bladeProjectile.GetComponent<Rigidbody2D>();
        Vector2 direction = (player.position - bladeSpawnPoint.position).normalized;
        rb.velocity = direction * bladeProjectileSpeed;

        Destroy(bladeProjectile, 3f);
    }

    public IEnumerator PerformMagicLightningAttack()
    {
        animator.SetTrigger("MagicLightningAttack");
        //audioSource.PlayOneShot(lightningSound);
        //lightningCastEffect.Play();

        yield return new WaitForSeconds(0.5f);

        GameObject lightningProjectile = Instantiate(lightningProjectilePrefab, lightningSpawnPoint.position, Quaternion.identity);
        Rigidbody2D rb = lightningProjectile.GetComponent<Rigidbody2D>();
        Vector2 direction = (player.position - lightningSpawnPoint.position).normalized;
        rb.velocity = direction * lightningProjectileSpeed;

        Destroy(lightningProjectile, 3f);
    }
}




