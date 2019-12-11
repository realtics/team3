using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doctor : NPC
{
	DoctorData doctorData;
	NPC[] healTargets;
    // Start is called before the first frame update
    public Animator anim;
	bool isHeal = false;
	float HealTime;

	//죽은 사람 목표타겟으로 설정해서 치료
	//끝나면 다시 자동차 타고 가던길 가기

	void Awake()
	{
		gameManager = GameManager.Instance;
	}

	void Start()
	{
		MasterDataInit();
		base.NPCInit();
	}

    // Update is called once per frame
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
		if (isWalk)
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
	void MasterDataInit()
	{
		defaultHp = doctorData.maxHp;
		moveSpeed = doctorData.moveSpeed;
		downTime = doctorData.downTime;
		findRange = doctorData.findRange;
		punchRange = doctorData.punchRange;
		shotRange = doctorData.shotRange;
		minIdleTime = doctorData.minIdleTime;
		maxIdleTime = doctorData.maxIdleTime;
		minWalkTime = doctorData.minWalkTime;
		maxWalkTime = doctorData.maxWalkTime;
		carOpenTimer = doctorData.carOpenTimer;
		carOpenTime = doctorData.carOpenTime;
		money = doctorData.money;
	}
	void TimerCheck()
	{
		patternChangeTimer += Time.deltaTime;
	}
	//힐하러 Move
	//힐
	//끝나면 차타기

	#endregion
}
