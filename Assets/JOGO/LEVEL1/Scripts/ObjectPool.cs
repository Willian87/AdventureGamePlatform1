using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Header("Pooling Settings")]
    [SerializeField] private GameObject platformPrefab; // The prefab for the falling platform
    [SerializeField] private int poolSize = 5;          // Number of platforms in the pool

    private Queue<GameObject> pool = new Queue<GameObject>(); // Queue for managing the pool

    private void Awake()
    {
        // Initialize the pool with inactive platform instances
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(platformPrefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetPlatform()
    {
        if (pool.Count > 0)
        {
            // Get a platform from the pool and activate it
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            // Optionally, instantiate a new platform if pool is empty
            GameObject obj = Instantiate(platformPrefab);
            return obj;
        }
    }

    public void ReturnPlatform(GameObject obj)
    {
        // Deactivate and return the platform to the pool
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
