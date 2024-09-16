using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoints : MonoBehaviour
{
    private GameController _GC;

    private void Start()
    {
        _GC = GameObject.FindGameObjectWithTag("GC").GetComponent<GameController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player sou eu");
            this.gameObject.SetActive(false);
            _GC.lastCheckPoint = transform.position;
            
        }
    }


}


//public class Checkpoint : MonoBehaviour
//{
//    // Reference to the player's transform
//    private Transform playerTransform;

//    // Variables to store player state information
//    private Vector3 playerStartPosition;
//    private int playerHealth;

//    // Set the initial checkpoint state
//    void Start()
//    {
//        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

//        // Set the initial checkpoint position
//        playerStartPosition = playerTransform.position;
//    }

//    // Called when the player enters the checkpoint collider
//    void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("Player"))
//        {
//            // Save player state
//            SavePlayerState();

//            // Set the new respawn point
//            SetRespawnPoint();

//            // You may want to play a sound or show a visual effect to indicate the checkpoint
//            // Example: AudioManager.PlayCheckpointSound();

//            // You can also disable the checkpoint so that the player can't trigger it again
//            // gameObject.SetActive(false);
//        }
//    }

//    void SavePlayerState()
//    {
//        // Save player position
//        playerStartPosition = playerTransform.position;

//        // Save player health (adjust this based on your player's health system)
//        PlayerHealthBar playerHealthComponent = playerTransform.GetComponent<PlayerHealthBar>();
//        if (playerHealthComponent != null)
//        {
//            //playerHealth = playerHealthComponent.GetCurrentHealth();
//        }
//    }

//    void SetRespawnPoint()
//    {
//        // Set the player's respawn position to the checkpoint position
//       //SceneTransition.SetRespawnPoint(playerStartPosition);

//        // Optionally, you might want to update UI or provide feedback to the player
//        // Example: UIManager.ShowRespawnPointSetMessage();
//    }
//}

