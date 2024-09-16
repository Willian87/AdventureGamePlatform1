using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlatformBoxColliderTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Tocou!");
            collision.transform.parent = this.transform;

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.parent = null;
            Debug.Log("Saiu!");

        }
    }
}
