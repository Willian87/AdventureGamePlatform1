using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverScreen; // Reference to the Game Over UI panel

    private bool isGameOver = false;

    // Method to trigger Game Over
    public void TriggerGameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

        // Activate Game Over screen
        gameOverScreen.SetActive(true);

        // Pause the game
        Time.timeScale = 0f;
    }

    // Retry the current level
    public void RetryGame()
    {
        // Unpause the game
        Time.timeScale = 1f;

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Quit the game
    public void QuitGame()
    {
        // Unpause the game (optional for returning to menu)
        Time.timeScale = 1f;

        // Load the main menu or quit the application
        SceneManager.LoadScene("Menu"); // Replace "MainMenu" with your menu scene name
        // Alternatively, Application.Quit() can be used for standalone builds
    }
}
