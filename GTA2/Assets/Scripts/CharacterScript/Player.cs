using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : People
{
    public bool isChasingCar { get; set; }
    public bool isGetOnTheCar { get; set; }
    public bool isBusted { get; set; }
    public bool isAttack { get; set; }

    CarController targetCar;
    float playerMoveSpeed = 2.0f;
    public GunState curGunIndex { get; set; }
    public List<Gun> gunList;
    PlayerPhysics playerPhysics;

    int defaultHp = 500;

    //Timer
    float jumpTime = 1.5f;
    float jumpTimer = 0.0f;
    float jumpMinTime = 0.5f;
    float respawnTime = 3.0f;
    float respawnTimer = 0.0f;
    float carOpenTime = 0.5f;
    float carOpenTimer = 0.0f;
    float bustedCheckTime = 3.0f;
    float bustedCheckTimer = 3.0f;

    Animator animator;
    
    

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        playerPhysics = GetComponent<PlayerPhysics>();
    }
    void Start()
    {
        PlayerInit();
        GunListInit();
        UIManager.Instance.HumanUIMode();
    }
    

    void Update()
    {
        AnimateUpdate();

        if (IsStuckedAnimated())
            return;
        UpdateInput();
    }
    void FixedUpdate()
    {
        UpdateTargetRotation();
        UpdateSlerpedRotation();
        TimerCheck();
        if (isGetOnTheCar)
            CarStealing();
        Move();
    }

    //Init
    void PlayerInit()
    {
        moveSpeed = playerMoveSpeed;
        hp = defaultHp;
    }
    void GunListInit()
    {
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
    }
    //Update
    void AnimateUpdate()
    {
        animator.SetBool("isWalk", isWalk);
        animator.SetBool("isShot", isShot);
        animator.SetBool("isPunch", isPunch);
        animator.SetBool("isJump", isJump);
        animator.SetBool("isDie", isDie);
        animator.SetBool("isGetOnTheCar", isGetOnTheCar);
    }
    void UpdateInput()
    {
        if (isDie)
            return;
        if (!MoveControlKeyboard())
            MoveControlJoystick();

        ActiveControl();
        WeaponSwap();
    }
    
   
    void CarStealing()
    {
        if (targetCar.isDoorOpen == false)//문열기
        {
            carOpenTimer += Time.deltaTime;
            transform.forward = targetCar.transform.forward;
            transform.position = targetCar.mainDoorPosition.transform.position;
            if (carOpenTimer > carOpenTime)
            {
                carOpenTimer = 0.0f;
                targetCar.isDoorOpen = true;
            }
        }
        else //탑승
        {
            transform.parent = targetCar.gameObject.transform;
            isGetOnTheCar = false;
            carOpenTimer = 0.0f;
            //사람 끌어내리기
            targetCar.PullOutOfATheCar();
            targetCar.GetOnTheCar(this);
            UIManager.Instance.CarUIMode(targetCar);
            print("탑승");
        }
    }
    

    void TimerCheck()
    {
        if (isJump)
            LandCheck();
        else if(isDie)
        {
            RespawnTimerCheck();
        }
        if(isBusted)
        {
            bustedCheckTimer += Time.deltaTime;
            if(bustedCheckTimer > bustedCheckTime)
            {
                isBusted = false;
                bustedCheckTimer = 0.0f;
            }
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

        if (--GameManager.Instance.remains == 0)
        {
            //GameOver;
            UIManager.Instance.TurnOnGameOverSprite();
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
            if (!playerPhysics.IsCarExistBelow())
            {
                jumpTimer = 0.0f;
                Land();
            }
        }
        else if(isChasingCar && jumpTimer > jumpMinTime)
        {
            jumpTimer = 0.0f;
            Land();
        }
    }
    protected override void Move()
    {
        if (isChasingCar)
        {
            //일정거리이상 멀어져서 차 쫓기 포기
            if (playerPhysics.InChasingDistance())
            {
                isChasingCar = false;
                return;
            }
            //차 탑승
            else if (playerPhysics.InStealingDistance())
            {
                isChasingCar = false;
                isGetOnTheCar = true;
            }
            else//차 쫓아가기
            {
                playerPhysics.LookAtCarDoor();
                playerPhysics.ChaseTheCar(moveSpeed);
            }
        }
        else//그 외 입력에 의한 이동
        {
            //playerPhysics.ChaseTheCar(moveSpeed);
            playerPhysics.MovePositionByInput(hDir, vDir, moveSpeed);
            print(hDir + " " + vDir);
        }

            
    }
    public int GetHp()
    {
        return hp;
    }
    bool MoveControlKeyboard()
    {
        if (Input.GetAxisRaw("Vertical") == 0 && Input.GetAxisRaw("Horizontal") == 0)
        {
            if (!isChasingCar)
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
            isChasingCar = false;
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
            if(!isChasingCar)
                isWalk = false;
            return;
        }
        isWalk = true;
        isChasingCar = false;
        hDir = UIManager.Instance.playerJoystick.Horizontal / 5.0f;
        vDir = UIManager.Instance.playerJoystick.Vertical / 5.0f;
    }
    void ActiveControl()
    {
        if (Input.GetKeyDown(KeyCode.A) && !isJump)
        {
            ShotButtonDownStatus();
            isChasingCar = false;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            ShotStop();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            isChasingCar = false;
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SetChaseTargetCar();//내리면서 바로 타지 않기
        }
    }
  
    public void Jump()
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
        GameManager.Instance.RespawnSetting();
        SetHpDefault();
        isDie = false;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<BoxCollider>().enabled = true;

        UIManager.Instance.TurnOffEndUI();
        UIManager.Instance.TurnOffEndUI();
        print("Player Respawn");
    }

    //TODO : UIManager에 있는것이 좋을 것 같음.

    public void ShotButtonDown()
    {
        ShotButtonDownGunSet();
        ShotButtonDownStatus();
    }

    void ShotButtonDownGunSet()
    {
        if (isJump)
        {
            return;
        }

        gunList[(int)curGunIndex].UpdateBottonDown();
    }

    void ShotButtonDownStatus()
    {
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
    bool IsStuckedAnimated()
    {
        if (isDie || isDown || isGetOnTheCar)
            return true;
        else
            return false;
    }
    public void ShotButtonUp()
    {
        gunList[(int)curGunIndex].UpdateBottonUp();
        ShotStop();
    }
    public void SetChaseTargetCar()
    {
        List<CarController> activeCarList = CarSpawnManager.Instance.activeCarList;

        //제일 가까운 차 가져오기
        float minDistance = 100.0f;

        foreach (var car in activeCarList)
        {
            if (minDistance > Vector3.Distance(car.transform.position, transform.position))
            {
                if (car.carState == CarController.CarState.destroied)
                    continue;

                targetCar = car;
                playerPhysics.SetCarDoorTransform(targetCar.mainDoorPosition.transform);
                minDistance = Vector3.Distance(car.transform.position, transform.position);
            }
        }
        if (Vector3.Distance(transform.position, targetCar.transform.position) < 5)
        {
            isChasingCar = true;
            isWalk = true;
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

    public override void Down()
    {
        isDown = true;
    }

    public override void Rising()
    {
        isDown = false;
    }
    #endregion
}