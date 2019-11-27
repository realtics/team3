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
    //float attackInterval = 1.0f;
    //float curAttackCoolTime = 0.0f;
    //float jumpTime = 1.5f;
    //float jumpTimer = 0.0f;
    float carOpenTimer = 0.0f;
    float carOpenTime = 0.5f;

    GameObject targetCar; //TODO : 경찰차 다시 타기에 사용
    public GunState curGunIndex { get; set; }

    public bool isAttack { get; set; }

    Rigidbody myRigidbody;

    public Animator animator;
    public List<NPCGun> gunList;

    float minIdleTime = 0.3f;
    float maxIdleTime = 1.0f;
    float minWalkTime = 10.0f;
    float maxWalkTime = 15.0f;

    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }
    void Start()
    {
        money = 50;
        moveSpeed = 1.0f;
        runSpeed = 1.5f;
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
        animator.SetBool("isDown", isDown);
        animator.SetBool("isGetOnTheCar", isGetOnTheCar);

        base.NPCUpdate();
        if (isDie || isDown)
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
                isShot = false;
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
        //사격
        if(InShotRange())
        {
            isWalk = false;
            transform.LookAt(new Vector3(GameManager.Instance.player.transform.position.x, transform.position.y, GameManager.Instance.player.transform.position.z));
            if(InChaseRange())//player 끌어내리러 가기
            {
                //사격
                //한번만
                isShot = true;
                isPunch = false;
                isWalk = true;
                gunList[1].GetComponent<NPCGun>().StartShot();
                
                base.ChasePlayer();
                
                if (InPunchRange())//player 끌어내리기
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
            }
        }
        else //쫓아가기
        {
            gunList[1].GetComponent<NPCGun>().StopShot();
            isWalk = true;
            isShot = false;
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
        else if(!PlayerOutofRange() && GameManager.Instance.wantedLevel >= 1)
        {
            isChasePlayer = true;
        }
    }
    void SetDefault()
    {
        hp = 100;
        isChasePlayer = false;
        isWalk = false;
        isGetOnTheCar = false;
        isShot = false;
        gunList[1].GetComponent<NPCGun>().StopShot();
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
        gunList[0].GetComponent<NPCGun>().StopShot();
        gunList[1].GetComponent<NPCGun>().StopShot();
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
