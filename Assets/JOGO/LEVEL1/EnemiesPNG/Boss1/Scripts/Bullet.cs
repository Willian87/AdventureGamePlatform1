using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //[SerializeField] private float bulletSpeed = 10f;
    //private Rigidbody2D rb;

    [SerializeField] private GameObject impactPrefab;

    //PlayerCombat player;
    // Start is called before the first frame update
    void Start()
    {
        //rb = GetComponent<Rigidbody2D>();
        //player = GetComponent<PlayerCombat>();
        //Destroy(gameObject, 3f);
        //rb.velocity = transform.right * bulletSpeed;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Toma safado!");
            collision.GetComponent<PlayerCombat>().TakingDamage(10);

        }


        Instantiate(impactPrefab, transform.position, transform.rotation);


        Destroy(gameObject);
    }

}
