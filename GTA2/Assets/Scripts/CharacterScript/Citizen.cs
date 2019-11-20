using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

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
    public CitizenState citizenState = CitizenState.IDLE;
    public Animator animator;

    float patternChangeTimer;
    float patternChangeInterval;
    float runawayTime = 10.0f;

    Rigidbody myRigidbody;
    float minIdleTime = 2.0f;
    float maxIdleTime = 5.0f;
    float minWalkTime = 2.0f;
    float maxWalkTime = 5.0f;

    Vector3 RunawayVector;

    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        base.NPCInit();

        patternChangeInterval = Random.Range(minIdleTime, maxIdleTime);
        patternChangeTimer = patternChangeInterval;
    }
    void Update()
    {
        animator.SetInteger("CitizenState", (int)citizenState);
        base.NPCUpdate();

        TimerCheck();
        ActivityByState();
    }
    void ActivityByState()
    {
        switch (citizenState)
        {
            case CitizenState.IDLE:
                break;
            case CitizenState.WALK:
                base.Raycast();
                base.Move();
                break;
            case CitizenState.RUN:
                base.RunAway();
                break;
            case CitizenState.DIE:
                break;
        }
    }
    void TimerCheck()
    {
        patternChangeTimer += Time.deltaTime;

        switch (citizenState)
        {
            case CitizenState.IDLE:
                if (DectectedPlayerAttack())
                {
                    RunawayVector = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
                    transform.LookAt(RunawayVector);
                    transform.Rotate(0, 180, 0);
                    citizenState = CitizenState.RUN;
                }
                PatternChange(patternChangeInterval);
                break;
            case CitizenState.WALK:
               
                if (DectectedPlayerAttack())
                {
                    RunawayVector = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
                    transform.LookAt(RunawayVector);
                    transform.Rotate(0, 180, 0);
                    citizenState = CitizenState.RUN;
                }
                PatternChange(patternChangeInterval);
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
    void PatternChange(float patternChangeInterval)
    {
        if (patternChangeTimer > patternChangeInterval)
        {
            patternChangeTimer = 0.0f;

            switch (citizenState)
            {
                case CitizenState.IDLE:
                    patternChangeInterval = Random.Range(minWalkTime, maxWalkTime);
                    citizenState = CitizenState.WALK;
                    break;
                case CitizenState.WALK:
                    patternChangeInterval = Random.Range(minIdleTime, maxIdleTime);
                    citizenState = CitizenState.IDLE;
                    break;
                case CitizenState.RUN:
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
        transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
        transform.Rotate(0, 180, 0);
        citizenState = CitizenState.RUN;
        patternChangeTimer = 0.0f;
    }
    protected override void Die() //리스폰 필요
    {
        isDie = true;
        citizenState = CitizenState.DIE;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<BoxCollider>().enabled = false;
    }
    public override void Respawn()
    {
        patternChangeTimer = 0;
        isDie = false;
        hp = 100;
        citizenState = CitizenState.IDLE;
        GameObject.FindWithTag("SpawnManager").GetComponent<SpawnManager>().NPCRepositioning(this);
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<BoxCollider>().enabled = true;
        print("Citizen Respawn");
    }
    #endregion
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall" && citizenState == CitizenState.RUN)
        {
            transform.Rotate(0, Random.Range(90, 270), 0);
        }
    }
}