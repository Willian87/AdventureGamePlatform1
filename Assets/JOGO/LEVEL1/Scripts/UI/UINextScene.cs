using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINextScene : MonoBehaviour
{

    [SerializeField] Button _nextScene;

    void Start()
    {
        _nextScene.onClick.AddListener(StartNewScene);
    }

    private void StartNewScene()
    {
        SceneTransition.instance.LoadNextScene();
    }
}
