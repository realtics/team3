using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

public class Citizen : NPC
{
	public CitizenData citizenData;
	public SpriteRenderer ClothSpriteRenderer;
	public AudioClip[] downClip;
	static float shoutTime = 10.0f;
	static float shoutTimer = 0.0f;
	void Awake()
    {
		gameManager = GameManager.Instance;
		base.TimerInit();
		MasterDataInit();
	}

	private void OnEnable()
	{
		base.NPCOnEnable();
		ClothSpriteRenderer.color = new Color(Random.Range(0, 1.0f), Random.Range(0, 1.0f), Random.Range(0, 1.0f));
	}
	
	private void OnDisable()
	{
		base.NPCOnDisable();
	}
	void Update()
    {
		base.NPCUpdate();
		base.PeopleUpdate();

		if (isDie || isDown)
            return;
		ShoutTimerCheck();
		TimerCheck();
    }
	
	void FixedUpdate()
    {
        if (isDie || isDown)
            return;
        if (isRunaway)
        {
            base.RunAway();
        }
        else if (isWalk)
        {
			base.Raycast();
			base.Move();
            TimerCheck(TimerType.PatternChange);
        }
		else
        {
            base.Raycast();
            TimerCheck(TimerType.PatternChange);
        }
	}
	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Car") && isRunaway)
		{
			transform.Rotate(0, Random.Range(90, 270), 0);
		}
	}
	#region lowlevelCode

    void TimerCheck()
    {
        

        if (DetectedPlayerAttack())
        {
			//if(ShoutTimerCheck() && Random.Range(0, 3) < 1)
			//{
			//	SoundManager.Instance.PlayClipToPosition(runAwayClip[Random.Range(0, runAwayClip.Length)], SoundPlayMode.HumanSFX, gameObject.transform.position);
			//	shoutTimer = 0.0f;
			//}

			base.SetRunaway();
        }
        else
            PatternChange();
    }
	bool ShoutTimerCheck()
	{
		shoutTimer += Time.deltaTime;

		if (shoutTimer > shoutTime)
		{
			return true;
		}
		else
			return false;
	}
	void MasterDataInit()
	{
		defaultHp = citizenData.maxHp;
        money = citizenData.money;

        moveSpeed = citizenData.moveSpeed;
		runSpeed = citizenData.runawaySpeed;

		findRange = citizenData.findRange;
		punchRange = citizenData.punchRange;
		shotRange= citizenData.shotRange;
		chaseRange=citizenData.chaseRange;
		outofRange=citizenData.outofRange;

        checkingTimes[(int)TimerType.Jump] = citizenData.jumpTime; 
        checkingTimes[(int)TimerType.Down] = citizenData.downTime;
        checkingTimes[(int)TimerType.CarOpen] = citizenData.carOpenTime;
        checkingTimes[(int)TimerType.RunAway] = citizenData.runawayTime;

        minIdleTime =citizenData.minIdleTime;
		maxIdleTime=citizenData.maxIdleTime;
		minWalkTime=citizenData.minWalkTime;
		maxWalkTime=citizenData.maxWalkTime;
	}

	#endregion
	#region override_method
	public override void Down()
    {
		SoundManager.Instance.PlayClipToPosition(downClip[Random.Range(0, downClip.Length)], SoundPlayMode.HumanSFX, gameObject.transform.position);
		boxCollider.isTrigger = true;
		rigidbody.isKinematic = true;
		isDown = true;
        base.SetRunaway();
    }

    public override void Rising()
    {
		boxCollider.isTrigger = false;
		rigidbody.isKinematic = false;
		isDown = false;
		transform.LookAt(new Vector3(GameManager.Instance.player.transform.position.x, transform.position.y, GameManager.Instance.player.transform.position.z));
        transform.Rotate(0, 180, 0);
        isRunaway = true;
        //patternChangeTimer = 0.0f;
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
		SetRunaway();
		if(isRunoverByPlayer)
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
			GameManager.Instance.killCount++;
			WorldUIManager.Instance.SetScoreText(transform.position, money);
			WantedLevel.instance.CommitCrime(WantedLevel.CrimeType.killPeople, transform.position);
		}
	}
	#endregion
}