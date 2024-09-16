//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class PlayerCombat : MonoBehaviour
//{
//    public static PlayerCombat instance; // modified
//    private GameController GC;

//    private Rigidbody2D rb; // modified
//    private Animator anim; // modified
//    private AudioSource audioSource; // modified

//    // Health System
//    public PlayerHealthBar pHealthBar; // modified
//    public int maxHealth = 100; // modified
//    public int currentHealth; // modified

//    // Player Position
//    public Vector3 Position; // modified

//    [SerializeField] private Transform attackPoint; // modified
//    [SerializeField] private float attackPointRadius = 1f; // modified
//    [SerializeField] private LayerMask enemyLayerMask; // modified

//    [Header("Combat Settings")]
//    [SerializeField] private int enemyDamageValue = 20; // modified
//    [SerializeField] private float attackCooldown = 0.1f; // modified
//    private bool canAttack = true; // modified

//    [Header("Audio")]
//    [SerializeField] private AudioClip attackSound; // modified

//    public delegate void PlayerEvent();
//    public static event PlayerEvent OnPlayerAttack;

//    private void Awake()
//    {
//        if (instance == null) // modified
//        {
//            instance = this; // modified
//        }
//        else
//        {
//            Destroy(this); // modified
//        }
//        DontDestroyOnLoad(this); // modified
//    }

//    // Start is called before the first frame update
//    void Start()
//    {
//        rb = GetComponent<Rigidbody2D>(); // modified
//        anim = GetComponent<Animator>(); // modified
//        audioSource = GetComponent<AudioSource>(); // modified
//        currentHealth = maxHealth; // modified
//        pHealthBar.SetHealth(maxHealth); // modified
//        GC = FindObjectOfType<GameController>(); // modified
//        transform.position = GC.lastCheckPoint; // modified
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.E) && canAttack) // modified
//        {
//            StartCoroutine(Attack()); // modified

//        }
//        //else
//        //{
//        //    anim.SetBool("isAttacking", false); // modified
//        //}

//        if (Input.GetKeyDown(KeyCode.B)) // modified
//        {
//            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // modified
//        }


//    }

//    private void FixedUpdate() // modified
//    {
//        // No changes required here
//    }

//    private IEnumerator Attack()
//    {
//        canAttack = false;
//        anim.SetBool("isAttacking", true);

//        Collider2D[] onEnemiesHit = Physics2D.OverlapCircleAll(attackPoint.position, attackPointRadius, enemyLayerMask);
//        foreach (Collider2D enemyCollider in onEnemiesHit)
//        {
//            Debug.Log("We hit " + enemyCollider.name);
//            enemyCollider.GetComponent<EnemyDamage>().OnTakingDamage(enemyDamageValue);
//        }

//        OnPlayerAttack?.Invoke();

//        if (attackSound != null && audioSource != null)
//        {
//            audioSource.PlayOneShot(attackSound);
//        }

//        yield return new WaitForSeconds(attackCooldown);
//        canAttack = true;
//        anim.SetBool("isAttacking", false);
//    }

//    private void ResetAttackCooldown() // modified
//    {
//        canAttack = true; // modified
//    }

//    public void TakingDamage(int damage) // modified
//    {
//        currentHealth -= damage; // modified

//        pHealthBar.SetHealth(currentHealth); // modified

//        if (currentHealth > 0) // modified
//        {
//            Debug.Log("Losing health"); // modified
//            anim.SetTrigger("isHurting"); // modified
//        }

//        if (currentHealth <= 0) // modified
//        {

//            OnDeath(); // modified
//        }
//    }

//    private void OnDeath() // modified
//    {


//        rb.gravityScale = 0f; // modified
//                              // modified
//        anim.SetBool("isDead", true); // modified
//        Debug.Log("Player is dead");
//        this.GetComponent<PlayerController>().enabled = false;

//        GetComponent<Collider2D>().enabled = false; // modified
//        this.enabled = false; // modified

//        StartCoroutine(ResetPlayerState()); // modified
//        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
//    }

//    private IEnumerator ResetPlayerState() // modified
//    {


//        yield return new WaitForSeconds(2f); // modified

//        this.GetComponent<PlayerController>().enabled = true;
//        anim.SetBool("isDead", false); // Reset player state, e.g., after respawn // modified
//        rb.gravityScale = 1; // modified
//        GetComponent<Collider2D>().enabled = true; // modified
//        this.enabled = true; // modified
//        currentHealth = maxHealth; // modified


//        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
//        yield return new WaitForSeconds(3f); // modified
//        transform.position = GC.lastCheckPoint;
//        pHealthBar.SetHealth(maxHealth); // modified


//    }

//    // public void Save()
//    // {
//    //     SaveLoadSystem.SavePlayerData(this);
//    // }

//    // public void Load()
//    // {
//    //     PlayerData data = SaveLoadSystem.LoadPlayerData();

//    //     currentHealth = data.health;

//    //     Vector3 position;

//    //     position.x = data.position[0];
//    //     position.y = data.position[1];
//    //     position.z = data.position[2];

//    //     transform.position = position;
//    // }

//    private void OnDrawGizmosSelected() // modified
//    {
//        if (attackPoint == null) // modified
//        {
//            return; // modified
//        }
//        Gizmos.color = Color.red; // modified
//        Gizmos.DrawWireSphere(attackPoint.position, attackPointRadius); // modified
//    }
//}

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCombat : MonoBehaviour
{
    public static PlayerCombat instance;

    public static event Action OnPlayerRespawn; 

    private GameController GC;
    private Rigidbody2D rb;
    private Animator anim;
    private AudioSource audioSource;

    [Header("Health System")]
    public PlayerHealthBar pHealthBar;
    public int maxHealth = 100;
    public int currentHealth;
    private int healthBeforeDeath; // Store player's health before death
    bool isInvulnerable = false;

    [Header("Combat Settings")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackPointRadius = 1f;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private int enemyDamageValue = 20;
    [SerializeField] private float attackCooldown = 0.1f;
    private bool canAttack = true;
    [SerializeField] [Range(0f, 1f)] private float healthRestorePercentage = 0.3f; // Set this in the Inspector (default 30%)

    

    [Header("Audio")]
    [SerializeField] private AudioClip attackSound;

    public delegate void PlayerEvent();
    public static event PlayerEvent OnPlayerAttack;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        currentHealth = maxHealth;
        pHealthBar.SetHealth(maxHealth);

        GC = FindObjectOfType<GameController>();
        transform.position = GC.lastCheckPoint;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canAttack)
        {
            StartCoroutine(Attack());
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private IEnumerator Attack()
    {
        canAttack = false;
        anim.SetBool("isAttacking", true);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackPointRadius, enemyLayerMask);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyDamage>()?.OnTakingDamage(enemyDamageValue);
        }

        OnPlayerAttack?.Invoke();

        if (attackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
        anim.SetBool("isAttacking", false);
    }

    public void TakingDamage(int damage)
    {
        if (isInvulnerable)
        {
            return; // Don't take damage if the player is invulnerable
        }

        currentHealth -= damage;

        pHealthBar.SetHealth(currentHealth);
        if (currentHealth > 0)
        {
            anim.SetTrigger("isHurting");
        }
        if (currentHealth <= 0)
        {
            OnDeath();
        }
    }

    //private void OnDeath()
    //{
    //    // Stop all movement by setting the velocity to zero
    //    rb.velocity = Vector2.zero;

    //    // Disable controls and gravity upon death
    //    rb.gravityScale = 0f;
    //    anim.SetBool("isDead", true);

    //    // Disable player movement and collision
    //    GetComponent<PlayerController>().enabled = false;
    //    GetComponent<Collider2D>().enabled = false;
    //    this.enabled = false;

    //    // Start the respawn/reset process
    //    StartCoroutine(ResetPlayerState());
    //}

    private IEnumerator ResetPlayerState()
    {
        // Wait for 2 seconds before starting respawn
        yield return new WaitForSeconds(2f);


        // Reset animation and physics properties
        anim.SetBool("isDead", false);
        rb.gravityScale = 1f;
        GetComponent<Collider2D>().enabled = true;

        OnPlayerRespawn?.Invoke(); // Notify other scripts that the player has respawned

        // Restore a percentage of the player's health before death
        int healthToRestore = Mathf.RoundToInt(healthBeforeDeath * healthRestorePercentage);
        currentHealth = healthToRestore;

        // Move the player to the last checkpoint position
        transform.position = GC.lastCheckPoint;

        // Update the health bar without resetting currentHealth to maxHealth
        pHealthBar.SetHealth(currentHealth);

        // Brief delay before re-enabling the player controls
        yield return new WaitForSeconds(0.5f);

        // Re-enable player controls and the script
        GetComponent<PlayerController>().enabled = true;
        this.enabled = true;

        // Optionally add an invulnerability period here to prevent instant death on respawn
        StartCoroutine(TemporaryInvulnerability());
    }




    private void OnDeath()
    {
        // Store the current health before death so it can be partially restored
        healthBeforeDeath = maxHealth;

        // Stop all movement by setting the velocity to zero
        rb.velocity = Vector3.zero;

        // Disable gravity to prevent further movement or falling
        rb.gravityScale = 0f;

        // Play death animation
        anim.SetBool("isDead", true);

        // Disable player controls and collision
        GetComponent<PlayerController>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        // Disable this script to stop further actions
        this.enabled = false;

        // Start coroutine to handle respawn and reset player state
        StartCoroutine(ResetPlayerState());
    }

    //private IEnumerator ResetPlayerState()
    //{
    //    // Wait for a few seconds before respawn
    //    yield return new WaitForSeconds(2f);

     

    //    // Reset player state (enable controls, gravity, etc.)
    //    this.GetComponent<PlayerController>().enabled = true;
    //    anim.SetBool("isDead", false);
    //    rb.gravityScale = 1f;
    //    GetComponent<Collider2D>().enabled = true;
    //    this.enabled = true;

    //    // Update the health bar to reflect the restored health
    //    pHealthBar.SetHealth(currentHealth);

    //    // Wait a bit and then teleport the player to the last checkpoint
    //    yield return new WaitForSeconds(1f);
    //    transform.position = GC.lastCheckPoint;
    //}



    public void IncreaseHealth(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        pHealthBar.SetHealth(currentHealth);
        Debug.Log("Health increased by " + amount);
    }



    private IEnumerator TemporaryInvulnerability()
    {
        // Set the player to invulnerable, using a flag that prevents damage
        isInvulnerable = true;

        // Optionally, change player appearance to indicate invulnerability (like flashing effect)
        StartCoroutine(FlashDuringInvulnerability());

        yield return new WaitForSeconds(2f); // Duration of invulnerability

        // Re-enable the player's vulnerability to damage
        isInvulnerable = false;
    }

    private IEnumerator FlashDuringInvulnerability()
    {
        SpriteRenderer playerSprite = GetComponent<SpriteRenderer>();
        Color originalColor = playerSprite.color;

        while (isInvulnerable)
        {
            // Flash effect: alternate between visible and invisible
            playerSprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f); // half transparency
            yield return new WaitForSeconds(0.1f);
            playerSprite.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }

        // Ensure player returns to original color after invulnerability
        playerSprite.color = originalColor;
    }


    //private void OnDeath()
    //{
    //    rb.gravityScale = 0f;
    //    anim.SetBool("isDead", true);

    //    GetComponent<PlayerController>().enabled = false;
    //    GetComponent<Collider2D>().enabled = false;
    //    this.enabled = false;

    //    StartCoroutine(ResetPlayerState());
    //}

    //private IEnumerator ResetPlayerState()
    //{
    //    yield return new WaitForSeconds(2f);

    //    this.GetComponent<PlayerController>().enabled = true;
    //    anim.SetBool("isDead", false);
    //    rb.gravityScale = 1f;
    //    GetComponent<Collider2D>().enabled = true;
    //    this.enabled = true;

    //    currentHealth = maxHealth;
    //    pHealthBar.SetHealth(maxHealth);

    //    yield return new WaitForSeconds(1f);
    //    transform.position = GC.lastCheckPoint;
    //}

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackPointRadius);
        }
    }
}


