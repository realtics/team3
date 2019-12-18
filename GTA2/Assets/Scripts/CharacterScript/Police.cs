using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Police : NPC
{
	public PoliceData policeData;

	void Awake()
	{
		gameManager = GameManager.Instance;
		MasterDataInit();
	}
	private void OnEnable()
	{
		base.NPCOnEnable();
		StartCoroutine(ActivityByState());
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
        TimerCheck();
        ActivityByState();
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
			isGetOnTheCar = true;
			isWalk = false;

			//문이 열려있는지 확인
			if (GameManager.Instance.player.playerPhysics.targetCar.isDoorOpen[0])
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
		else //사격 //inShotRange
		{
			base.LookAtPlayer();
			base.StartShot();
		}
	}
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Car") && isChasePlayer)
		{
			Jump();
		}
	}
	#region lowLevelCode

	void MasterDataInit()
	{
		defaultHp = policeData.maxHp;
		moveSpeed = policeData.moveSpeed;
		downTime = policeData.downTime;
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
		carOpenTimer = policeData.carOpenTimer;
		carOpenTime = policeData.carOpenTime;
		money = policeData.money;
	}
	IEnumerator ActivityByState()
	{
		while (true)
		{
			if (isDie || isDown)
				yield break;

			else if (isWalk)
			{
				base.Raycast();
			}

			yield return new WaitForSeconds(0.3f);
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
		{
			SetDefault();
		}
		else if (!PlayerOutofRange() && WantedLevel.instance.level >= 1)
		{
			isChasePlayer = true;
		}
		else
		{
			isChasePlayer = false;
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
	void TimerCheck()
	{
		patternChangeTimer += Time.deltaTime;

		if (!isChasePlayer)
		{
			PatternChange();
		}
		if (DetectedPlayerAttack() && !isChasePlayer)
		{
			WantedLevel.instance.CommitCrime(WantedLevel.CrimeType.gunFire, gameManager.player.transform.position);
		}
	}
	void PullOutDriver()
	{
		GameManager.Instance.player.playerPhysics.targetCar.PullOutDriver();
	}
	void OpenTheDoor()
	{
		isWalk = false;
		GameManager.Instance.player.playerPhysics.LookAtCar();

		if (!GameManager.Instance.player.playerPhysics.targetCar.isRunningOpenTheDoor[0])
		{
			GameManager.Instance.player.playerPhysics.targetCar.isRunningOpenTheDoor[0] = true;
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
	public override void Down()
    {
		boxCollider.isTrigger = true;
		rigidbody.isKinematic = true;
		isDown = true;
    }

    public override void Rising()
    {
		boxCollider.isTrigger = false;
		rigidbody.isKinematic = false;
		//경찰은 맞아도 일어 나기만 함
		isDown = false;
    }
	protected override void Die() //리스폰 필요
	{
		if (isDie)
			return;
		isDie = true;
		gunList[0].GetComponent<NPCGun>().StopShot();
		gunList[1].GetComponent<NPCGun>().StopShot();
		GameManager.Instance.IncreaseMoney(money);
		rigidbody.isKinematic = true;
		boxCollider.enabled = false;
		NPCSpawnManager.Instance.DiedNPC.Add(this);
		SoundManager.Instance.PlayClipToPosition(dieClip[Random.Range(0, dieClip.Length)], SoundPlayMode.OneShotPlay, gameObject.transform.position);
	}
	public override void Runover(float runoverSpeed, Vector3 carPosition, bool isRunoverByPlayer = false)
	{
		if (runoverSpeed < runoverMinSpeed)
			return;
		Vector3 runoverVector = transform.position - carPosition;

		//속도에 비례한 피해 데미지 보정수치
		this.runoverSpeed = Mathf.Clamp((runoverSpeed / 3000.0f), 0, 0.3f);
		this.runoverVector = runoverVector.normalized * this.runoverSpeed * Mathf.Abs(Vector3.Dot(runoverVector, Vector3.right));
		isRunover = true;
		hDir = 0; vDir = 0;

		transform.LookAt(carPosition);
		
		if (isRunoverByPlayer)
		{
			print("보정전 수치 : " + runoverSpeed);
		}
		if (runoverSpeed > runoverHurtMinSpeed)
		{
			Hurt((int)(runoverSpeed / 3));
		}
		if (isRunoverByPlayer && isDie)
		{
			GameManager.Instance.IncreaseMoney(money);
			WorldUIManager.Instance.SetScoreText(transform.position, money);
			WantedLevel.instance.CommitCrime(WantedLevel.CrimeType.killPeople, transform.position);
		}
	}
	#endregion
}
