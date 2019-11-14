using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CarCtr : MonoBehaviour
{
    Rigidbody rbody;
    public TrailRenderer trailLeft;
    public TrailRenderer trailRight;
    public GameObject shadow;

    public bool isControlledByPlayer;

    Vector3 destination;

    public float maxSpeed;
    float curSpeed;
    float targetSpeed;
    public float rotSpeed;

    public LayerMask collisionLayer;
    RaycastHit hit;
    float distToObstacle = Mathf.Infinity;
    Vector3 reboundForce = Vector3.zero;

    TrafficLight trafficLight = null;

    float inputH;
    float inputV;

    private void Awake()
    {
        rbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        DrawSkidMark();

        shadow.transform.position = transform.position + new Vector3(0.05f, 0, -0.05f);
    }

    private void FixedUpdate()
    {
        if (isControlledByPlayer)
        {
            PlayerInput();
            MoveCar();
        }
        else
        {
            Raycast();
            CarMoveAI();
        }
    }

    private void PlayerInput()
    {
        inputV = Input.GetAxisRaw("Vertical");
        inputH = Input.GetAxisRaw("Horizontal");
    }

    void DrawSkidMark()
    {
        if (inputH != 0 && curSpeed > 150)
        {
            trailLeft.emitting = true;
            trailRight.emitting = true;
        }
        else
        {
            trailLeft.emitting = false;
            trailRight.emitting = false;
        }
    }

    private void Raycast()
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

    private void DrawRaycastDebugLine()
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

    private void CarMoveAI()
    {
        Vector3 dir = destination - transform.position;

        targetSpeed = Mathf.Clamp(distToObstacle-1, 0, 1) * maxSpeed;

        if (targetSpeed < curSpeed)
        {
            curSpeed -= 200 * Time.deltaTime;
        }
        else
        {
            curSpeed += 100 * Time.deltaTime;
        }
        curSpeed = Mathf.Clamp(curSpeed, 0, maxSpeed/2);

        dir.y = 0;
        rbody.MoveRotation ( Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 0.2f));
        
        float angle = Vector3.Angle(transform.forward, dir.normalized) / 10;
        angle = Mathf.Clamp(angle, 1, 10);
        rbody.velocity = transform.forward * curSpeed * Time.deltaTime * (1/angle);
    }

    private void MoveCar()
    {
        reboundForce *= 0.85f;

        curSpeed += 100 * inputV * Time.deltaTime;

        if (inputV == 0)
        {
            curSpeed *= 0.97f;
            if (curSpeed < 1)
                curSpeed = 0;
        }

        curSpeed = Mathf.Clamp(curSpeed, (maxSpeed / 2) * -1, maxSpeed);

        transform.Rotate(0, inputH * rotSpeed * Time.deltaTime * (Mathf.Abs(curSpeed) / 400), 0);
        rbody.velocity = transform.forward * curSpeed * Time.deltaTime + reboundForce;
    }

    public void SetDestination(Vector3 pos)
    {
        destination = pos;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(destination, 0.25f);
        Handles.Label(destination, "destination");

        Handles.Label(transform.position + Vector3.right, "spd: " + curSpeed);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag == "Wall")
        {
            curSpeed *= 0.25f;
            Vector3 inDirection = transform.forward;
            reboundForce = Vector3.Reflect(inDirection, col.contacts[0].normal) * curSpeed * 0.15f;
            //Debug.DrawLine(transform.position, transform.position - inDirection, Color.blue, 1f);
            //Debug.DrawLine(transform.position, transform.position + reboundForce, Color.red, 1f);
        }
    }

    void OnCollisionStay(Collision col)
    {
        if (col.transform.tag == "Wall")
            curSpeed *= 0.9f;
    }
}
