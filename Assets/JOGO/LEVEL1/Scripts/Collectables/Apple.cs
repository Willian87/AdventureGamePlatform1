using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Apple : MonoBehaviour, CollectableItems
{
    public static event Action OnAppleCollected; 

    public void Collector()
    {
        Debug.Log("Tem veneno");
        Destroy(gameObject);
        OnAppleCollected?.Invoke();
    }

    
}
