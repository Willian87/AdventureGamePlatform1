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

//using UnityEngine;

//public class Collector : MonoBehaviour
//{
//    private int coinCount = 0;
//    private int coinsNeededForHealthIncrease = 5; // Number of coins needed to increase health
//    private int heartCount = 0;
//    private int heartsNeededForHealthIncrease = 1;

//    private PlayerCombat playerCombat;

//    private void Start()
//    {
//        playerCombat = FindObjectOfType<PlayerCombat>();
//    }

//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        CollectableItems collectables = collision.GetComponent<CollectableItems>();
//        if (collectables != null)
//        {
//            collectables.Collector();
//            coinCount++;
//            heartCount++;
//            CheckCoinCountForHealthIncrease();
//            CheckHeartCountForHealthIncrease();
//        }
//    }

//    private void CheckCoinCountForHealthIncrease()
//    {
//        if (coinCount >= coinsNeededForHealthIncrease)
//        {
//            int healthIncreaseAmount = FindObjectOfType<Coins>().healthIncrease;
//            playerCombat.IncreaseHealth(healthIncreaseAmount);
//            coinCount = 0; // Reset coin count after increasing health
//        }
//    }

//    private void CheckHeartCountForHealthIncrease()
//    {
//        if(heartCount >= heartsNeededForHealthIncrease)
//        {
//            int healthIncreaseAmount = FindObjectOfType<Heart>().healthIncrease;
//            playerCombat.IncreaseHealth(healthIncreaseAmount);
//            heartCount = 0;
//        }
//    }
//}
using System;
using System.Collections;
using UnityEngine;

public class Collector : MonoBehaviour
{
    private int coinCount = 0;
    private int coinsNeededForHealthIncrease = 15; // Number of coins needed to increase health
    private int heartCount = 3; // Initial number of lives (hearts)

    [SerializeField] private GameOverManager gameOverManager;
    [SerializeField] private float gameOverCount;

    public static event Action OnCoinCountReset;

    private PlayerCombat playerCombat;

    private void Start()
    {
        playerCombat = FindObjectOfType<PlayerCombat>();
        UpdateHeartUI();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CollectableItems collectables = collision.GetComponent<CollectableItems>();
        if (collectables != null)
        {
            collectables.Collector();
            if (collectables is Coins)
            {
                coinCount++;
                CheckCoinCountForHealthIncrease();
            }
            else if (collectables is Heart)
            {
                IncrementHeartCount();
            }
            else if(collectables is Apple)
            {
                AppleCountForHealthIncrease();
            }
        }
    }

    private void CheckCoinCountForHealthIncrease()
    {
        if(playerCombat.currentHealth < playerCombat.maxHealth)
        {
            if (coinCount >= coinsNeededForHealthIncrease)
            {
                int healthIncreaseAmount = FindObjectOfType<Coins>().healthIncrease;
                playerCombat.IncreaseHealth(healthIncreaseAmount);
                coinCount = 0; // Reset coin count after increasing health
                OnCoinCountReset?.Invoke();
            }
        }
        
    }

    private void AppleCountForHealthIncrease()
    {
       
            int healthIncreaseAmount = FindObjectOfType<Apple>().healthIncrease;
            playerCombat.IncreaseHealth(healthIncreaseAmount);
            coinCount = 0; // Reset coin count after increasing health
        
    }

    private void IncrementHeartCount()
    {
        heartCount++;
        UpdateHeartUI();
    }

    public void DecreaseHeartCount()
    {
        heartCount--;
        UpdateHeartUI();

        if (heartCount <= 0)
        {
            StartCoroutine(GameOver());
        }
    }

    public int GetHeartCount() 
    {
        return heartCount;
    }

    private void UpdateHeartUI()
    {
        FindObjectOfType<HeartUI>().SetHeartCount(heartCount);
    }

    //private void GameOver()
    //{
    //    Debug.Log("Game Over!");
    //    // Add game-over handling logic here (e.g., show game-over screen, stop gameplay)
        
    //    StartCoroutine(GameOver());
    //}

    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(gameOverCount);
        gameOverManager.TriggerGameOver();
    }
}



