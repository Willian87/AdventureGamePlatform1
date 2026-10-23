using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    static bool isLoading;
    public GameObject playerPrefab;
    public PlayerCombat player;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) SavePlayer();
        if (Input.GetKeyDown(KeyCode.L)) LoadGame();
    }

    public void SavePlayer()
    {
        if (isLoading) return;
        player = PlayerCombat.instance;
        if (player == null || !player.gameObject.activeInHierarchy || player.currentHealth <= 0 ||
            !SceneManager.GetActiveScene().name.StartsWith("Level"))
        {
            Debug.LogWarning("Cannot save without a living player in a level.");
            return;
        }
        SaveLoadSystem.SavePlayerData(player);
    }

    public void LoadGame()
    {
        PlayerData data = SaveLoadSystem.LoadPlayerData();
        if (data != null) StartCoroutine(RestoreSavedGame(data));
    }

    // Shared by the menu and in-game load button: load exactly once, then restore.
    public static IEnumerator RestoreSavedGame(PlayerData data)
    {
        if (isLoading) yield break;
        if (data == null || data.position == null || data.position.Length != 3 ||
            string.IsNullOrEmpty(data.sceneToBeLoaded) || !data.sceneToBeLoaded.StartsWith("Level") ||
            !Application.CanStreamedLevelBeLoaded(data.sceneToBeLoaded))
        {
            Debug.LogError("Save data has no valid level or player position.");
            yield break;
        }
        foreach (float coordinate in data.position)
        {
            if (float.IsNaN(coordinate) || float.IsInfinity(coordinate))
            {
                Debug.LogError("Save data contains an invalid player position.");
                yield break;
            }
        }

        isLoading = true;
        try
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
            // Only Level01 contains the player, managers and HUD. Bootstrap them
            // when loading a later saved level directly from the main menu.
            if (PlayerCombat.instance == null && data.sceneToBeLoaded != "Level01")
            {
                yield return SceneManager.LoadSceneAsync("Level01");
                yield return null;
            }
            yield return SceneManager.LoadSceneAsync(data.sceneToBeLoaded);
            // Let the new scene's Start methods finish before applying saved state.
            yield return null;
            PlayerCombat restoredPlayer = PlayerCombat.instance;
            if (restoredPlayer == null && instance != null && instance.playerPrefab != null)
            {
                Instantiate(instance.playerPrefab);
                yield return null;
                restoredPlayer = PlayerCombat.instance;
            }
            if (restoredPlayer == null)
            {
                Debug.LogError("The saved level has no player to restore.");
                yield break;
            }
            restoredPlayer.RestoreState(new Vector3(data.position[0], data.position[1], data.position[2]),
                data.health > 0 ? data.health : restoredPlayer.maxHealth);
            if (instance != null) instance.player = restoredPlayer;
        }
        finally
        {
            isLoading = false;
        }
    }
}
