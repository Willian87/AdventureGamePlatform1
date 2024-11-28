//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using TMPro;

//public class HeartUI : MonoBehaviour
//{
//    [SerializeField] private TextMeshProUGUI heartText;
//    int heartCount;

//    private void OnEnable()
//    {
//        Heart.OnHeartCollected += IncrementHeartCount;
//    }

//    void IncrementHeartCount()
//    {
//        heartCount++;
//        heartText.text = $" x {heartCount}";
//    }


//}

using UnityEngine;
using TMPro;

public class HeartUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI heartText;

    public void SetHeartCount(int count)
    {
        heartText.text = $" x {count}";
    }
}
