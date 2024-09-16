using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawnPoint : MonoBehaviour
{
    private Transform playerRP;



    // Start is called before the first frame update
    void Awake()
    {
        playerRP = GameObject.FindGameObjectWithTag("Player").transform;

        if(playerRP != null)
        {
            playerRP.position = transform.position;
        }
        else
        {
            Debug.Log("Obejct Not found!");
        }
    }

    
}
