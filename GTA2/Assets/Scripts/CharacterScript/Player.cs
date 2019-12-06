using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerPhysics))]
[RequireComponent(typeof(PlayerTimer))]
public class Player : People
{
    public bool isChasingCar { get; set; }
    public bool isBusted { get; set; }
    public bool isAttack { get; set; }

    float playerMoveSpeed = 2.0f;
	[HideInInspector] public float runoverMinSpeed = 30.0f;
    public GunState curGunIndex { get; set; }

    public List<PlayerGun> gunList;
    public PlayerPhysics playerPhysics;
    PlayerTimer playerTimer;
    int defaultHp = 200;

    Animator animator;
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        playerPhysics = GetComponent<PlayerPhysics>();
        playerTimer = GetComponent<PlayerTimer>();
		rigidbody = GetComponent<Rigidbody>();
	}
    void Start()
    {
        PlayerInit();
        GunListInit();
        UIManager.Instance.HumanUIMode();
    }
    void Update()
    {
        DebugX.DrawRay(transform.position, transform.forward);

        base.PeopleUpdate();
        AnimateUpdate();
        if (IsStuckedAnimation() || isDriver)
            return;
        UpdateInput();
    }
    void FixedUpdate()
    {
        TimerCheck();
        UpdateTargetRotation();
        UpdateSlerpedRotation();

        if (isGetOnTheCar)
            CarStealing();
        Move();
    }
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("NPCPunch"))
		{
			other.gameObject.SetActive(false);
			int bulletDamage = other.gameObject.GetComponentInParent<Bullet>().bulletDamage;
			isBusted = true;
			Hurt(bulletDamage);
		}
		else if (other.gameObject.CompareTag("NPCBullet"))
		{
			int bulletDamage = other.gameObject.GetComponentInParent<Bullet>().bulletDamage;
			other.gameObject.GetComponentInParent<Bullet>().Explosion();
			isBusted = false;

			Hurt(bulletDamage);
		}
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
        if (isDie)
            return;
        if (playerPhysics.targetCar.isDoorOpen[0]) //문열고 탑승
        {
            isChasingCar = false;
            isGetOnTheCar = false;

            if (playerPhysics.targetCar.carManager.ai.isPolice && playerPhysics.targetCar.passengers[0] != null)
            {
				StartCloseTheDoor();
                return;
            }
			else if (!playerPhysics.targetCar.isRunningCloseTheDoor)
			{
				StartCloseTheDoor();
			}
			//사람 끌어내리기
			transform.parent = playerPhysics.targetCar.gameObject.transform;
            playerPhysics.targetCar.PullOutDriver();
            playerPhysics.targetCar.GetOnTheCar(this, 0);
            UIManager.Instance.CarUIMode(playerPhysics.targetCar.carManager);
        }
        else//문열기
        {
            isWalk = false;
            playerPhysics.LookAtCar();
            if (!playerPhysics.targetCar.isRunningOpenTheDoor)
            {

                transform.forward = playerPhysics.targetCar.transform.forward;
                StartCoroutine(playerPhysics.targetCar.OpenTheDoor(0));
            }
            //거리 멀어지면 실패
            if (!playerPhysics.IsGetOnDistance())
            {
                Down();
                isChasingCar = false;
                isGetOnTheCar = false;
                return;
            }
        }
    }
	
   
   
	public void SetChaseTargetCar()
	{
		CarManager car = CarSpawnManager.Instance.FindClosestCar(transform.position);
		if (car == null)
			return;

		playerPhysics.targetCar = car.passengerManager;
		playerPhysics.SetCarDoorTransform(playerPhysics.targetCar.doorPositions[0]);

		if ((transform.position - playerPhysics.targetCar.transform.position).magnitude < 5)
		{
			isChasingCar = true;
			isWalk = true;
		}
	}
	void ChasingCar()
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
			isGetOnTheCar = true;
			playerPhysics.ChaseTheCar(moveSpeed);
		}
		else//차 쫓아가기
		{
			playerPhysics.ChaseTheCar(moveSpeed);
		}
	}

	#region lowlevelCode
	public int GetHp()
    {
        return hp;
    }
	void PlayerInit()
	{
		moveSpeed = playerMoveSpeed;
		downTime = 1.0f;
		hp = defaultHp;
	}
	void StartShot()
	{
		gunList[(int)curGunIndex].UpdateButtonDown();
		isShot = true;
		isAttack = true;
		isPunch = false;
	}
	void StopShot()
	{
		gunList[(int)curGunIndex].UpdateButtonUp();
		isShot = false;
		isAttack = false;
		isPunch = false;
	}
	
	void GunListInit()
	{
		List<PlayerGun> gunTempList = new List<PlayerGun>();
		foreach (var item in gunList)
		{
			GameObject NewGun = Instantiate(item.gameObject);
			NewGun.transform.parent = transform;
			NewGun.SetActive(false);

			PlayerGun NewPlayerGun = NewGun.GetComponent<PlayerGun>();
			NewPlayerGun.Init();
			gunTempList.Add(NewPlayerGun);
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
		animator.SetBool("isDown", isDown);
		animator.SetBool("isDie", isDie);
		animator.SetBool("isGetOnTheCar", isGetOnTheCar);
		animator.SetBool("isRunover", isRunover);
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
	void StartCloseTheDoor()
	{
		transform.forward = playerPhysics.targetCar.transform.forward;
		StartCoroutine(playerPhysics.targetCar.CloseTheDoor(0));
	}
	public void SetHpDefault()
	{
		hp = defaultHp;
	}
	void TimerCheck()
	{
		if (isDie)
		{
			if (playerTimer.RespawnTimerCheck())
				Respawn();
		}
		if (isBusted && playerTimer.BustedTimerCheck() && !isDie)
		{
			isBusted = false;
		}
	}

	public void ResetAllGunBullet()
	{
		foreach (var item in gunList)
		{
			item.ResetBulletCount();
		}
	}
	#endregion
	#region overrideMethod
	public override void Respawn()
	{
		UIManager.Instance.HumanUIMode();
		GameManager.Instance.RespawnSetting();
		SetHpDefault();
		isDriver = false;
		isDown = false;
		isWalk = false;
		isDie = false;

		UIManager.Instance.TurnOffEndUI();

		if (isBusted)
		{
			//Invoke("Down", 0.2f);
		}
		else
		{
			gameObject.transform.SetParent(null);
			GetComponentInChildren<SpriteRenderer>().enabled = true;
			rigidbody.isKinematic = false;
			boxCollider.enabled = true;
		}

		print("Player Respawn");
	}
	protected override void Move()
	{
		if (isChasingCar)
		{
			ChasingCar();
		}
		else//그 외 입력에 의한 이동
		{
			isChasingCar = false;
			playerPhysics.MovePositionByInput(hDir, vDir, moveSpeed);
		}
	}
	protected override void LandCheck()
	{
		Debug.DrawRay(transform.position, transform.up * -1, Color.red);
		print(jumpTimer);

		if (JumpTimerCheck())
		{
			if (!IsCarExistBelow())
				Land();
		}
		else if (isChasingCar && JumpMinTimeCheck())
			Land();
	}
	protected override void Die()
	{
		if (isDie)
			return;
		isDie = true;
		rigidbody.isKinematic = true;
		boxCollider.enabled = false;
		hDir = 0; vDir = 0;

		GameOverUISetting();
		WantedLevel.instance.ResetWantedLevel();
		CameraController.Instance.ZoomIn();
	}
	

	#endregion
	#region InputLogic
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
	public void MoveControlJoystick()
	{
		//키보드랑 독립적으로 작동하게 변경
		if (Mathf.Abs(UIManager.Instance.playerMoveJoystick.Horizontal) < 0.01f && Mathf.Abs(UIManager.Instance.playerMoveJoystick.Vertical) < 0.01f)
		{
			if (!isChasingCar)
				isWalk = false;
			return;
		}
		isWalk = true;
		isChasingCar = false;
		hDir = UIManager.Instance.playerMoveJoystick.Horizontal / 5.0f;
		vDir = UIManager.Instance.playerMoveJoystick.Vertical / 5.0f;
	}
	bool IsMoveInput()
    {
        if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) &&
            !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow) &&
            !isJump)
            return true;
        else
            return false;
    }
    
    void ActiveControl()
    {
        if (Input.GetKeyDown(KeyCode.A) && !isJump)
        {
            ShotAnimation();
			StartShot();
			isChasingCar = false;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            StopShot();
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
	#endregion
	#region UIButtonLogic
	public void ShotButtonDown()
    {
        ShotButtonDownGunSet();
        ShotAnimation();
    }

    void ShotButtonDownGunSet()
    {
        if (IsStuckedAnimation())
        {
            return;
        }

        gunList[(int)curGunIndex].UpdateButtonDown();
    }
	void GameOverUISetting()
	{
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
	void ShotAnimation()
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
	public void ShotButtonUp()
	{
		gunList[(int)curGunIndex].UpdateButtonUp();
		StopShot();
	}
	#endregion
}