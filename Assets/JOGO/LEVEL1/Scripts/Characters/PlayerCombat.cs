using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCombat : MonoBehaviour
{
    public static PlayerCombat instance;

    public static event Action OnPlayerRespawn;

    Collector collector;

    private GameController GC;
    private Rigidbody2D rb;
    private Animator anim;
    private AudioSource audioSource;

    [Header("Health System")]
    public PlayerHealthBar pHealthBar;
    public int maxHealth = 100;
    public int currentHealth;
    //private int healthBeforeDeath; // Store player's health before death
    bool isInvulnerable = false;
    private bool isDead;
    private float defaultGravity;
    private Color defaultColor;

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
            SceneManager.sceneLoaded += OnSceneLoaded;
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
        audioSource = GetComponentInChildren<AudioSource>();
        defaultGravity = rb.gravityScale;
        defaultColor = GetComponent<SpriteRenderer>().color;

        currentHealth = maxHealth;
        PersistentGameplayUI.PreserveHUD();
        BindSceneReferences();

        GC = FindObjectOfType<GameController>();
        if (GC != null) transform.position = GC.lastCheckPoint;
    }

    void Update()
    {
        if (Time.timeScale == 0f || isDead) return;

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
            //enemy.GetComponent<BossDamage>()?.OnTakingDamage(enemyDamageValue);
            enemy.GetComponent<BossBehavior>()?.TakeDamage(enemyDamageValue);
        }

        OnPlayerAttack?.Invoke();
        PlaySound(attackSound);

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
        anim.SetBool("isAttacking", false);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogError("Missing AudioClip or AudioSource!");
        }
    }


    public void TakingDamage(int damage)
    {
        if (isInvulnerable || isDead)
        {
            return; // Don't take damage if the player is invulnerable
        }

        currentHealth = Mathf.Max(0, currentHealth - damage);

        if (pHealthBar != null) pHealthBar.SetHealth(currentHealth);

      
        if (currentHealth > 0)
        {
            anim.SetTrigger("isHurting");
        }
        if (currentHealth <= 0)
        {
            
            OnDeath();

        }

    }

    private IEnumerator ResetPlayerState()
    {
        // Wait for 2 seconds before starting respawn
        yield return new WaitForSeconds(2f);


        // Reset animation and physics properties
        anim.SetBool("isDead", false);
        rb.gravityScale = defaultGravity;
        GetComponent<Collider2D>().enabled = true;

        OnPlayerRespawn?.Invoke(); // Notify other scripts that the player has respawned

        // Restore a percentage of the player's health before death
        int healthToRestore = Mathf.Max(1, Mathf.RoundToInt(maxHealth * healthRestorePercentage));
        currentHealth = healthToRestore;

        // Move the player to the last checkpoint position
        if (GC != null) transform.position = GC.lastCheckPoint;

        // Update the health bar without resetting currentHealth to maxHealth
        if (pHealthBar != null) pHealthBar.SetHealth(currentHealth);

        // Brief delay before re-enabling the player controls
        yield return new WaitForSeconds(0.5f);

        // Re-enable player controls and the script
        GetComponent<PlayerController>().enabled = true;
        this.enabled = true;
        isDead = false;

        // Optionally add an invulnerability period here to prevent instant death on respawn
        StartCoroutine(TemporaryInvulnerability());
    }




    private void OnDeath()
    {
        if (isDead) return;
        isDead = true;
        StopAllCoroutines();
        canAttack = true;
        anim.SetBool("isAttacking", false);
        rb.velocity = Vector2.zero; // Stop movement
        rb.gravityScale = 0f;
        anim.SetBool("isDead", true);

        GetComponent<PlayerController>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;

        collector.DecreaseHeartCount(); // Decrease life count

        if (collector.GetHeartCount() > 0) // Check if lives remain
        {
            StartCoroutine(ResetPlayerState());
        }
        else
        {
            // If no lives remain, don't reset, show Game Over instead
            Debug.Log("Out of lives. Game Over!");
        }
        //// Store the current health before death so it can be partially restored
        //healthBeforeDeath = maxHealth;

        //// Stop all movement by setting the velocity to zero
        //rb.velocity = Vector3.zero;

        //// Disable gravity to prevent further movement or falling
        //rb.gravityScale = 0f;

        //// Play death animation
        //anim.SetBool("isDead", true);

        //// Disable player controls and collision
        //GetComponent<PlayerController>().enabled = false;
        //GetComponent<Collider2D>().enabled = false;

        //// Disable this script to stop further actions
        //this.enabled = false;



        //// Start coroutine to handle respawn and reset player state
        //StartCoroutine(ResetPlayerState());
    }

    public void IncreaseHealth(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        if (pHealthBar != null) pHealthBar.SetHealth(currentHealth);
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

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (instance == this) instance = null;
    }

    private void BindSceneReferences()
    {
        GC = GameController.instance;
        collector = GetComponent<Collector>();
        pHealthBar = PersistentGameplayUI.FindHUD<PlayerHealthBar>();
        if (pHealthBar != null)
        {
            pHealthBar.SetMaxValue(maxHealth);
            pHealthBar.SetHealth(currentHealth);
        }
    }

    private string lastLevelName;

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        if (instance != this) return;
        bool reloadingLevel = lastLevelName == scene.name;
        bool gameplay = scene.name.StartsWith("Level");
        gameObject.SetActive(gameplay);
        if (!gameplay)
        {
            lastLevelName = null;
            return;
        }
        BindSceneReferences();
        if (rb != null)
        {
            int health = reloadingLevel || lastLevelName == null ? maxHealth : currentHealth;
            RestoreState(transform.position, health);
            if ((reloadingLevel || lastLevelName == null) && collector != null) collector.ResetLives();
        }
        lastLevelName = scene.name;
    }

    public void RestoreState(Vector3 position, int health)
    {
        StopAllCoroutines();
        isDead = false;
        isInvulnerable = false;
        canAttack = true;
        currentHealth = Mathf.Clamp(health, 1, maxHealth);
        transform.position = position;
        rb.velocity = Vector2.zero;
        rb.gravityScale = defaultGravity;
        anim.SetBool("isDead", false);
        anim.SetBool("isAttacking", false);
        GetComponent<Collider2D>().enabled = true;
        GetComponent<SpriteRenderer>().color = defaultColor;
        GetComponent<PlayerController>().enabled = true;
        enabled = true;
        BindSceneReferences();
        if (GC != null) GC.lastCheckPoint = position;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackPointRadius);
        }
    }
}


