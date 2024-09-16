using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Scriptable Objects/EnemiesData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int maxHealth;
    public float moveSpeed;
    public int attackDamage;

    public void SetDefaults()
    {
        maxHealth = 100;
        moveSpeed = 5f;
        attackDamage = 10;
    }
}
