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
    }

    private void OnDisable()
    {
        
    }

    private void IncrementCoinCount()
    {
        coinCount++;
        coinText.text = $" x {coinCount}";
    }
}
