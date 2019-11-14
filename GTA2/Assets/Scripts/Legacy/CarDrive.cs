using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CarDrive : MonoBehaviour
{
    public bool isControlledByPlayer;

    Rigidbody rbody;
    public TrailRenderer trailLeft;
    public TrailRenderer trailRight;
    public GameObject shadow;

    public float maxSpeed;
    float curSpeed;
    public float rotSpeed;

    float inputH;
    float inputV;

    Vector3 reboundForce = Vector3.zero;

    void Awake()
    {
        rbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(isControlledByPlayer)
            PlayerInput();

        DrawSkidMark();

        shadow.transform.position = transform.position + new Vector3(0.05f, 0, -0.05f);
    }

    void FixedUpdate()
    {
        if (isControlledByPlayer)
            MoveCar();
    }

    void PlayerInput()
    {
        inputV = Input.GetAxisRaw("Vertical");
        inputH = Input.GetAxisRaw("Horizontal");
    }

    void MoveCar()
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

        transform.Rotate(0, inputH * rotSpeed * Time.deltaTime * (Mathf.Abs(curSpeed)/400), 0);
        //rbody.AddForce(transform.forward * curSpeed);
        rbody.velocity = transform.forward * curSpeed * Time.deltaTime + reboundForce;
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

    void OnDrawGizmosSelected()
    {
        if(rbody != null)
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
