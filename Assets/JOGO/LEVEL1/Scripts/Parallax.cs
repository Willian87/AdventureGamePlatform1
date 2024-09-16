using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float startPosition;
    private float length;

    private Transform cam;

    [SerializeField] private float parallaxEfect;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        float resPosition = cam.position.x * (1 - parallaxEfect);
        float distance = cam.position.x * parallaxEfect;
        transform.position = new Vector3(startPosition + distance, transform.position.y, transform.position.z);

        if(resPosition > startPosition + length)
        {
            startPosition += length;
        }
        else if(resPosition < startPosition - length )
        {
            startPosition -= length;
        }
    }
}
