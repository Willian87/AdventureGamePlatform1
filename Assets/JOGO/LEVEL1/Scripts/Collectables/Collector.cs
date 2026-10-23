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

    private void Awake()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene,
        UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        if (GetComponent<PlayerCombat>() != PlayerCombat.instance) return;
        if (gameOverManager == null) gameOverManager = FindObjectOfType<GameOverManager>(true);
        UpdateHeartUI();
    }

    public void ResetLives()
    {
        StopAllCoroutines();
        heartCount = 3;
        coinCount = 0;
        OnCoinCountReset?.Invoke();
        UpdateHeartUI();
    }

    private void Start()
    {
        playerCombat = GetComponent<PlayerCombat>();
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
                CheckCoinCountForHealthIncrease(((Coins)collectables).healthIncrease);
            }
            else if (collectables is Heart)
            {
                IncrementHeartCount();
            }
            else if(collectables is Apple)
            {
                playerCombat.IncreaseHealth(((Apple)collectables).healthIncrease);
            }
        }
    }

    private void CheckCoinCountForHealthIncrease(int healthIncreaseAmount)
    {
        if(playerCombat.currentHealth < playerCombat.maxHealth)
        {
            if (coinCount >= coinsNeededForHealthIncrease)
            {
                playerCombat.IncreaseHealth(healthIncreaseAmount);
                coinCount = 0; // Reset coin count after increasing health
                OnCoinCountReset?.Invoke();
            }
        }
        
    }

    private void IncrementHeartCount()
    {
        heartCount++;
        UpdateHeartUI();
    }

    public void DecreaseHeartCount()
    {
        if (heartCount <= 0) return;
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
        HeartUI heartUI = PersistentGameplayUI.FindHUD<HeartUI>();
        if (heartUI != null) heartUI.SetHeartCount(heartCount);
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
