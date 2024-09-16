using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="PlayerStats", menuName = "Scriptable Objects/PlayerData", order = 0)]

public class PlayerStats : ScriptableObject
{
    public string playerName;
    public int maxHealth = 100;
    public float moveSpeed = 5f;

}
