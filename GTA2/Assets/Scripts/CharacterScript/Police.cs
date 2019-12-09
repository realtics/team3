using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Police : NPC
{
	void Awake()
	{
		gameManager = GameManager.Instance;
	}
	void Start()
	{
		base.NPCInit();
		SpecInit(); 
		TimeInit();
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
			}
			else
			{
				OpenTheDoor();
			}
		}
		else if (InShotRange()) //사격
		{
			base.LookAtPlayer();
			base.StartShot();
		}
		else //추격
		{
			isGetOnTheCar = false;
			base.StopShot();
			base.ChasePlayer();
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
	void SpecInit()
	{
		money = 50;
		moveSpeed = 1.0f;
		jumpTime = 0.3f;
		runSpeed = 1.5f;
	}
	void TimeInit()
	{
		jumpTime = 0.5f;
		minIdleTime = 0.3f;
		maxIdleTime = 1.0f;
		minWalkTime = 10.0f;
		maxWalkTime = 15.0f;
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
		isGetOnTheCar = false;
		transform.parent = GameManager.Instance.player.playerPhysics.targetCar.gameObject.transform;
		GameManager.Instance.player.playerPhysics.targetCar.PullOutDriver();
	}
	void OpenTheDoor()
	{
		isWalk = false;
		GameManager.Instance.player.playerPhysics.LookAtCar();

		if (!GameManager.Instance.player.playerPhysics.targetCar.isRunningOpenTheDoor)
		{
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
        //경찰은 맞아도 일어 나기만 함
        isDown = false;
    }
	protected override void Die() //리스폰 필요
	{
		isDie = true;
		gunList[0].GetComponent<NPCGun>().StopShot();
		gunList[1].GetComponent<NPCGun>().StopShot();
		GameManager.Instance.IncreaseMoney(money);
		rigidbody.isKinematic = true;
		boxCollider.enabled = false;
	}
	#endregion
}
