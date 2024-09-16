//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;



//public class SceneTransition : MonoBehaviour
//{
//    public static SceneTransition instance;

//    private void Awake()
//    {
//        if(instance == null)
//        {
//            instance = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//        DontDestroyOnLoad(instance);
//    }
//    public enum Scene
//    {
//        Menu,
//        LoadingScene,
//        Level01,
//        Level01Underground
//    }

//    public void LoadScene(Scene scene)
//    {
//        SceneManager.LoadScene(scene.ToString());
//    }

//    public void LoadNewGame()
//    {
//        SceneManager.LoadScene(Scene.LoadingScene.ToString());
//    } 

//    public void LoadMainMenu()
//    {
//        SceneManager.LoadScene(Scene.Menu.ToString());
//    }

//    public void LoadNextScene()
//    {
//        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
//    }

//    public void LoadPreviousScene()
//    {
//        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
//    }

//    public void LoadScene01()
//    {
//        SceneManager.LoadScene(1);
//    }

//    public void LoadScene02()
//    {
//        SceneManager.LoadScene(2);
//    }

//    public void LoadSavedGame()
//    {
//        PlayerData data = SaveLoadSystem.LoadPlayerData();
//        if (data != null)
//        {
//            StartCoroutine(LoadSavedGameScene(data));
//        }
//    }

//    private IEnumerator LoadSavedGameScene(PlayerData data)
//    {
//        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(data.sceneToBeLoaded);

//        while (!asyncLoad.isDone)
//        {
//            yield return null;
//        }

//        GameManager gameManager = FindObjectOfType<GameManager>();
//        if (gameManager != null)
//        {
//            gameManager.LoadGame(); // Load game data after the scene is loaded

//        }
//    }
//}

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    // Singleton instance
    public static SceneTransition instance { get; private set; }

    // Enum to manage scene names
    public enum Scene
    {
        MainMenu,
        LoadingScreen,
        Level01,
        Level02,
        Level03,
        Level04
    }

    private void Awake()
    {
        // Ensure only one instance of SceneTransition exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Load a scene by name
    public void LoadScene(Scene scene)
    {
        StartCoroutine(LoadSceneAsync(scene.ToString()));
    }

    // Load the main menu
    public void LoadMainMenu()
    {
        LoadScene(Scene.MainMenu);
    }

    // Load the next scene based on build index
    public void LoadNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            StartCoroutine(LoadSceneAsync(nextSceneIndex));
        }
        else
        {
            Debug.LogWarning("No more scenes to load.");
        }
    }

    // Load the previous scene based on build index
    public void LoadPreviousScene()
    {
        int prevSceneIndex = SceneManager.GetActiveScene().buildIndex - 1;
        if (prevSceneIndex >= 0)
        {
            StartCoroutine(LoadSceneAsync(prevSceneIndex));
        }
        else
        {
            Debug.LogWarning("No previous scene to load.");
        }
    }

    // Load a saved game scene
    public void LoadSavedGame()
    {
        PlayerData data = SaveLoadSystem.LoadPlayerData();
        if (data != null)
        {
            StartCoroutine(LoadSavedGameScene(data));
        }
        else
        {
            Debug.LogError("No saved game data found.");
        }
    }

    // Coroutine to load a scene asynchronously by name
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Optional: Add a loading screen or progress bar here

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    // Coroutine to load a scene asynchronously by index
    private IEnumerator LoadSceneAsync(int sceneIndex)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);

        // Optional: Add a loading screen or progress bar here

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    // Coroutine to load a saved game scene
    private IEnumerator LoadSavedGameScene(PlayerData data)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(data.sceneToBeLoaded);

        // Optional: Add a loading screen or progress bar here

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Ensure GameManager is properly initialized
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.LoadGame();
        }
        else
        {
            Debug.LogError("GameManager not found in the scene.");
        }
    }
}

