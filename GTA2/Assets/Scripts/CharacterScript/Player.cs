﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : People
{
    public bool isWalk { get; set; }
    public bool isShot { get; set; }
    public bool isPunch { get; set; }
    public bool isJump { get; set; }
    public bool isStealing { get; set; }
    public bool isBusted { get; set; }
    int remains = 3;
    GameObject targetCar;
    float playerMoveSpeed = 2.0f;

    public GunState curGunIndex { get; set; }

    public bool isAttack { get; set; }
    public int money { get; set; }

    int defaultHp = 500;
    float jumpTime = 1.5f;
    float jumpTimer = 0.0f;
    float respawnTime = 3.0f;
    float respawnTimer = 0.0f;

    Rigidbody myRigidBody;
    RaycastHit hit;

    public LayerMask collisionLayer;
    public Animator animator;
    public List<Gun> gunList;
    public List<Transform> respawnPoint;
    // UI메니저 추가 - 조이스틱 상황에 맞게 키보드 동작을 위함
    void Awake()
    {
        myRigidBody = GetComponent<Rigidbody>();
    }
    void Start()
    {
        moveSpeed = playerMoveSpeed;
        hp = defaultHp;
        // 이거 그냥 건 리스트 쓰니까 프리펩에 있는게 계속 수정됌 그래서 복사를 해
        // 프리팹은 지키는 쪽으로 합시다ㅇㅇ
        List<Gun> gunTempList = new List<Gun>();
        foreach (var item in gunList)
        {
            GameObject NewGun = Instantiate(item.gameObject);
            NewGun.transform.parent = transform;
            NewGun.SetActive(false);
            gunTempList.Add(NewGun.GetComponent<Gun>());
        }
        gunList.Clear();
        gunList = gunTempList;

        gunList[(int)GunState.None].bulletCount = 1;
        gunList[(int)GunState.None].gameObject.SetActive(true);

        UIManager.Instance.HumanUIMode();
    }

    void Update()
    {
        animator.SetBool("isWalk", isWalk);
        animator.SetBool("isShot", isShot);
        animator.SetBool("isPunch", isPunch);
        animator.SetBool("isJump", isJump);
        animator.SetBool("isDie", isDie);
        if (isDie)
            return;
        UpdateInput();
    }
    void FixedUpdate()
    {
        UpdateTargetRotation();
        UpdateSlerpedRotation();
        TimerCheck();
        Move();
    }

    void UpdateInput()
    {
        if (isDie)
            return;
        if(!MoveControlKeyboard())
            MoveControlJoystick();

        ActiveControl();
        WeaponSwap();
    }

    void TimerCheck()
    {
        if (isJump)
            LandCheck();
        else if(isDie)
        {
            RespawnTimerCheck();
        }

    }
    void RespawnTimerCheck()
    {
        respawnTimer += Time.deltaTime;

        if(respawnTime < respawnTimer)
        {
            respawnTimer = 0.0f;
            Respawn();
        }
    }
    protected override void Die()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<BoxCollider>().enabled = false;
        hDir = 0;
        vDir = 0;
        isDie = true;
        remains--;

        if(remains == 0)
        {
            remains = 3;
            //GameOver
        }
        else if (isBusted)
        {
            UIManager.Instance.TurnOnBustedSprite();
        }
        else //isWasted
        {
            UIManager.Instance.TurnOnWastedSprite();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("NPCPunch"))
        {
            other.gameObject.SetActive(false);
            int bulletDamage = other.gameObject.GetComponent<Bullet>().bulletDamage;
            print(bulletDamage);
            isBusted = true;
            Hurt(bulletDamage, true);
        }
        else if (other.gameObject.CompareTag("NPCBullet"))
        {
            int bulletDamage = other.gameObject.GetComponent<Bullet>().bulletDamage;
            isBusted = false;

            Hurt(bulletDamage);
        }
    }
    #region lowlevelCode
    void LandCheck()
    {
        Debug.DrawRay(transform.position, transform.up * -1, Color.red);

        jumpTimer += Time.deltaTime;

        if (jumpTimer > jumpTime)
        {
            // 밑에 차가없으면 Land
            if (!(Physics.Raycast(transform.position, transform.up * -1, out hit, 1f, collisionLayer)
                && hit.transform.CompareTag("Car")))
            {
                jumpTimer = 0.0f;
                Land();
            }
        }
    }
    
    protected override void Move()
    {
        if (isStealing)
        {
            transform.LookAt(targetCar.transform);
            myRigidBody.MovePosition(transform.position + (transform.forward * Time.deltaTime * moveSpeed));
            //일정거리이상 멀어져서 차 쫓기 포기
            if (Vector3.Distance(transform.position, targetCar.transform.position) > 5)
            {
                isStealing = false;
                return;
            }
            //차 탑승
            //TODO : 운전석으로 점프해서가서 탑승 모션 구현
            else if (Vector3.Distance(transform.position, targetCar.transform.position) < 1)
            {
                isStealing = false;
                targetCar.GetComponent<CarController>().GetOnTheCar(this as People);
                UIManager.Instance.CarUIMode(targetCar.GetComponent<CarController>());
                print("탐");
            }
        }
        else
            myRigidBody.MovePosition(transform.position + (new Vector3(hDir, 0, vDir).normalized * Time.deltaTime * moveSpeed));
    }
    public int GetHp()
    {
        return hp;
    }
    bool MoveControlKeyboard()
    {
        if (Input.GetAxisRaw("Vertical") == 0 && Input.GetAxisRaw("Horizontal") == 0)
        {
            if (!isStealing)
                isWalk = false;
            vDir = 0;
            hDir = 0;
            return false;
        }
        else
        {
            vDir = Input.GetAxisRaw("Vertical"); //GetAxis
            hDir = Input.GetAxisRaw("Horizontal");
            isWalk = true;
            isStealing = false;
            return true;
        }
    }
    bool isAnyActive()
    {
        if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) &&
            !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow) &&
            !isJump)
            return true;
        else
            return false;
    }
    public void MoveControlJoystick()
    {
        //키보드랑 독립적으로 작동하게 변경
        if (Mathf.Abs(UIManager.Instance.playerJoystick.Horizontal) < 0.01f && Mathf.Abs(UIManager.Instance.playerJoystick.Vertical) < 0.01f)
        {
            if(!isStealing)
                isWalk = false;
            return;
        }
        isWalk = true;
        isStealing = false;
        hDir = UIManager.Instance.playerJoystick.Horizontal / 5.0f;
        vDir = UIManager.Instance.playerJoystick.Vertical / 5.0f;
    }
    void ActiveControl()
    {
        if (Input.GetKeyDown(KeyCode.A) && !isJump)
        {
            ShotButtonDown();
            isStealing = false;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            ShotStop();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            isStealing = false;
            JumpButtonDown();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SetChaseTargetCar();//내리면서 바로 타지 않기
        }
    }
  
    void Jump()
    {
        if (isJump)
            return;
        isJump = true;
        GetComponent<Rigidbody>().useGravity = false;
        transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
    }
    void Land()
    {
        isJump = false;
        transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
        GetComponent<Rigidbody>().useGravity = true;
    }
    void WeaponSwap()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            SwapNext();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SwapPrev();
        }
    }
    public void SwapNext()
    {
        if ((int)curGunIndex <= 0)
        {
            gunList[(int)curGunIndex].gameObject.SetActive(false);
            curGunIndex = GunState.Granade;
        }
        else
        {
            gunList[(int)curGunIndex].gameObject.SetActive(false);
            curGunIndex--;
        }
        while (gunList[(int)curGunIndex].bulletCount <= 0)
        {
            curGunIndex--;
            if ((int)curGunIndex == gunList.Count)
            {
                curGunIndex = GunState.None;
            }
        }
        gunList[(int)curGunIndex].gameObject.SetActive(true);
        Debug.Log(curGunIndex);
    }
    public void SwapPrev()
    {
        if ((int)curGunIndex >= gunList.Count - 1)
        {
            gunList[(int)curGunIndex].gameObject.SetActive(false);
            curGunIndex = GunState.None;
        }
        else
        {
            gunList[(int)curGunIndex].gameObject.SetActive(false);
            curGunIndex++;
        }

        while (gunList[(int)curGunIndex].bulletCount <= 0)
        {
            curGunIndex++;
            if ((int)curGunIndex == gunList.Count)
            {
                curGunIndex = GunState.None;
            }
        }
        gunList[(int)curGunIndex].gameObject.SetActive(true);
        Debug.Log(curGunIndex);
    }
    public void Respawn()
    {
        UIManager.Instance.HumanUIMode();
        isDie = false;
        SetHpDefault();
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<BoxCollider>().enabled = true;
        print("Player Respawn");
        transform.position = respawnPoint[Random.Range(0, respawnPoint.Count)].position;
        //카메라 위치 조정,
        //자동차 셋팅
        //사람위치 재할당
        if (isBusted)
        {
            UIManager.Instance.TurnOffEndUI();
        }
        else
        {
            UIManager.Instance.TurnOffEndUI();
        }
    }

    //TODO : UIManager에 있는것이 좋을 것 같음.
    public void JumpButtonDown()
    {
        Jump();
    }

    public void ShotButtonDown()
    {
        if (isJump)
        {
            return;
        }

        gunList[(int)curGunIndex].UpdateBottonDown();
        switch (curGunIndex)
        {
            case GunState.None:
                isPunch = true;
                break;
            case GunState.FireBottle:
            case GunState.Granade:
                isPunch = true;
                isAttack = true;
                break;
            case GunState.Pistol:
            case GunState.DoublePistol:
            case GunState.Machinegun:
            case GunState.SleepMachinegun:
            case GunState.RocketLauncher:
            case GunState.Electric:
            case GunState.ShotGun:
            case GunState.FireGun:
                isShot = true;
                isAttack = true;
                break;
            default:
                break;
        }
    }

    public void ShotButtonUp()
    {
        gunList[(int)curGunIndex].UpdateBottonUp();
        ShotStop();
    }

    public void SetChaseTargetCar()
    {
        isStealing = true;
        isWalk = true;

        List<GameObject> activeCarList = GameObject.FindWithTag("SpawnManager").GetComponent<SpawnManager>().activeCarList;
        //제일 가까운 차 가져오기

        float minDistance = 100.0f;

        foreach (var car in activeCarList)
        {
            if (minDistance > Vector3.Distance(car.transform.position, transform.position))
            {
                targetCar = car;
                minDistance = Vector3.Distance(car.transform.position, transform.position);
            }
        }
    }
    public void SetHpDefault()
    {
        hp = defaultHp;
    }
    void ShotStop()
    {
        isShot = false;
        isAttack = false;
        isPunch = false;
    }
    #endregion
}