using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CarCtr : MonoBehaviour
{
    Vector3 destination;
    public bool isDestReached = true;

    public float maxSpeed;
    float curSpeed;
    float targetSpeed;
    public float rotSpeed;

    public LayerMask collisionLayer;
    RaycastHit hit;
    float distToObstacle = Mathf.Infinity;

    TrafficLight trafficLight = null;

    Rigidbody rbody;

    void Awake()
    {
        rbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        //destination = transform.position;
    }
    
    void FixedUpdate()
    {
        Raycast();
        MoveCar();
    }

    void Raycast()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1f, collisionLayer))
        {
            if(hit.transform.tag == "TrafficLight")
            {
                if (trafficLight == null)
                    trafficLight = hit.transform.GetComponent<TrafficLight>();

                if (Vector3.Dot(transform.forward, hit.transform.forward) < -0.8f && 
                    trafficLight.signalColor == TrafficLight.SignalColor.SC_Red)
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
                trafficLight = null;
            }           
        }
        else
        {
            distToObstacle = Mathf.Infinity;
            trafficLight = null;
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
            Debug.DrawRay(transform.position, transform.forward * 1f, Color.blue);
        }
    }

    void MoveCar()
    {
        if (isDestReached)
        {
            return;
        }

        Vector3 dir = destination - transform.position;

        // 속도조절
        targetSpeed = Mathf.Clamp(distToObstacle-1, 0, 1) * maxSpeed;

        if (targetSpeed < curSpeed)
        {
            curSpeed -= 4 * Time.deltaTime;
        }
        else
        {
            curSpeed += 2 * Time.deltaTime;
        }
        curSpeed = Mathf.Clamp(curSpeed, 0, maxSpeed);

        // 실제 회전 및 이동
        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 0.2f);
        //transform.Translate(transform.forward * curSpeed * Time.deltaTime, Space.World);

        dir.y = 0;
        rbody.MoveRotation ( Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 0.2f));
        
        float angle = Vector3.Angle(transform.forward, dir.normalized) / 10;
        angle = Mathf.Clamp(angle, 1, 10);
        rbody.velocity = transform.forward * curSpeed * 50 * Time.deltaTime * (1/angle);
    }

    public void SetDestination(Vector3 pos)
    {
        destination = pos;
        isDestReached = false;
    }

    public void StopCar()
    {
        curSpeed = 0;
        destination = transform.position;
        isDestReached = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(destination, 0.25f);
        Handles.Label(destination, "destination");

        //Gizmos.DrawWireSphere(transform.position, 0.25f);
        //Handles.Label(transform.position, "mypos");
    }
}
