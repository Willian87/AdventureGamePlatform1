using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;
    int coinCount;
   
    private void OnEnable()
    {
        Coins.OnCoinCollected += IncrementCoinCount;
        Collector.OnCoinCountReset += ResetCoinCount;
    }

    private void OnDisable()
    {
        Coins.OnCoinCollected -= IncrementCoinCount;
        Collector.OnCoinCountReset -= ResetCoinCount;
    }

    private void IncrementCoinCount()
    {
        coinCount++;
        coinText.text = $" x {coinCount}";
    }

    private void ResetCoinCount()
    {
        coinCount = 0; // Reset the coin count
        UpdateCoinUI();
    }

    private void UpdateCoinUI()
    {
        coinText.text = $" x {coinCount}";
    }
}
