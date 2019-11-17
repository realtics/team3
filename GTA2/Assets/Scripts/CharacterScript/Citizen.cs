using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Citizen : NPC
{
    public enum CitizenState
    {
        IDLE,
        WALK,
        RUN,
        DIE,
        DOWN
    }
    // Start is called before the first frame update
    public CitizenState citizenState = CitizenState.IDLE;
    public Animator anim;

    private float patternChangeTimer;
    private float patternChangeInterval;
    private float runawayTime = 10.0f;
    
    //HumanCtr스크립트 참조 코드
    private Vector3 destination;
    private RaycastHit hit;
    private float distToObstacle = Mathf.Infinity;
    private TrafficLight trafficLight = null;
    
    public bool isDestReached = true;
    public LayerMask collisionLayer;

    Rigidbody rbody;

    private void Awake()
    {
        rbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        base.NPCInit();

        patternChangeInterval = Random.Range(3.0f, 500.0f);
        patternChangeTimer = patternChangeInterval;
    }
    // Start is called before the first frame update
    private void Update()
    {
        anim.SetInteger("CitizenState", (int)citizenState);
        base.NPCUpdate();

        TimerCheck();
        ActivityByState();
    }
    private void ActivityByState()
    {
        switch (citizenState)
        {
            case CitizenState.IDLE:
                break;
            case CitizenState.WALK:
                Raycast();
                Move();
                break;
            case CitizenState.RUN:
                RunAway();
                break;
            case CitizenState.DIE:
                break;
        }
    }

    #region RefHumanCtr
    protected override void Move()
    {
        if (isDestReached)
            return;

        Vector3 dir = destination - transform.position;

        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 0.4f);

        if (distToObstacle != Mathf.Infinity)
            return;

        transform.Translate(transform.forward * moveSpeed * Time.deltaTime, Space.World);
    }
    void Raycast()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit, 0.5f, collisionLayer))
        {
            if (hit.transform.tag == "TrafficLight")
            {
                if (trafficLight == null)
                    trafficLight = hit.transform.GetComponent<TrafficLight>();

                if (trafficLight.signalColor == TrafficLight.SignalColor.SC_Green)
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
        Handles.Label(destination, "destination");
    }
    #endregion
    protected override void Die() //리스폰 필요
    {
        if (!isDie)
        {
            playert.GetComponent<Player>().money += 10;
        }

        isDie = true;
        citizenState = CitizenState.DIE;
        GetComponent<Rigidbody>().isKinematic = false;
    }

    //TODO : IDLE Timer, Walk Timer 따로만들기
    private void TimerCheck()
    {
        patternChangeTimer += Time.deltaTime;

        switch (citizenState)
        {
            case CitizenState.IDLE:
            case CitizenState.WALK:
                if (DectectedPlayerAttack())
                {
                    citizenState = CitizenState.RUN;
                }
                PatternChange(patternChangeInterval);
                patternChangeInterval = Random.Range(3.0f, 500.0f);
                break;
            case CitizenState.RUN:
                if (DectectedPlayerAttack())
                {
                    citizenState = CitizenState.RUN;
                }
                PatternChange(runawayTime);
                break;
            case CitizenState.DIE:
                break;
            case CitizenState.DOWN:
                break;
            default:
                break;
        }
    }
    private void PatternChange(float patternChangeInterval)
    {
        if (patternChangeTimer > patternChangeInterval)
        {
            patternChangeTimer = 0.0f;

            switch (citizenState)
            {
                case CitizenState.IDLE:
                    UpdateTargetDirection();
                    citizenState = CitizenState.WALK;
                    break;
                case CitizenState.WALK:
                case CitizenState.RUN:
                    //가까운 목적지 찾기 해야함
                    citizenState = CitizenState.IDLE;
                    break;
            }
        }
    }
    #region override method
    public override void Down()
    {
        citizenState = CitizenState.DOWN;
    }

    public override void Rising()
    {
        if (citizenState == CitizenState.RUN)
            return;
        transform.LookAt(new Vector3(playert.transform.position.x, transform.position.y, playert.transform.position.z));
        transform.Rotate(0, 180, 0);
        citizenState = CitizenState.RUN;
        patternChangeTimer = 0.0f;
    }

    public override void Respawn()
    {
        patternChangeTimer = 0;
        isDie = false;
        hp = 100;
        citizenState = CitizenState.IDLE;
        GameObject.FindWithTag("SpawnManager").GetComponent<SpawnManager>().NPCRepositioning(this);
        print("Citizen Respawn");
    }
    #endregion
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Wall" && citizenState == CitizenState.RUN)
        {
            print("collision");
            transform.Rotate(0, Random.Range(90, 270), 0);
        }
    }

    
}