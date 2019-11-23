using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

public class Citizen : NPC
{
    public Animator animator;

    [SerializeField]
    float patternChangeTimer;
    [SerializeField]
    float patternChangeInterval;
    float runawayTime = 5.0f;

    Rigidbody myRigidbody;
    float minIdleTime = 2.0f;
    float maxIdleTime = 5.0f;
    float minWalkTime = 2.0f;
    float maxWalkTime = 5.0f;

    public bool isRunaway{get;set;}

    Vector3 RunawayVector;

    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        patternChangeInterval = Random.Range(minIdleTime, maxIdleTime);
        patternChangeTimer = patternChangeInterval;
    }
    void Update()
    {
        animator.SetBool("isWalk", isWalk);
        //animator.SetBool("isShot", isShot);
        animator.SetBool("isPunch", isPunch);
        //animator.SetBool("isJump", isJump);
        animator.SetBool("isDie", isDie);
        animator.SetBool("isDown", isDown);
        //animator.SetBool("isStealingCar", isStealingCar);

        base.NPCUpdate();
        if (isDie || isDown)
            return;

        TimerCheck();
        ActivityByState();
    }
    void ActivityByState()
    {
        if (isRunaway)
        {
            base.RunAway();
        }
        else if (isWalk)
        {
            base.Raycast();
            base.Move();
        }
    }
    void TimerCheck()
    {
        patternChangeTimer += Time.deltaTime;

        if (DectectedPlayerAttack())
        {
            SetRunaway();
        }
        else
            PatternChange();
    }
    void SetRunaway()
    {
        patternChangeTimer = 0.0f;
        patternChangeInterval = runawayTime;
        RunawayVector = new Vector3(GameManager.Instance.player.transform.position.x, transform.position.y, GameManager.Instance.player.transform.position.z);
        transform.LookAt(RunawayVector);
        transform.Rotate(0, 180, 0);
        isRunaway = true;
        isWalk = true;
    }
    void PatternChange()
    {
        if (patternChangeTimer > patternChangeInterval)
        {
            patternChangeTimer = 0.0f;
            if (isRunaway)
            {
                patternChangeInterval = Random.Range(minIdleTime, maxIdleTime);
                isRunaway = false;
                isWalk = false;
            }
            else if (isWalk)
            {
                patternChangeInterval = Random.Range(minIdleTime, maxIdleTime);
                isWalk = false;
            }
            else
            {
                patternChangeInterval = Random.Range(minWalkTime, maxWalkTime);
                isWalk = true;
            }
        }
    }
    #region override_method
    public override void Down()
    {
        isDown = true;
        SetRunaway();
    }

    public override void Rising()
    {
        transform.LookAt(new Vector3(GameManager.Instance.player.transform.position.x, transform.position.y, GameManager.Instance.player.transform.position.z));
        transform.Rotate(0, 180, 0);
        isRunaway = true;
        patternChangeTimer = 0.0f;
    }
    protected override void Die()
    {
        isDie = true;
        GameManager.Instance.IncreaseMoney(money);
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<BoxCollider>().enabled = false;
    }
    public override void Respawn()
    {
        patternChangeTimer = 0;
        isDie = false;
        hp = 100;
        SpawnManager.Instance.NPCRepositioning(this);
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<BoxCollider>().enabled = true;
        print("Citizen Respawn");
    }
    #endregion
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall" && isRunaway)
        {
            transform.Rotate(0, Random.Range(90, 270), 0);
        }
    }
}