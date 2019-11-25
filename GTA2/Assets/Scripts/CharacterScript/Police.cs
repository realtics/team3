using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Police : NPC
{
    public bool isStealing { get; set; }
    public bool isChasePlayer { get; set; }

    float patternChangeTimer = 3.0f;
    float patternChangeInterval = 3.0f;
    float attackInterval = 1.0f;
    float curAttackCoolTime = 0.0f;
    float jumpTime = 1.5f;
    float jumpTimer = 0.0f;

    GameObject targetCar; //TODO : 경찰차 다시 타기에 사용

    public GunState curGunIndex { get; set; }

    public bool isAttack { get; set; }

    Rigidbody myRigidbody;

    public Animator animator;
    public List<NPCGun> gunList;

    float minIdleTime = 2.0f;
    float maxIdleTime = 5.0f;
    float minWalkTime = 2.0f;
    float maxWalkTime = 5.0f;

    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }
    void Start()
    {
        money = 50;
        runSpeed = 1.0f;
        patternChangeInterval = Random.Range(minIdleTime, maxIdleTime);
        patternChangeTimer = patternChangeInterval;
    }

    void Update()
    {
        animator.SetBool("isWalk", isWalk);
        animator.SetBool("isShot", isShot);
        animator.SetBool("isPunch", isPunch);
        animator.SetBool("isJump", isJump);
        animator.SetBool("isDie", isDie);

        base.NPCUpdate();
        if (isDie)
            return;
        TimerCheck();
        ActivityByState();
    }
    void ActivityByState()
    {
        if(isChasePlayer)
        {
            if (PlayerOutofRange())
            {
                isChasePlayer = false;
                return;
            }
            if (InPunchRange())
            {
                isWalk = false;
                //TODO : 총쏘기
                //InShotRange();
                //if (InPunchRange())
                //{
                //    isPunch = true;
                //    isShot = true;
                //}

                //TODO : NPC punch 완성되는 대로 샷
                //GunPunch Shot

                isShot = false;
                isPunch = true;
                gunList[0].GetComponent<NpcGunPunch>().StartShot();
            }
            else
            {
                gunList[0].GetComponent<NpcGunPunch>().StopShot();
                isWalk = true;
                base.ChasePlayer();
            }
                
        }
        else if(isWalk)
        {
            base.Raycast();
            base.Move();
        }
    }

    protected override void Die() //리스폰 필요
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
        isWalk = false;
        hp = 100;
        NPCSpawnManager.Instance.NPCRepositioning(this);
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<BoxCollider>().enabled = true;
        print("Police Respawn");
    }

    void TimerCheck()
    {
        patternChangeTimer += Time.deltaTime;

        if(!isChasePlayer)
        {
            if(DectectedPlayerAttack())
            {
                SetChasePlayer();
            }
            else
                PatternChange();
        }
    }
    void SetChasePlayer()
    {
        isChasePlayer = true;
    }
    void PatternChange()
    {
        if (patternChangeTimer > patternChangeInterval)
        {
            patternChangeTimer = 0.0f;

            if(isWalk)
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
    #region override method
    public override void Down()
    {
        isDown = true;
    }

    public override void Rising()
    {
        //경찰은 맞아도 일어 나기만 함
        isDown = false;
    }

  
    #endregion
}
