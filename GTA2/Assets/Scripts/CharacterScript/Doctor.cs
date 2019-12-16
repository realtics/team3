using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doctor : NPC
{
	public DoctorData doctorData;
	NPC[] healTargets;
	public List<NPC> DiedNPC;
	float HealTime = 3.0f;
	float HealTimer = 0.0f;
	CarManager embulanceCar; //타고온 차량

	enum DoctorState
	{
		GoToheal,
		Healing,
		GoBackToCar
	}
	DoctorState doctorState = DoctorState.GoToheal;
	void Awake()
	{
		gameManager = GameManager.Instance;
	}

	void Start()
	{
		MasterDataInit();
		StartCoroutine(GetDiedNPC());
	}

	// Update is called once per frame
	void Update()
	{
		base.PeopleUpdate();
		base.NPCUpdate();
	}
	void FixedUpdate()
	{
		if (isDie || isDown)
			return;
		ActivityByState();
	}
	IEnumerator GetDiedNPC() //OnEnable로 이동
	{
		while (true)
		{
			if(DiedNPC != null)
				DiedNPC.Clear();
			foreach (GameObject npcGameObect in NPCSpawnManager.Instance.allNPC)
			{
				NPC npc = npcGameObect.GetComponent<NPC>();

				if (npc.isDie)
				{
					DiedNPC.Add(npc);
				}
			}
			//거리 순정렬
			for(int i = 0; i < DiedNPC.Count; i++)
			{
				for (int j = i; j < DiedNPC.Count - (i + 1); j++)
				{
					if(Vector3.SqrMagnitude(transform.position - DiedNPC[j].transform.position) > Vector3.SqrMagnitude(transform.position - DiedNPC[j + 1].transform.position))
					{
						NPC temp = DiedNPC[j];
						DiedNPC[j] = DiedNPC[j + 1];
						DiedNPC[j + 1] = temp;
					}
				}
			}

			foreach(NPC npc in DiedNPC)
			{
				print(Vector3.SqrMagnitude(transform.position - npc.transform.position));
			}
			yield return new WaitForSeconds(5.0f);
		}
	}
	
	void ActivityByState()
	{
		switch (doctorState)
		{
			case DoctorState.GoToheal:
				if(DiedNPC.Count != 0)//테스트 용
				{
					//살아난 사람은 제거
					//if (!DiedNPC[0].isDie)
					//{
					//	DiedNPC.RemoveAt(0);
					//}
					transform.LookAt(DiedNPC[0].transform.position);
					Move();
					if (MathUtil.isArrived(transform.position, DiedNPC[0].transform.position))
						doctorState = DoctorState.Healing;
				}
				break;
			case DoctorState.Healing:
				isWalk = false;
				HealTimerCheck();
				break;
			case DoctorState.GoBackToCar:
				Move();
				break;
		}
	}
	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Car") && isWalk)
		{
			Jump();
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
	override protected void Move()
	{
		isWalk = true;
		transform.Translate(transform.forward * moveSpeed * Time.deltaTime, Space.World);
	}
	void HealTimerCheck()
	{
		HealTimer += Time.deltaTime;

		if(HealTimer > HealTime)
		{
			HealTimer = 0.0f;
			DiedNPC[0].Respawn();
			DiedNPC.Remove(DiedNPC[0]);

			if (DiedNPC.Count == 0)
			{
				doctorState = DoctorState.GoBackToCar;
			}
			else
			{
				doctorState = DoctorState.GoToheal;
			}
		}
	}
	//힐하러 Move
	//힐
	//끝나면 차타기

	#endregion
}
