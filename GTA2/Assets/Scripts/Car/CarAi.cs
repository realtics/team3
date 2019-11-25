using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

[RequireComponent(typeof(CarManager))]
public class CarAi : MonoBehaviour
{
    public CarManager carManager;

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

    void Start()
    {
        SetAiMaxSpeedMultiplier();
    }

    void OnEnable()
    {
        aiState = AiState.normal;
        SetAiMaxSpeedMultiplier();
    }

    public void FixedLoop()
    {
        Raycast();

        switch (aiState)
        {
            case AiState.normal:
            case AiState.evade:
                {
                    CarMoveAI();
                }
                break;
            case AiState.chase:
            case AiState.chaseR:
            case AiState.chaseL:
            case AiState.chaseF:
            case AiState.chaseBlock:
                {
                    CarChaseAI();
                }
                break;
        }
    }

    void SetAiMaxSpeedMultiplier()
    {
        switch (aiState)
        {
            case AiState.normal:
                maxSpdMultiplier = 0.4f;
                break;
            case AiState.chase:
                maxSpdMultiplier = 1.2f;
                break;
            case AiState.chaseR:
            case AiState.chaseL:
                maxSpdMultiplier = 1.6f;
                break;
            case AiState.chaseF:
                maxSpdMultiplier = 1.8f;
                break;
            case AiState.chaseBlock:
                maxSpdMultiplier = 1.0f;
                break;
            case AiState.evade:
                maxSpdMultiplier = 1.0f;
                break;
            default:
                maxSpdMultiplier = 1.0f;
                break;
        }
    }

    void Raycast()
    {
        distToObstacle = Mathf.Infinity;

        if (Physics.Raycast(transform.position, transform.forward, out hit, 1.5f, collisionLayer))
        {
            if (hit.transform.tag == "TrafficLight")
            {
                if (aiState != AiState.evade &&
                    Vector3.Dot(transform.forward, hit.transform.forward) < -0.8f)
                {
                    distToObstacle = hit.distance;
                }
            }
            else
            {
                distToObstacle = hit.distance;
            }
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
            Debug.DrawRay(transform.position, transform.forward * 1.5f, Color.blue);
        }
    }

    void CarChaseAI()
    {
        if (chaseTarget == null)
        {
            aiState = AiState.normal;
            return;
        }

        ChaseStateLoop();
        SetAiMaxSpeedMultiplier();
        CarMoveAI();
    }

    void ChaseStateLoop()
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

        if (dist > 16)
            aiState = AiState.normal;

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
                    else if (dist > 3.5f)
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

    void CarMoveAI()
    {
        float h = 0;
        float v = 0;

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
            targetSpeed = Mathf.Clamp(distToObstacle - 1, 0, 1) * carManager.movement.maxSpeed * maxSpdMultiplier * carManager.damage.maxSpdMultiplier;
        }

        targetSpeed *= (1 - (Mathf.Abs(h) / 2f));

        if (targetSpeed < carManager.movement.curSpeed)
        {
            v = -4;
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

    public void ChasePlayer()
    {
        if (GameManager.Instance.wantedLevel <= 0)
            return;

        if (aiState != AiState.normal)
            return;

        if (Vector3.Distance(transform.position, GameManager.Instance.player.transform.position) > 10)
            return;

        chaseTarget = GameManager.Instance.player.transform;
        aiState = AiState.chase;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(destination, 0.25f);
        Gizmos.DrawWireSphere(destination, 1);
        //Handles.Label(destination, "destination");
        //Handles.Label(transform.position + Vector3.right, "spd: " + curSpeed);
    }
}
