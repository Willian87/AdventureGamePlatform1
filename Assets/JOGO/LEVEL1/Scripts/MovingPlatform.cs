using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform movingPlatform;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private float speed = 1f;
    float direction = 1f;

    // Update is called once per frame
    void Update()
    {
        PlatformMoving();
    }

    void PlatformMoving()
    {
        Vector2 target = CurrentMovementTarget();

        movingPlatform.position = Vector2.Lerp(movingPlatform.position, target, speed * Time.deltaTime);

        float distance = (target - (Vector2)movingPlatform.position).magnitude;

        if (distance <= 0.1f)
        {
            direction *= -1f;
        }
    }
    Vector2 CurrentMovementTarget()
    {
        if (direction == 1)
        {
            return startPoint.position;
        }
        else
        {
            return endPoint.position;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(startPoint.position, movingPlatform.position);
        Gizmos.DrawLine(movingPlatform.position, endPoint.position);
    }
}
