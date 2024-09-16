//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System;

//public class Coins : MonoBehaviour, CollectableItems
//{

//    public static event Action OnCoinCollected;


//    public void Collector()
//    {
//        Debug.Log("Peguei essa porra!");
//        Destroy(gameObject);

//        OnCoinCollected?.Invoke();
//        FindObjectOfType<SoundManagment>().Play("CoinCollected");
//    }
//}

using System;
using UnityEngine;

public class Coins : MonoBehaviour, CollectableItems
{
    public static event Action OnCoinCollected;

    public int healthIncrease = 20; // Amount of health to increase after a set number of coins

    public void Collector()
    {
        Debug.Log("Coin collected!");
        Destroy(gameObject);

        OnCoinCollected?.Invoke();
        FindObjectOfType<SoundManagment>().Play("CoinCollected");
    }
}

