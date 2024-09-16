using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class VirtualCamera : MonoBehaviour
{
    private VirtualCamera instance;

    CinemachineVirtualCamera virtualCamera;

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
        virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        //DontDestroyOnLoad(instance);
    }
    // Start is called before the first frame update
    void Start()
    {
        
        if(virtualCamera != null)
        {
            virtualCamera.Follow = PlayerController.FindObjectOfType<PlayerController>().transform;
            virtualCamera.LookAt = PlayerController.FindObjectOfType<PlayerController>().transform;
        }
        else
        {
            //return;
            Debug.Log("Nada encontrado");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
