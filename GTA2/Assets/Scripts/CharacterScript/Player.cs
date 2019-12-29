using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerPhysics))]
public class Player : People
{
	public PlayerData playerData;
	public bool isChasingCar { get; set; }
	public bool isBusted { get; set; }
	public bool isAttack { get; set; }
	public bool isDriver { get; set; }

	public float runoverMinSpeedInChasing { get; set; } = 130;

	public GunState curGunIndex { get; set; }
    public List<PlayerGun> gunList;
	
	[HideInInspector]
	public PlayerPhysics playerPhysics;
	
	Animator animator;
	float rotHDir;
    float rotVDir;

    void Awake()
    {
		ComponentInit();
		base.TimerInit();
		MasterDataInit();
	}
    void Start()
    {
		GunListInit();
        UIManager.Instance.HumanUIMode();
    }
    void Update()
    {
        DebugX.DrawRay(transform.position, transform.forward);
        base.PeopleUpdate();
        AnimateUpdate();

		if (IsStuckedAnimation() || isDriver)
		{
			StopShot();
			return;
		}
            
        UpdateInput();
    }
    void FixedUpdate()
    {
        if (isDie)
        {
            if (TimerCheck(TimerType.Respawn))
                Respawn();
            return;
        }
        UpdateTargetRotation();
        UpdateSlerpedRotation();
        //위는 문제없음
        if (isGetOnTheCar)
            CarStealing();
        Move();
    }
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("NPCPunch"))
			HurtByNPC(true, other.gameObject.GetComponentInParent<Bullet>().bulletDamage);
		else if (other.gameObject.CompareTag("NPCBullet"))
		{
			other.gameObject.GetComponentInParent<Bullet>().Explosion();
			HurtByNPC(false, other.gameObject.GetComponentInParent<Bullet>().bulletDamage);
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
		playerPhysics.LookAtCar();

		if (playerPhysics.targetCar.doors[0].doorState == CarPassengerManager.DoorState.open) //문이열려있는 경우 탑승
        {
			GetOntheCar();
        }
        else//문열기 시도
        {
			OpenTheDoor();
        }
    }
	public void SetChaseTargetCar()
	{
		CarManager car = CarSpawnManager.Instance.FindClosestCar(transform.position);
		if (car == null)
			return;

		playerPhysics.targetCar = car.passengerManager;
		playerPhysics.SetCarDoorTransform(playerPhysics.targetCar.doors[0].transform);

		if ((transform.position - playerPhysics.targetCar.transform.position).magnitude < 5)
		{
			isChasingCar = true;
			isWalk = true;
		}
	}
	void ChasingCar()
	{
		//멀어져서 차 쫓기 포기
		if (playerPhysics.InChasingDistance())
		{
			isChasingCar = false;
			return;
		}
		//차 탑승
		else if (playerPhysics.InStealingDistance())
		{
			isGetOnTheCar = true;
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
	public void Respawn()
	{
		if (!isBusted)
		{
			UIManager.Instance.HumanUIMode();

			if (GameManager.Instance.playerCar != null)
			{
				PlayerDriverSetting(false);
				isDriver = false;
			}
			else
			{
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
				rigidbody.isKinematic = false;
				boxCollider.enabled = true;
				spriteRenderer.enabled = true;
			}
		}
		SetHpDefault();
		InitAnimation();
		GameManager.Instance.RespawnSetting();
		DebugX.Log("Player Respawn");
	}
	void StartShot()
	{
		gunList[(int)curGunIndex].UpdateButtonDown();
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

    public void RespawnGunList()
    {
        foreach (var item in gunList)
        {
            item.gameObject.SetActive(false);
        }


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
        GunState tempGunIndex = curGunIndex;
        for (int i = 0; i < gunList.Count; i++)
        {
            gunList[i].gameObject.SetActive(false);
        }


        for (int i = 0; i < gunList.Count; i++)
		{
            tempGunIndex++;
            if (tempGunIndex >= GunState.GunEnd)
            {
                tempGunIndex = GunState.None;
            }

            if (gunList[(int)tempGunIndex].bulletCount <= 0)
            {
                continue;
            }
            else if (gunList[(int)tempGunIndex].bulletCount > 0)
            {
                gunList[(int)tempGunIndex].gameObject.SetActive(true);

                curGunIndex = tempGunIndex;
                break;
            }
        }
	}
	public void SwapPrev()
    {
        GunState tempGunIndex = curGunIndex;

        for (int i = 0; i < gunList.Count; i++)
        {
            gunList[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < gunList.Count; i++)
		{
            tempGunIndex--;
            if (tempGunIndex < GunState.None)
            {
                tempGunIndex = GunState.GunEnd - 1;
            }

            if (gunList[(int)tempGunIndex].bulletCount <= 0)
            {
                continue;
            }
            else if (gunList[(int)tempGunIndex].bulletCount > 0)
            {
                gunList[(int)tempGunIndex].gameObject.SetActive(true);

                curGunIndex = tempGunIndex;
                break;
            }
        }
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
	
	public void ResetAllGunBullet()
	{
		foreach (var item in gunList)
		{
			item.ResetBulletCount();
		}
	}
	void OpenTheDoor()
	{
		isWalk = false;

		if (playerPhysics.targetCar.doors[0].doorState == CarPassengerManager.DoorState.close)
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
	void GetOntheCar()
	{
		isChasingCar = false;
		isGetOnTheCar = false;

		//경찰차면 못타고 문닫기
		if (playerPhysics.targetCar.carManager.ai.isPolice && playerPhysics.targetCar.doors[0].passenger != PeopleType.None)
		{
			StartCloseTheDoor();
			return;
		}
		//탑승후 문닫기
		if (playerPhysics.targetCar.doors[0].doorState == CarPassengerManager.DoorState.open)
		{
			StartCloseTheDoor();
		}
		//사람 끌어내리고 타기
		GameManager.Instance.player.playerPhysics.targetCar.GetOffTheCar(0, false, true);
		playerPhysics.targetCar.GetOnTheCar(People.PeopleType.Player);

        UIManager.Instance.CarUIMode(playerPhysics.targetCar.carManager);
	}
	void HurtByNPC(bool isPunch, int damage)
	{
		if (isPunch)
		{
			SoundManager.Instance.PlayClipToPosition(punchClip, SoundPlayMode.ObjectSFX, transform.position);
			isBusted = true;
			Hurt(damage);
		}
		else
		{
			isBusted = false;
			Hurt(damage);
		}
	}
	public void PlayerDriverSetting(bool GetOn, int idx = 0)
    {
        //플레이어 드라이버 셋팅
        if (GetOn)
        {
            rigidbody.isKinematic = true;
            boxCollider.enabled = false;
            spriteRenderer.enabled = false;
            isDriver = true;
            transform.SetParent(GameManager.Instance.playerCar.transform);
        }
        else
        {
            rigidbody.isKinematic = false;
            boxCollider.enabled = true;
            spriteRenderer.enabled = true;
            isDriver = false;
			transform.SetParent(null);
		}
    }
	void MasterDataInit()
	{
		base.checkingTimes[(int)TimerType.Down] = playerData.downTime;
        base.checkingTimes[(int)TimerType.Jump] = playerData.jumpTime;
        base.checkingTimes[(int)TimerType.Runover] = playerData.runoverTime;
		base.checkingTimes[(int)TimerType.Respawn] = playerData.respawnTime;
		base.checkingTimes[(int)TimerType.AutoLand] = playerData.autoLandTime;
        base.checkingTimes[(int)TimerType.Land] = playerData.autoLandTime;
        base.checkingTimes[(int)TimerType.JumpMin] = 0.1f;

		defaultHp = playerData.maxHp;
		hp = playerData.maxHp;
		moveSpeed = playerData.moveSpeed;
	}
	#endregion
	#region overrideMethod
	
	public override void Runover(float runoverSpeed, Vector3 carPosition, bool isPlayerCar = true)
	{
        isBusted = false;
        base.Runover(runoverSpeed, carPosition, isPlayerCar);
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
		base.LandCheck();
		
		if (isChasingCar && MathUtil.isArrivedIn2D(transform.position, playerPhysics.targetCar.transform.position) && 
			!IsCarExistBelow() &&
			TimerCheck(TimerType.JumpMin))
			Land();
	}
	protected override void Die()
	{
		base.Die();

		StopShot();
		GameOverUISetting();
		WantedLevel.instance.ResetWantedLevel();
		CameraController.Instance.ZoomIn();
        QuestManager.Instance.ResetQuest();
	}

    protected override void UpdateTargetRotation()
    {
        targetDirectionVector = new Vector3(rotHDir, 0, rotVDir).normalized;
    }
    protected override void UpdateSlerpedRotation()
    {
        if (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
        {
            targetDirectionVector = new Vector3(hDir, 0, vDir).normalized;

			if(targetDirectionVector != Vector3.zero)
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetDirectionVector), 0.4f);
        }
        else if (0 != hDir || 0 != vDir || 0 != rotHDir || 0 != rotVDir)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetDirectionVector), 0.4f);
            return;
        }
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
		if (Mathf.Abs(UIManager.Instance.playerMoveJoystick.Horizontal) < 0.01f && Mathf.Abs(UIManager.Instance.playerMoveJoystick.Vertical) < 0.01f)
		{
			if (!isChasingCar)
				isWalk = false;

            if (Mathf.Abs(UIManager.Instance.playerWeaponJoystick.Horizontal) < 0.01f && Mathf.Abs(UIManager.Instance.playerWeaponJoystick.Vertical) < 0.01f)
                return;
		}
		isWalk = true;
		isChasingCar = false;
        
        UpdateJoystickDirection();
    }

    void UpdateJoystickDirection()
    {
        float moveHDir = UIManager.Instance.playerMoveJoystick.Horizontal / 5.0f;
        float moveVDir = UIManager.Instance.playerMoveJoystick.Vertical / 5.0f;

        float weaponHDir = UIManager.Instance.playerWeaponJoystick.Horizontal / 5.0f;
        float weaponVDir = UIManager.Instance.playerWeaponJoystick.Vertical / 5.0f;


        if (Mathf.Abs(weaponHDir) <= .01f && Mathf.Abs(weaponVDir) <= .01f)
        {
            hDir = moveHDir;
            vDir = moveVDir;

            rotHDir = hDir;
            rotVDir = vDir;
        }

        else
        {
            hDir = moveHDir;
            vDir = moveVDir;

            rotHDir = weaponHDir;
            rotVDir = weaponVDir;
        }
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
            SetChaseTargetCar();
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
			UIManager.Instance.TurnOnGameOver();
		}
		else if (isBusted)
		{
			UIManager.Instance.TurnOnBusted();
		}
		else //isWasted
		{
			UIManager.Instance.TurnOnWasted();
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
	protected override void InitAnimation()
	{
		base.InitAnimation();
		isChasingCar = false;
		isAttack = false;
	}
	void OnCollisionStay(Collision collision)
	{
		if (collision.gameObject.CompareTag("Car") && isChasingCar && !isGetOnTheCar)
		{
			Jump();
		}
	}
	public void ShotButtonUp()
	{
		gunList[(int)curGunIndex].UpdateButtonUp();
		StopShot();
	}
	void ComponentInit()
	{
		playerPhysics = GetComponent<PlayerPhysics>();
		animator = GetComponentInChildren<Animator>();
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		rigidbody = GetComponent<Rigidbody>();
		boxCollider = GetComponent<BoxCollider>();
	}
	#endregion
}