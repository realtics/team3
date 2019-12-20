using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Police : NPC
{
	public PoliceData policeData;

	void Awake()
	{
		base.TimerInit();
		MasterDataInit();
	}
	private void OnEnable()
	{
		base.NPCOnEnable();
	}
	private void OnDisable()
	{
		base.NPCOnDisable();
	}
	void Update()
    {
		base.PeopleUpdate();
		base.NPCUpdate();

        if (isDie || isDown)
        {
            StopPunch();
            return;
        }
        if (!isChasePlayer)
        {
            PatternChangeTimerCheck();
        }
        if (DetectedPlayerAttack() && !isChasePlayer)
        {
            WantedLevel.instance.CommitCrime(WantedLevel.CrimeType.gunFire, GameManager.Instance.player.transform.position);
        }

		PlayerStateCheck();
    }
	void FixedUpdate()
	{
		if (isDie || isDown)
			return;
		if (isChasePlayer)
		{
			if (GameManager.Instance.player.isDriver)
			{
				ChasePlayerCharacterInCar();
			}
			else
			{
				isShot = false;
				gunList[1].GetComponent<NPCGun>().StopShot();
				ChasePlayerCharacter();
			}
		}
		else if (isWalk)
		{
			base.Raycast();
			base.Move();
		}
		else
			base.Raycast();
	}
	void ChasePlayerCharacterInCar()
	{
		if (PlayerOutofRange())
		{
			isChasePlayer = false;
			return;
		}
		//끌어내리는 범위
		if (InPunchRange())//InPullOutRange()
		{
			isGetOnTheCar = true;
			isWalk = false;

			//문이 열려있는지 확인
			if (GameManager.Instance.player.playerPhysics.targetCar.doors[0].doorState == CarPassengerManager.DoorState.open)
			{
				PullOutDriver();
				isGetOnTheCar = false;
			}
			else
			{
				OpenTheDoor();
			}
		}
		else if (InChaseRange()) //추격
		{
			isGetOnTheCar = false;
			base.StopShot();
			base.ChasePlayer();
		}
		else //사격
		{
			base.LookAtPlayer();
			base.StartShot();
		}
	}
	private void OnCollisionStay(Collision collision)
	{
		if(collision.gameObject.CompareTag("Car") && isChasePlayer && !isJump)
		{
			Jump();
		}
	}
	#region lowLevelCode

	void MasterDataInit()
	{
		defaultHp = policeData.maxHp;
		moveSpeed = policeData.moveSpeed;
		runSpeed = policeData.runawaySpeed;

		findRange = policeData.findRange;
		punchRange = policeData.punchRange;
		shotRange = policeData.shotRange;
		chaseRange = policeData.chaseRange;
		outofRange = policeData.outofRange;

		minIdleTime = policeData.minIdleTime;
		maxIdleTime = policeData.maxIdleTime;
		minWalkTime = policeData.minWalkTime;
		maxWalkTime = policeData.maxWalkTime;

        checkingTimes[(int)TimerType.Jump] = policeData.jumpTime;
        checkingTimes[(int)TimerType.Down] = policeData.downTime;
        checkingTimes[(int)TimerType.CarOpen] = policeData.carOpenTime;
		base.Timers[(int)TimerType.JumpMin] = 1.0f;
		money = policeData.money;
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
			StartPunch();
		}
        else
        {
			StopPunch();
			base.ChasePlayer();
        }
    }
	void PlayerStateCheck()
	{
		if (GameManager.Instance.player.isDie)
			SetDefault();
		else if (!PlayerOutofRange() && WantedLevel.instance.level >= 1)
			isChasePlayer = true;
		else
			isChasePlayer = false;
	}
	void SetDefault()
	{
		hp = 100;
		isChasePlayer = false;
		isWalk = false;
		isGetOnTheCar = false;
		isShot = false;
        gunList[0].GetComponent<NPCGun>().StopShot();
        gunList[1].GetComponent<NPCGun>().StopShot();
		isPunch = false;
	}
	
	void PullOutDriver()
	{
		GameManager.Instance.player.playerPhysics.targetCar.PullOutDriver();
	}
	void OpenTheDoor()
	{
		isWalk = false;
		GameManager.Instance.player.playerPhysics.LookAtCar();

		if (GameManager.Instance.player.playerPhysics.targetCar.doors[0].doorState == CarPassengerManager.DoorState.close)
		{
			GameManager.Instance.player.playerPhysics.targetCar.doors[0].doorState = CarPassengerManager.DoorState.opening;
			transform.forward = GameManager.Instance.player.playerPhysics.targetCar.transform.forward;
			StartCoroutine(GameManager.Instance.player.playerPhysics.targetCar.OpenTheDoor(0));
		}

		//거리 멀어지면 실패
		if (Vector3.SqrMagnitude(transform.position - GameManager.Instance.player.playerPhysics.targetCar.transform.position) < 0.1f)
		{
			Down();
			isGetOnTheCar = false;
			return;
		}
	}
	#endregion
	#region override method
	protected override void Die() //리스폰 필요
	{
		base.Die();
		gunList[0].GetComponent<NPCGun>().StopShot();
		gunList[1].GetComponent<NPCGun>().StopShot();
	}
	
	#endregion
}
