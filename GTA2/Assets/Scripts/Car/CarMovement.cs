﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

[RequireComponent(typeof(CarManager))]
public class CarMovement : MonoBehaviour
{
    public CarManager carManager;
	
	Rigidbody rbody;

	public CarData data;
    public float curSpeed;

    Vector3[] oldForwards = new Vector3[20];
    Vector3 reboundForce = Vector3.zero;

    void Awake()
    {
        rbody = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        carManager.OnDriverGetOff += OnDriverGetOff;
		carManager.OnDestroy += OnCarDestroy;
	}

    void OnDisable()
    {
        carManager.OnDriverGetOff -= OnDriverGetOff;
		carManager.OnDestroy -= OnCarDestroy;
	}

    public void FixedLoop()
    {
        UpdateOldForwards();
        MoveCarByInput();
    }

    void UpdateOldForwards()
    {
        for (int i = 0; i < oldForwards.Length - 1; i++)
        {
            oldForwards[i] = oldForwards[i + 1];
        }
        oldForwards[oldForwards.Length - 1] = transform.forward;
    }

    public void MoveCarByInput()
    {
        reboundForce *= 0.85f;

        curSpeed += data.acceleration * carManager.input.GetInputV() * Time.deltaTime;

        if (carManager.input.GetInputV() == 0)
        {
            curSpeed *= 0.97f;
            if (Mathf.Abs(curSpeed) < 1)
                curSpeed = 0;
        }

        curSpeed *= 1 - (Mathf.Abs(carManager.input.GetInputH()) / 70);

        float maxSpdMul = 1.0f;
        if (carManager.carState == CarManager.CarState.controlledByAi)
            maxSpdMul = carManager.ai.maxSpdMultiplier;

        float finalMaxSpd = data.maxSpeed * maxSpdMul * carManager.damage.maxSpdMultiplier;
        curSpeed = Mathf.Clamp(curSpeed, finalMaxSpd * -0.5f, finalMaxSpd);

        transform.Rotate(0, carManager.input.GetInputH() * data.rotSpeed * Time.deltaTime * (Mathf.Abs(curSpeed) / 400), 0);

        Vector3 dir = Vector3.zero;
        if (carManager.carState == CarManager.CarState.controlledByPlayer && carManager.input.GetInputH() != 0)
        {
            for (int i = 0; i < oldForwards.Length; i++)
            {
                dir += oldForwards[i];
            }
            dir.Normalize();
        }
        else
        {
            dir = transform.forward;
        }

        rbody.velocity = dir * curSpeed * Time.deltaTime + reboundForce;
    }

    void OnDriverGetOff(People.PeopleType peopleType, int idx)
    {
        curSpeed = 0;
    }

	void OnCarDestroy(bool sourceIsPlayer)
	{
		curSpeed = 0;
	}

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.CompareTag("Wall"))
        {
            curSpeed *= 0.25f;
            Vector3 inDirection = transform.forward;
            reboundForce = Vector3.Reflect(inDirection, col.contacts[0].normal) * curSpeed * 0.15f;

			float rotAmount = Vector3.SignedAngle(inDirection, reboundForce, Vector3.up);
			transform.Rotate(0, rotAmount / 5, 0);

			//DebugX.DrawRay(transform.position, transform.position - inDirection, Color.blue, 1f);
			//DebugX.DrawRay(transform.position, transform.position + reboundForce, Color.red, 1f);
        }
        else if (col.transform.CompareTag("Car"))
        {
            curSpeed *= 0.25f;
        }

		if (col.transform.CompareTag("NPC") || col.transform.CompareTag("Player"))
		{
			float hitForce = curSpeed;
			if (carManager.carType == CarManager.CarType.tank)
				hitForce *= 10;

			col.gameObject.GetComponent<People>().Runover(
					hitForce, transform.position, carManager.carState == CarManager.CarState.controlledByPlayer);
		}
		
    }
	private void OnTriggerStay(Collider other)
	{
		if (other.transform.CompareTag("NPC") || other.transform.CompareTag("Player"))
		{
			float hitForce = curSpeed;
			if (carManager.carType == CarManager.CarType.tank)
				hitForce *= 10;

			other.gameObject.GetComponent<People>().Runover(
					hitForce, transform.position, carManager.carState == CarManager.CarState.controlledByPlayer);
		}
	}
	void OnCollisionStay(Collision col)
    {
        if (col.transform.CompareTag("Wall") && Mathf.Abs(curSpeed) > 50)
        {
            curSpeed *= 0.9f;
        }
    }
}
