using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTest : MonoBehaviour
{

    public EnemyData enemyData;

    public int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        Initializing();
    }

    public void Initializing()
    {
        currentHealth = enemyData.maxHealth;

        
    }
}
