using UnityEngine;
using Cinemachine;

public class BossFightCameraTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera bossFightCamera;
    [SerializeField] private GameObject bossHealthBar;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            bossFightCamera.Priority = 20; // Sets higher priority
            bossHealthBar.SetActive(true); // Shows boss health
            Debug.Log("Freez Motherfucker");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            bossFightCamera.Priority = 5; // Lowers priority to exit
            bossHealthBar.SetActive(false); // Hides boss health
        }
    }
}

