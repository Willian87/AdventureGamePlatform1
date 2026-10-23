using UnityEngine;

public class PlayerRespawnPoint : MonoBehaviour
{
    private void Start()
    {
        // Awake ran before all scene objects existed; Start can find the surviving player.
        PlayerCombat player = PlayerCombat.instance;
        if (GameController.instance != null)
            GameController.instance.lastCheckPoint = transform.position;
        if (player != null)
            player.transform.position = transform.position;
    }
}
