using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreen : MonoBehaviour
{
    public GameObject pauseMenuUI;  // Reference to the Pause Menu UI
    private bool isPaused = false;  // Track if the game is paused

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
        ResumeGame();
    }

    private void Start()
    {
        // Make sure the pause menu is initially inactive
        pauseMenuUI.SetActive(false);
    }

    void Update()
    {
        // Toggle pause state when the player presses the 'Escape' key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();

            }
            else
            {
                PauseGame();

            }
        }
    }

    // Pause the game and display the pause menu
    public void PauseGame()
    {
        isPaused = true;
        AudioListener.pause = true;
        pauseMenuUI.SetActive(true);  // Show the pause menu
        Time.timeScale = 0f;  // Stop the game time (freeze gameplay)
        

    }

    // Resume the game and hide the pause menu
    public void ResumeGame()
    {
        isPaused = false;
        AudioListener.pause = false;
        pauseMenuUI.SetActive(false);  // Hide the pause menu
        Time.timeScale = 1f;  // Resume the game time
        

    }

    // Restart the current level/scene
    public void RestartGame()
    {
        ResumeGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);  // Reload the current scene
    }

    // Quit the game
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();  // Quit the application (works in a build, not the editor)
    }
}
