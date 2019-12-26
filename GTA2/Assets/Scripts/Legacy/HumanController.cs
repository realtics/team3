using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

public class HumanController : MonoBehaviour
{
    Vector3 destination;
    public bool isDestReached = true;

    public float speed;

    public LayerMask collisionLayer;
    RaycastHit hit;
    float distToObstacle = Mathf.Infinity;

    void Update()
    {
		print("이거 안씀?");
        Raycast();
        Move();
    }

    void Raycast()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit, 0.5f, collisionLayer))
        {
            if (hit.transform.tag == "TrafficLight")
            {
                if (Vector3.Dot(transform.forward, hit.transform.forward) < -0.8f)
                {
                    distToObstacle = hit.distance;
                }
                else
                {
                    distToObstacle = Mathf.Infinity;
                }
            }
            else
            {
                distToObstacle = hit.distance;
            }
        }
        else
        {
            distToObstacle = Mathf.Infinity;
        }

        DrawRaycastDebugLine();
    }

    void DrawRaycastDebugLine()
    {
        if (distToObstacle < Mathf.Infinity)
        {
            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.forward * 0.5f, Color.blue);
        }
    }

    void Move()
    {
        if (isDestReached)
            return;

        Vector3 dir = destination - transform.position;

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 0.4f);

        if (distToObstacle != Mathf.Infinity)
            return;

        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
    }

    public void SetDestination(Vector3 pos)
    {
        destination = pos;
        isDestReached = false;
    }

    public void Stop()
    {
        destination = transform.position;
        isDestReached = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(destination, 0.25f);
        //Handles.Label(destination, "destination");
    }
}
