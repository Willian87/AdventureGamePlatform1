using UnityEngine;
using UnityEngine.SceneManagement;


[System.Serializable]
public class PlayerData
{
    public int health;
    public float[] position;
    public string sceneToBeLoaded;
    public PlayerData(PlayerCombat player)
    {
        //health = player.currentHealth;

        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;

        sceneToBeLoaded = SceneManager.GetActiveScene().name;
    }
}