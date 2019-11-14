using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Citizen : NPC
{
    public enum CITIZENSTATE
    {
        IDLE,
        WALK,
        RUN,
        DIE,
        DOWN
    }
    // Start is called before the first frame update
    public CITIZENSTATE citizenState = CITIZENSTATE.IDLE;
    public Animator anim;

    private float patternChangeTimer;
    private float patternChangeInterval;
    private float runawayTime = 10.0f;
    private float respawnTime = 5.0f;
    //HumanCtr
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

    void Start()
    {
        base.NPCInit();
        base.Rising += new RiseEventHandler(SetRunAway);
        base.Down += new DownEventHandler(SetDown);
        base.Respawn += new RespawnEventHandler(SetRespawn);

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
    void ActivityByState()
    {
        switch (citizenState)
        {
            case CITIZENSTATE.IDLE:
                break;
            case CITIZENSTATE.WALK:
                Raycast();
                Move();
                break;
            case CITIZENSTATE.RUN:
                RunAway();
                break;
            case CITIZENSTATE.DIE:
                break;
        }
    }
    //HumanCtr
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
    //
    protected override void Die() //리스폰 필요
    {
        isDie = true;
        citizenState = CITIZENSTATE.DIE;
        GetComponent<Rigidbody>().isKinematic = false;
    }

    void TimerCheck()
    {
        patternChangeTimer += Time.deltaTime;

        switch (citizenState)
        {
            case CITIZENSTATE.IDLE:
            case CITIZENSTATE.WALK:
                if (DectectedPlayerAttack())
                {
                    SetRunAway();
                }
                PatternChange(patternChangeInterval);
                patternChangeInterval = Random.Range(3.0f, 500.0f);
                break;
            case CITIZENSTATE.RUN:
                if (DectectedPlayerAttack())
                {
                    SetRunAway();
                }
                PatternChange(runawayTime);
                break;
            case CITIZENSTATE.DIE:
                break;
            case CITIZENSTATE.DOWN:
                break;
            default:
                break;
        }
        /*if (citizenState == CITIZENSTATE.DIE || citizenState == CITIZENSTATE.DOWN)
        {
            patternChangeTimer += Time.deltaTime;
            if(patternChangeTimer > respawnTime)
            {
                patternChangeTimer = 0;
                citizenState = CITIZENSTATE.IDLE;
            }
            return;
        }
            
        patternChangeTimer += Time.deltaTime;
        
        if (DectectedPlayerAttack())
        {
            SetRunAway();
        }
        if(citizenState == CITIZENSTATE.RUN)
        {
            PatternChange(runawayTime);
        }
        else
        {
            PatternChange(patternChangeInterval);
            patternChangeInterval = Random.Range(3.0f, 500.0f);
        }
         */   
    }
    void PatternChange(float patternChangeInterval)
    {
        if (patternChangeTimer > patternChangeInterval)
        {
            patternChangeTimer = 0.0f;

            switch (citizenState)
            {
                case CITIZENSTATE.IDLE:
                    UpdateTargetDirection();
                    citizenState = CITIZENSTATE.WALK;
                    break;
                case CITIZENSTATE.WALK:
                case CITIZENSTATE.RUN:
                    //가까운 목적지 찾기 해야함
                    citizenState = CITIZENSTATE.IDLE;
                    break;
            }
        }
    }
    void SetRunAway()
    {
        if (citizenState == CITIZENSTATE.RUN)
            return;
        transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
        transform.Rotate(0, 180, 0);
        //GetComponent<Rigidbody>().isKinematic = false;
        //GetComponent<BoxCollider>().enabled = true;
        citizenState = CITIZENSTATE.RUN;
        patternChangeTimer = 0.0f;
    }
    void SetDown()
    {
        //GetComponent<Rigidbody>().isKinematic = true;
        //GetComponent<BoxCollider>().enabled = false;
        citizenState = CITIZENSTATE.DOWN;
        print("Down");
    }
    void SetRespawn()
    {
        patternChangeTimer = 0;
        isDie = false;
        hp = 100;
        citizenState = CITIZENSTATE.IDLE;
        print("Citizen Respawn");
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Wall" && citizenState == CITIZENSTATE.RUN)
        {
            print("collision");
            transform.Rotate(0, Random.Range(90, 270), 0);
        }
        //UpdateTargetDirection();\
        
    }
}