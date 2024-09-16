using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine (LoadingScene());
    }

    IEnumerator LoadingScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Level01");

        while (!asyncLoad.isDone)
        {
            yield return null; 
        }
    }

}
