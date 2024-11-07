using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDamage1 : MonoBehaviour
{
    private Boss1 bossScript;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    private Animator anim;
    private Rigidbody2D rb;
    private Canvas healthCanvas;
    private EnemyHealthBar healthBar;

    [SerializeField] float enemyOut = 2f;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        healthBar = GetComponentInChildren<EnemyHealthBar>();
        healthCanvas = GetComponentInChildren<Canvas>();
        bossScript = GetComponent<Boss1>();

        if (healthBar != null)
        {
            healthBar.SetMaxValue(maxHealth);
            healthBar.SetHealth(currentHealth);
        }
    }

    public void OnTakingDamage(int damage)
    {
        currentHealth -= damage;

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        anim.SetBool("isHurting", true);
        //bossScript.OnHealthChange(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        rb.gravityScale = 0;
        anim.SetBool("isDead", true);

        if (healthCanvas != null)
        {
            healthCanvas.enabled = false;
        }

        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }

        this.GetComponent<Boss>().enabled = false;
        this.GetComponent<BossPatrol>().enabled = false;

        yield return new WaitForSeconds(enemyOut);
        this.gameObject.SetActive(false);
    }
}

