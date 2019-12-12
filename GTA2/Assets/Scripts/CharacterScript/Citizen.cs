using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

public class Citizen : NPC
{
	public CitizenData citizenData;
    void Awake()
    {
		gameManager = GameManager.Instance;
	}

    void Start()
    {
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
            return;

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
            base.Move();
        }
    }
	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Wall" && collision.gameObject.tag == "Car" && isRunaway)
		{
			transform.Rotate(0, Random.Range(90, 270), 0);
		}
	}
	#region lowlevelCode
	IEnumerator ActivityByState()
    {
        while(true)
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
    void TimerCheck()
    {
        patternChangeTimer += Time.deltaTime;

        if (DetectedPlayerAttack())
        {
            base.SetRunaway();
        }
        else
            PatternChange();
    }
	void MasterDataInit()
	{
		defaultHp = citizenData.maxHp;
		moveSpeed = citizenData.moveSpeed;
		downTime = citizenData.downTime;
		runSpeed = citizenData.runawaySpeed;
		findRange = citizenData.findRange;
		punchRange = citizenData.punchRange;
		shotRange= citizenData.shotRange;
		chaseRange=citizenData.chaseRange;
		outofRange=citizenData.outofRange;
		minIdleTime=citizenData.minIdleTime;
		maxIdleTime=citizenData.maxIdleTime;
		minWalkTime=citizenData.minWalkTime;
		maxWalkTime=citizenData.maxWalkTime;
		carOpenTimer=citizenData.carOpenTimer;
		carOpenTime=citizenData.carOpenTime;
		runawayTime=citizenData.runawayTime;
		money= citizenData.money;
	}

	#endregion
	#region override_method
	public override void Down()
    {
		boxCollider.isTrigger = true;
		rigidbody.isKinematic = true;
		isDown = true;
        base.SetRunaway();
    }

    public override void Rising()
    {
        transform.LookAt(new Vector3(GameManager.Instance.player.transform.position.x, transform.position.y, GameManager.Instance.player.transform.position.z));
        transform.Rotate(0, 180, 0);
        isRunaway = true;
        patternChangeTimer = 0.0f;
    }
	public override void Runover(float runoverSpeed, Vector3 carPosition)
	{
		Vector3 runoverVector = transform.position - carPosition;

		if (runoverSpeed < 50)
			return;

		//속도에 비례한 피해 데미지 보정수치
		this.runoverSpeed = Mathf.Clamp((runoverSpeed / 3000.0f), 0, 0.3f);
		this.runoverVector = (runoverVector.normalized * this.runoverSpeed * Mathf.Abs(Vector3.Dot(runoverVector, Vector3.right)));
		isRunover = true;
		hDir = 0; vDir = 0;

		transform.LookAt(carPosition);

		if (isDown && runoverSpeed > 30)
		{
			Hurt((int)(runoverSpeed * 4));
		}
		else if (runoverSpeed > 200)
		{
			Hurt((int)(runoverSpeed / 4));
		}
		SetRunaway();
	}
	#endregion
}