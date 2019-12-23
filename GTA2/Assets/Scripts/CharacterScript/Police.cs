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
		StartCoroutine(Raycast());
		StartCoroutine(PlayerStateCheck());
	}
	private void OnDisable()
	{
		base.NPCOnDisable();
		StopCoroutine(Raycast());
		StopCoroutine(PlayerStateCheck());
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
			base.Move();
		}
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
			LookAtPlayerCar();
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
				OpenTheDoor(GameManager.Instance.player.playerPhysics.targetCar);
			}
		}
		else if (InChaseRange()) //추격
		{
			if(isGetOnTheCar) //문여는 중 놓칠경우
			{
				Down();
				isGetOnTheCar = false;
			}
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
		runSpeed = policeData.runSpeed;

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
	public void LookAtPlayerCar()
	{
		transform.LookAt(new Vector3(GameManager.Instance.player.playerPhysics.targetCar.transform.position.x, transform.position.y, GameManager.Instance.player.playerPhysics.targetCar.transform.position.z));
		DebugX.DrawRay(transform.position, (GameManager.Instance.player.playerPhysics.targetCar.transform.position - transform.position), Color.blue);
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
	IEnumerator PlayerStateCheck()
	{
		while (true)
		{
			if (GameManager.Instance.player.isDie)
				SetDefault();
			else if (!PlayerOutofRange() && WantedLevel.instance.level >= 1)
			{
				StopCoroutine(Raycast());
				isChasePlayer = true;
			}
			else
			{
				StartCoroutine(Raycast());
				isChasePlayer = false;
			}

			yield return new WaitForSeconds(0.5f);
		}
		
			
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
		GameManager.Instance.player.playerPhysics.targetCar.GetOffTheCar(0, false, true);
	}
	void OpenTheDoor(CarPassengerManager pm)
	{
		isWalk = false;
        transform.forward = pm.transform.forward;
        StartCoroutine(pm.OpenTheDoor(0));
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
