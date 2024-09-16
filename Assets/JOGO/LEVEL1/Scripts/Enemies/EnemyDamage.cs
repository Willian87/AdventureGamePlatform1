//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class EnemyDamage : MonoBehaviour
//{

//    private Enemy enemy;
//    Rigidbody2D rb;

//    [SerializeField] private int _MaxHealth = 100;
//    [SerializeField] private int _CurrentHealth;

//     private Animator anim;

//    [SerializeField] private EnemyHealthBar _EHealthBar;

//    [SerializeField] private Canvas canvas;

//    // Start is called before the first frame update
//    void Start()
//    {
//        _CurrentHealth = _MaxHealth;
//        rb = GetComponent<Rigidbody2D>();
//        anim = GetComponent<Animator>();
//    }

//    public void OnTakingDamage(int damage)
//    {
//        _CurrentHealth -= damage;

//        _EHealthBar.SetHealth(_CurrentHealth);

//        anim.SetBool("isHurting", true);

//        if (_CurrentHealth > 0)
//        {
//            Debug.Log("juice indo embora");

//        }
//        if(_CurrentHealth <= 0)
//        {
//            Debug.Log("juice foi embora");
//            anim.SetBool("isHurting", false);
//            OnEnemyDeath();
//        }
//    }

//    void OnEnemyDeath()
//    {

//        rb.gravityScale = 0;
//        anim.SetBool("isDead", true);
//        canvas.enabled = false;
//        GetComponent<Collider2D>().enabled = false;
//        this.enabled = false;
//        return;
//    }

//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    private Enemy enemy;
    private EnemyPatrol enemyPatrol;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    private Animator anim;
    private Rigidbody2D rb;
    private Canvas healthCanvas;
    private EnemyHealthBar healthBar;

    [SerializeField] float enemyOut = 2f;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Assuming EnemyHealthBar and Canvas are assigned in the Inspector
        healthBar = GetComponentInChildren<EnemyHealthBar>();
        healthCanvas = GetComponentInChildren<Canvas>();

        // Initialize health bar
        if (healthBar != null)
        {
            healthBar.SetMaxValue(maxHealth);
            healthBar.SetHealth(currentHealth);
        }
    }

    public void OnTakingDamage(int damage)
    {
        currentHealth -= damage;

        // Update health bar display
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        anim.SetBool("isHurting", true);

        if (currentHealth <= 0)
        {
            
            
           StartCoroutine(Die());
            
        }
    }

    IEnumerator Die()
    {
        // Perform death actions
        
        rb.gravityScale = 0;
        
        anim.SetBool("isDead", true);

        // Disable health display
        if (healthCanvas != null)
        {
            healthCanvas.enabled = false;
        }

        // Disable collider and script
        Collider2D[] colliders = GetComponents<Collider2D>();
        
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }

        this.GetComponent<Enemy>().enabled = false;
        this.GetComponent<EnemyPatrol>().enabled = false;


        yield return new WaitForSeconds(enemyOut);
        Debug.Log("No sumi pq?");
        this.gameObject.SetActive(false);
    }

    

}

