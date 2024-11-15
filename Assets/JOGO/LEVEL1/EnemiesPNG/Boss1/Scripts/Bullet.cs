using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 20;
    public float lifetime = 5f;

    [SerializeField] private LayerMask playerLayerMask;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Deal damage to the player
            collision.GetComponent<PlayerCombat>().TakingDamage(damage);
            Destroy(gameObject);
        }
    }
}

