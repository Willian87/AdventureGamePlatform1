using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    public GameObject playerPrefab;
    public PlayerCombat player;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        DontDestroyOnLoad(instance);
    }
    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerCombat>();

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            SavePlayer();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadGame();
        }
    }

    public void SavePlayer()
    {
        SaveLoadSystem.SavePlayerData(player);
    }

    public void LoadGame()
    {
        
        PlayerData data = SaveLoadSystem.LoadPlayerData();
        
        if(data != null)
        {
            StartCoroutine(LoadSceneAndRestorePlayerData(data));
        }
        
    }

    private IEnumerator LoadSceneAndRestorePlayerData(PlayerData data)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(data.sceneToBeLoaded);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        player = FindObjectOfType<PlayerCombat>();

        if(playerPrefab != null)
        {
            GameObject playerObject = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            player = playerObject.GetComponent<PlayerCombat>();

            //player.currentHealth = data.health;

            Vector3 position;
            position.x = data.position[0];
            position.y = data.position[1];
            position.z = data.position[2];

            player.transform.position = position;

            SceneManager.LoadScene(data.sceneToBeLoaded);
        }
    }
}
