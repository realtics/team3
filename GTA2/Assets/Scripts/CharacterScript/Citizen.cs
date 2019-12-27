using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Citizen : NPC
{
	public CitizenData citizenData;
	public SpriteRenderer ClothSpriteRenderer;
	public AudioClip[] downClip;
	void Awake()
    {
		base.TimerInit();
		MasterDataInit();
	}

	private void OnEnable()
	{
		base.NPCOnEnable();
		ClothesColorRandomSetting();
		StartCoroutine(Raycast());
	}
	
	private void OnDisable()
	{
		base.NPCOnDisable();
		StopCoroutine(Raycast());
	}
	void Update()
    {
		base.PeopleUpdate();
		base.NPCUpdate();

		if (isDie || isDown)
            return;
		
		if (DetectedPlayerAttack())
		{
			base.SetRunaway();
		}
		else
			PatternChangeTimerCheck();
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
			base.Move();
		}
	}
	
	void OnCollisionStay(Collision collision)
	{
		if (isRunaway && (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Car")))
		{
			transform.Rotate(0, Random.Range(90, 270), 0);
		}
	}
	#region lowlevelCode
	
	void ClothesColorRandomSetting()
	{
		ClothSpriteRenderer.color = new Color(Random.Range(0, 1.0f), Random.Range(0, 1.0f), Random.Range(0, 1.0f));
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
		base.Timers[(int)TimerType.JumpMin] = 1.0f;

		minIdleTime =citizenData.minIdleTime;
		maxIdleTime=citizenData.maxIdleTime;
		minWalkTime=citizenData.minWalkTime;
		maxWalkTime=citizenData.maxWalkTime;
	}

	#endregion
	#region override_method
	public override void Down()
    {
		base.Down();
        base.SetRunaway();
		SoundManager.Instance.PlayClipToPosition(downClip[Random.Range(0, downClip.Length)], SoundPlayMode.HumanSFX, gameObject.transform.position);
	}

    public override void Rising()
    {
		base.Rising();

		transform.LookAt(new Vector3(GameManager.Instance.player.transform.position.x, transform.position.y, GameManager.Instance.player.transform.position.z));
        transform.Rotate(0, 180, 0);
        isRunaway = true;

    }
	public override void Runover(float runoverSpeed, Vector3 carPosition, bool isRunoverByPlayer = false)
	{
		if (runoverSpeed < runoverMinSpeed)
			return;
		else if (isDown)
		{
			Hurt((int)(runoverSpeed * 5));
		}

		Vector3 runoverVector = transform.position - carPosition;

		//속도에 비례한 피해 데미지 보정수치
		this.runoverSpeed = Mathf.Clamp((runoverSpeed / 3000.0f), 0, 0.3f);
		this.runoverVector = (runoverVector.normalized * this.runoverSpeed * Mathf.Abs(Vector3.Dot(runoverVector, Vector3.right)));
		isRunover = true;
		hDir = 0; vDir = 0;

		transform.LookAt(carPosition);

		if (runoverSpeed > runoverHurtMinSpeed)
		{
			Hurt((int)(runoverSpeed / 3));
			SetRunaway();
		}
		if (isRunoverByPlayer && isDie)
		{
			GameManager.Instance.IncreaseMoney(money);
			GameManager.Instance.killCount++;
			WorldUIManager.Instance.SetScoreText(transform.position, money);
			WantedLevel.instance.CommitCrime(WantedLevel.CrimeType.killPeople, transform.position);

			SoundManager.Instance.PlayClipToPosition(squashClip, SoundPlayMode.ObjectSFX, transform.position);
		}
		
	}
	#endregion
}