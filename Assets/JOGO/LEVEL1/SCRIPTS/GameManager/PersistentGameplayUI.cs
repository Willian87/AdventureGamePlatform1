using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

// Later levels have no player HUD. Keep its original canvases and button targets alive.
public class PersistentGameplayUI : MonoBehaviour
{
    public static void PreserveHUD()
    {
        foreach (PlayerHealthBar health in FindObjectsOfType<PlayerHealthBar>(true)) Preserve(health.gameObject);
        foreach (HeartUI hearts in FindObjectsOfType<HeartUI>(true)) Preserve(hearts.gameObject);
        foreach (PauseScreen pause in FindObjectsOfType<PauseScreen>(true))
        {
            Preserve(pause.gameObject);
            if (pause.pauseMenuUI != null) Preserve(pause.pauseMenuUI);
        }
        foreach (GameOverManager manager in FindObjectsOfType<GameOverManager>(true))
            if (manager.GameOverScreen != null) Preserve(manager.GameOverScreen);
        foreach (EventSystem events in FindObjectsOfType<EventSystem>(true)) Preserve(events.gameObject);
    }

    public static T FindHUD<T>() where T : Component
    {
        T fallback = null;
        foreach (T component in FindObjectsOfType<T>(true))
        {
            if (component.GetComponentInParent<PersistentGameplayUI>() != null) return component;
            fallback = component;
        }
        return fallback;
    }

    private static void Preserve(GameObject target)
    {
        GameObject root = target.transform.root.gameObject;
        if (root.GetComponent<PersistentGameplayUI>() == null)
            root.AddComponent<PersistentGameplayUI>();
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        bool gameplay = scene.name.StartsWith("Level");
        // Reloaded Level01 contains new copies of the same HUD and EventSystem.
        // Keep the originals so persistent player and manager references stay valid.
        if (gameplay)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                if (root != gameObject && root.name == gameObject.name &&
                    ((GetComponent<Canvas>() != null && root.GetComponent<Canvas>() != null) ||
                     (GetComponent<EventSystem>() != null && root.GetComponent<EventSystem>() != null) ||
                     (GetComponent<PauseScreen>() != null && root.GetComponent<PauseScreen>() != null)))
                {
                    root.SetActive(false);
                    Destroy(root);
                }
            }
        }
        gameObject.SetActive(gameplay);
    }
}
