//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Collector : MonoBehaviour
//{
//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        CollectableItems collectables = collision.GetComponent<CollectableItems>();
//        if(collectables != null)
//        {
//            collectables.Collector();
//        }
//    }
//}

using UnityEngine;

public class Collector : MonoBehaviour
{
    private int coinCount = 0;
    private int coinsNeededForHealthIncrease = 5; // Number of coins needed to increase health
    private int heartCount = 0;
    private int heartsNeededForHealthIncrease = 1;

    private PlayerCombat playerCombat;

    private void Start()
    {
        playerCombat = FindObjectOfType<PlayerCombat>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CollectableItems collectables = collision.GetComponent<CollectableItems>();
        if (collectables != null)
        {
            collectables.Collector();
            coinCount++;
            heartCount++;
            CheckCoinCountForHealthIncrease();
            CheckHeartCountForHealthIncrease();
        }
    }

    private void CheckCoinCountForHealthIncrease()
    {
        if (coinCount >= coinsNeededForHealthIncrease)
        {
            int healthIncreaseAmount = FindObjectOfType<Coins>().healthIncrease;
            playerCombat.IncreaseHealth(healthIncreaseAmount);
            coinCount = 0; // Reset coin count after increasing health
        }
    }

    private void CheckHeartCountForHealthIncrease()
    {
        if(heartCount >= heartsNeededForHealthIncrease)
        {
            int healthIncreaseAmount = FindObjectOfType<Heart>().healthIncrease;
            playerCombat.IncreaseHealth(healthIncreaseAmount);
            heartCount = 0;
        }
    }
}

