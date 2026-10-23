using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverScreen; // Reference to the Game Over UI panel

    private bool isGameOver = false;

    public GameObject GameOverScreen => gameOverScreen;

    private void Awake()
    {
        SceneManager.sceneLoaded += ResetForScene;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= ResetForScene;
    }

    private void ResetForScene(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        isGameOver = false;
        if (gameOverScreen != null) gameOverScreen.SetActive(false);
        AudioListener.pause = false;
        Time.timeScale = 1f;
    }

    // Method to trigger Game Over
    public void TriggerGameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

        // Activate Game Over screen
        if (gameOverScreen != null) gameOverScreen.SetActive(true);

        // Pause the game
        Time.timeScale = 0f;
    }

    // Retry the current level
    public void RetryGame()
    {
        // Unpause the game
        AudioListener.pause = false;
        Time.timeScale = 1f;

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Quit the game
    public void QuitGame()
    {
        // Unpause the game (optional for returning to menu)
        AudioListener.pause = false;
        Time.timeScale = 1f;

        // Load the main menu or quit the application
        SceneManager.LoadScene("MainMenu"); // Replace "MainMenu" with your menu scene name
        // Alternatively, Application.Quit() can be used for standalone builds
    }
}
