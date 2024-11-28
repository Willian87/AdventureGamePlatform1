using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Apple : MonoBehaviour, CollectableItems
{
    public static event Action OnAppleCollected;
    public int healthIncrease = 20; // Amount of health to increase after a set number of Apples

    public void Collector()
    {
        Debug.Log("Tem veneno");
        Destroy(gameObject);
        OnAppleCollected?.Invoke();
    }

    
}
