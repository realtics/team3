using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(CarManager))]
public class CarAi : MonoBehaviour
{
    public CarManager carManager;
	public CarPathManager carPathManager;

    public enum AiState
    {
        normal, chase, chaseR, chaseL, chaseF, chaseBlock, evade
    }
    public AiState aiState = AiState.normal;
    public bool isPolice = false;
    public Transform chaseTarget;

    Vector3 destination;
    public float maxSpdMultiplier = 1.0f;
    float targetSpeed;

    public LayerMask collisionLayer;
    RaycastHit hit;
    float distToObstacle = Mathf.Infinity;

	float h = 0;
	float v = 0;

    void OnEnable()
    {
        aiState = AiState.normal;
		chaseTarget = null;

        SetAiMaxSpeedMultiplier();

		StopAllCoroutines();
		StartCoroutine(RaycastCor());
		StartCoroutine(TryDetour());

		if(isPolice)
			StartCoroutine(CarChaseAI());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    public void FixedLoop()
    {
        Raycast();

		CarMoveAI();
	}

    void SetAiMaxSpeedMultiplier()
    {
        switch (aiState)
        {
            case AiState.normal:
                maxSpdMultiplier = 0.4f;
                break;
            case AiState.chase:
                maxSpdMultiplier = 1.4f;
                break;
            case AiState.chaseR:
            case AiState.chaseL:
                maxSpdMultiplier = 1.6f;
                break;
            case AiState.chaseF:
                maxSpdMultiplier = 1.6f;
                break;
            case AiState.chaseBlock:
                maxSpdMultiplier = 1.0f;
                break;
            case AiState.evade:
                maxSpdMultiplier = 1.2f;
                break;
            default:
                maxSpdMultiplier = 1.0f;
                break;
        }
		//
		maxSpdMultiplier = Mathf.Clamp(maxSpdMultiplier, 0, 1.0f + (WantedLevel.instance.level * 0.2f));
    }

    IEnumerator RaycastCor()
    {
        while (true)
        {
            Raycast();
            yield return new WaitForSeconds(0.2f);
        }
    }

    void Raycast()
    {
        distToObstacle = Mathf.Infinity;

		Vector3 rayDirection = Quaternion.Euler(0, h * 20, 0) * transform.forward;

		if(Physics.SphereCast(transform.position + (transform.forward * 0.2f), 0.2f, transform.forward, out hit, 1.5f, collisionLayer))
		//if (Physics.Raycast(transform.position, rayDirection, out hit, 1.5f, collisionLayer))
        {
            if (hit.transform.tag == "TrafficLight")
            {
                if (aiState != AiState.evade &&
                    Vector3.Dot(transform.forward, hit.transform.forward) < -0.9f)
                {
                    distToObstacle = hit.distance;
                }
            }
            else
            {
                distToObstacle = hit.distance;
			}
        }

        DrawRaycastDebugLine(rayDirection);
    }

    void DrawRaycastDebugLine(Vector3 rayDirection)
    {
        if (distToObstacle < Mathf.Infinity)
        {
            DebugX.DrawRay(transform.position, rayDirection * hit.distance, Color.red);
        }
        else
        {
			DebugX.DrawRay(transform.position, rayDirection * 1.5f, Color.blue);
        }
    }

	IEnumerator TryDetour()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.25f);

			if (aiState != AiState.normal)
				continue;

			if (Mathf.Abs(h) > 0.1f)
				continue;

			if (distToObstacle == Mathf.Infinity)
				continue;

			carPathManager.ChangeLaneIfPossible();
		}		
	}

	IEnumerator CarChaseAI()
    {
		while (true)
		{
			yield return new WaitForSeconds(0.1f);

            if (carManager.carState != CarManager.CarState.controlledByAi)
                continue;

            if(aiState == AiState.normal && chaseTarget == null)
            {
                SetChaseTarget();
            }                

            if (chaseTarget == null)
            {
                StopChase();
            }
            else if(aiState != AiState.normal)
            {
                if (chaseTarget.tag == "Car")
                {
                    ChaseStateLoopForCarTarget();
                }
                else
                {
                    ChaseStateLoopForHumanTarget();
                }

                SetAiMaxSpeedMultiplier();
            }
        }
    }

    void ChaseStateLoopForCarTarget()
    {
        switch (aiState)
        {
            case AiState.chaseL:
                {
                    destination = chaseTarget.position + (chaseTarget.forward * 1.5f) + (chaseTarget.right * -1.5f);
                }
                break;
            case AiState.chaseR:
                {
                    destination = chaseTarget.position + (chaseTarget.forward * 1.5f) + (chaseTarget.right * 1.5f);
                }
                break;
            case AiState.chaseF:
            case AiState.chaseBlock:
                {
                    destination = chaseTarget.position + (chaseTarget.forward * 2f);
                }
                break;
            default:
                {
                    destination = chaseTarget.position;
                }
                break;
        }

        Vector3 dir = destination - transform.position;
        float dist = dir.magnitude;

        if (dist > 15)
        {
			StopChase();

            return;
        }
		else if(dist < 4f && carManager.movement.curSpeed < 15)
		{
			carManager.passengerManager.GetOffTheCar(0);
			carManager.passengerManager.GetOffTheCar(1);

			return;
		}

        switch (aiState)
        {
            case AiState.chase:
                {
                    if (dist < 2.7f)
                    {
                        if (Random.Range(0, 2) == 0)
                        {
                            aiState = AiState.chaseR;
                        }
                        else
                        {
                            aiState = AiState.chaseL;
                        }
                    }
                }
                break;
            case AiState.chaseL:
            case AiState.chaseR:
                {
                    if (dist < 1)
                    {
                        aiState = AiState.chaseF;
                    }
                    else if (dist > 4)
                    {
                        aiState = AiState.chase;
                    }
                }
                break;
            case AiState.chaseF:
                {
                    if (dist > 3.5f)
                    {
                        aiState = AiState.chase;
                    }
                    else if (dist < 1)
                    {
                        aiState = AiState.chaseBlock;
                    }
                }
                break;
            case AiState.chaseBlock:
                {
                    if (dist > 3)
                    {
                        aiState = AiState.chase;
                    }
                }
                break;
        }
    }

	void ChaseStateLoopForHumanTarget()
	{
		destination = chaseTarget.position;

		Vector3 dir = destination - transform.position;
		float dist = dir.magnitude;

		if (dist > 16)
		{
			StopChase();
		}
		else if (dist < 2.5f && carManager.movement.curSpeed < 15)
		{
			carManager.passengerManager.GetOffTheCar(0);
			carManager.passengerManager.GetOffTheCar(1);
		}
	}

	void CarMoveAI()
    {
		if(carManager.carType == CarManager.CarType.ambulance && 
			GameManager.Instance.ambulanceTargetNPC != null)
		{
			Vector3 dist = (GameManager.Instance.ambulanceTargetNPC.transform.position - transform.position);
			if (Mathf.Abs(dist.x) < 0.1f || Mathf.Abs(dist.z) < 0.1f)
			{
				carManager.carState = CarManager.CarState.idle;
				carManager.passengerManager.GetOffTheCar(0);
				carManager.passengerManager.GetOffTheCar(1);
				return;
			}
		}
			
        h = 0;
        v = 0;

        Vector3 dir = destination - transform.position;
        dir.y = 0;

        if (aiState != AiState.chaseBlock)
        {
            float angle = Vector3.SignedAngle(dir, transform.forward, Vector3.up) / 30;
            angle = Mathf.Clamp(angle, -1.5f, 1.5f);

            h = -angle;
        }

        if (aiState == AiState.chaseBlock)
        {
            targetSpeed = 0;
        }
        else
        {
            targetSpeed = Mathf.Clamp(distToObstacle - 1, 0, 1) * carManager.movement.data.maxSpeed * maxSpdMultiplier * carManager.damage.maxSpdMultiplier;
        }

        targetSpeed *= (1 - (Mathf.Abs(h) / 3f));

        if (targetSpeed < carManager.movement.curSpeed)
        {
            v = -3;
        }
        else if (targetSpeed * 0.9f > carManager.movement.curSpeed)
        {
            v = 1;
        }
        else
        {
            v = 0;
        }

        carManager.input.AiInput(h, v);
        carManager.movement.MoveCarByInput();
    }

    public void SetDestination(Vector3 pos)
    {
        destination = pos;
    }

    void SetChaseTarget()
    {
        if (WantedLevel.instance.level < 1 ||
            (transform.position - GameManager.Instance.player.transform.position).sqrMagnitude > 100)
        {
            chaseTarget = null;
            return;
        }

        if (GameManager.Instance.playerCar != null)
		{
			chaseTarget = GameManager.Instance.playerCar.transform;
		}
		else
		{
            chaseTarget = GameManager.Instance.player.transform;
		}

        aiState = AiState.chase;
        carManager.effects.TurnOnSiren();
    }

	public void StopChase()
	{
        if (aiState == AiState.normal && chaseTarget == null)
            return;

        carManager.effects.TurnOffSiren(People.PeopleType.None, 0);

		chaseTarget = null;
		aiState = AiState.normal;
	}

    void OnDrawGizmosSelected()
    {
		Gizmos.color = Color.red;
		//Gizmos.DrawWireSphere(destination, 0.25f);
		Gizmos.DrawWireSphere(destination, 1);
        //Handles.Label(destination, "destination");
        //Handles.Label(transform.position + Vector3.right, "spd: " + curSpeed);
    }
}
