using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameMenu : MonoBehaviour
{
    private static UIGameMenu instance;
    [SerializeField] Button _NewGame;
    [SerializeField] Button _LoadGame;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        _NewGame.onClick.AddListener(LoadMainMenu);

    }

    private void LoadMainMenu()
    {
        SceneTransition.instance.LoadMainMenu();
    }
    
}
