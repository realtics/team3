using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Citizen : NPC
{
	public CitizenData citizenData;
	public SpriteRenderer ClothSpriteRenderer;
	public AudioClip[] downClip;
	//static float shoutTime = 10.0f;
	//static float shoutTimer = 0.0f;

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
		if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Car") && isRunaway)
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
		base.Runover(runoverSpeed, carPosition, isRunoverByPlayer);
		SetRunaway();
	}
	#endregion
}