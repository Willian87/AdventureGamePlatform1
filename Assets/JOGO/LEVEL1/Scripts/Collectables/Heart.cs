using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Heart : MonoBehaviour, CollectableItems
{
    public static event Action OnHeartCollected;
    public int healthIncrease = 20; // Amount of health to increase after a set number of hearts

    public void Collector()
    {
        Debug.Log("Ai meu coracao!");
        Destroy(gameObject);
        OnHeartCollected?.Invoke();
    }
}
