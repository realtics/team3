using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Police : NPC
{
    public bool isChasePlayer { get; set; }
    //경찰이 차에 탈때, 경찰이 플레이어 꺼낼때 모두 사용
    public bool isGetOnTheCar { get; set; } 

    float patternChangeTimer = 3.0f;
    float patternChangeInterval = 3.0f;
    float attackInterval = 1.0f;
    float curAttackCoolTime = 0.0f;
    float jumpTime = 1.5f;
    float jumpTimer = 0.0f;
    float carOpenTimer = 0.0f;
    float carOpenTime = 0.5f;

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
        animator.SetBool("isGetOnTheCar", isGetOnTheCar);

        base.NPCUpdate();
        if (isDie)
            return;
        TimerCheck();
        ActivityByState();
        PlayerStateCheck();
    }
    void ActivityByState()
    {
        if(isChasePlayer)
        {
            if(GameManager.Instance.player.isDriver)
            {
                ChasePlayerCharacterInCar();
            }
            else
            {
                ChasePlayerCharacter();
            }
               
        }
        else if(isWalk)
        {
            base.Raycast();
            base.Move();
        }
    }
    void ChasePlayerCharacter()
    {
        if (PlayerOutofRange())
        {
            isChasePlayer = false;
            return;
        }
        if (InPunchRange())
        {
            isWalk = false;
            isShot = false;
            isPunch = true;
            gunList[0].GetComponent<NPCGun>().StartShot();
        }
        else
        {
            gunList[0].GetComponent<NPCGun>().StopShot();
            isWalk = true;
            base.ChasePlayer();
        }
    }
    void ChasePlayerCharacterInCar()
    {
        if (PlayerOutofRange())
        {
            isChasePlayer = false;
            return;
        }
        if(InShotRange())
        {
            //차뺏기
            isWalk = false;
            if (InPunchRange())
            {
                transform.forward = GameManager.Instance.player.transform.forward;
                transform.position = GameManager.Instance.player.transform.position;

                //플레이어 끌어내리기
                if (CarOpenTimerCheck())
                {
                    GameManager.Instance.player.GetOffTheCar();
                    GameManager.Instance.player.isDown = true;
                    GameManager.Instance.player.isBusted = true;
                    GameManager.Instance.player.isDriver = false;
                }
                else
                {
                    //문열기 애니메이션
                    isGetOnTheCar = true;
                }
            }
            else //사격
            {
                gunList[1].GetComponent<NPCGun>().StartShot();
            }
        }
        else
        {
            gunList[1].GetComponent<NPCGun>().StopShot();
            isWalk = true;
            isShot = true;
            isPunch = false;
            base.ChasePlayer();
        }
    }
    void PlayerStateCheck()
    {
        if (GameManager.Instance.player.isDie)
        {
            SetDefault();
        }
    }
    void SetDefault()
    {
        hp = 100;
        isChasePlayer = false;
        isWalk = false;
        isGetOnTheCar = false;
        isPunch = false;
    }
    public bool CarOpenTimerCheck()
    {
        carOpenTimer += Time.deltaTime;

        if (carOpenTimer > carOpenTime)
        {
            carOpenTimer = 0.0f;
            return true;
        }
        return false;
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
