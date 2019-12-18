using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doctor : NPC
{
	public DoctorData doctorData;
	public NPC targetNPC;
	float HealTime = 3.0f;
	float HealTimer = 0.0f;
	public CarManager ambulanceCar;
	public int idx = 0;
	
	public enum DoctorState
	{
		GoToheal,
		Healing,
		GoBackToCar
	}
	public DoctorState doctorState = DoctorState.GoToheal;
	void Awake()
	{
		gameManager = GameManager.Instance;
		MasterDataInit();
	}
	private void OnEnable()
	{
		GetDiedNPC();
	}
	private void OnDisable()
	{
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
	bool GetDiedNPC() //OnEnable로 이동
	{
		if (NPCSpawnManager.Instance.DiedNPC.Count != 0)
		{
			targetNPC = NPCSpawnManager.Instance.DiedNPC[0];
			NPCSpawnManager.Instance.DiedNPC.Remove(targetNPC);
			return true;
		}
		else
			return false;
	}
	
	void ActivityByState()
	{
		switch (doctorState)
		{
			case DoctorState.GoToheal:
				if (targetNPC == null)
				{
					doctorState = DoctorState.GoBackToCar;
					return;
				}
				transform.LookAt(new Vector3(targetNPC.transform.position.x, transform.position.y, targetNPC.transform.position.z));
				
				if (MathUtil.isArrived(new Vector3(transform.position.x, ambulanceCar.transform.position.y, transform.position.z), ambulanceCar.transform.position))
				{
					if(isJump)
						Land();
					doctorState = DoctorState.Healing;
				}
				else
					Move();
				break;
			case DoctorState.Healing:
				isWalk = false;
				HealTimerCheck();
				break;
			case DoctorState.GoBackToCar:
				
				if (MathUtil.isArrived(new Vector3(transform.position.x, ambulanceCar.transform.position.y, transform.position.z), ambulanceCar.transform.position))
				{
					isWalk = false;
					if (isJump)
						Land();
					OpenTheDoor(idx);
				}
				else
				{
					transform.LookAt(new Vector3(ambulanceCar.passengerManager.doorPositions[idx].transform.position.x, transform.position.y, ambulanceCar.passengerManager.doorPositions[idx].transform.position.z));
					Move();
				}
				break;
		}
	}
	void OpenTheDoor(int idx = 0)
	{
		if (!ambulanceCar.passengerManager.isRunningOpenTheDoor[idx])//문열기 셋팅
		{
			transform.forward = ambulanceCar.transform.forward;
			ambulanceCar.passengerManager.isRunningOpenTheDoor[idx] = true;
			isGetOnTheCar = true;
			StartCoroutine(ambulanceCar.passengerManager.OpenTheDoor(idx));
		}
		else//문열기
		{
			if (ambulanceCar.passengerManager.isDoorOpen[idx])//탑승
			{
				ambulanceCar.passengerManager.GetOnTheCar(PeopleType.Doctor, idx);
				gameObject.SetActive(false);
			}
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
			targetNPC.Respawn();
			targetNPC = null;

			if (GetDiedNPC())
			{
				doctorState = DoctorState.GoToheal;
			}
			else
			{
				doctorState = DoctorState.GoBackToCar;
			}
		}
	}
	
	//void SortByDistance(bool isAsc = true)
	//{
	//	if (isAsc)
	//	{
	//		for (int i = 0; i < DiedNPC.Count; i++)
	//		{
	//			for (int j = i; j < DiedNPC.Count - (i + 1); j++)
	//			{
	//				if (Vector3.SqrMagnitude(transform.position - DiedNPC[j].transform.position) > 
	//					Vector3.SqrMagnitude(transform.position - DiedNPC[j + 1].transform.position))
	//				{
	//					NPC temp = DiedNPC[j];
	//					DiedNPC[j] = DiedNPC[j + 1];
	//					DiedNPC[j + 1] = temp;
	//				}
	//			}
	//		}
	//	}
	//	else
	//	{
	//		for (int i = 0; i < DiedNPC.Count; i++)
	//		{
	//			for (int j = i; j < DiedNPC.Count - (i + 1); j++)
	//			{
	//				if (Vector3.SqrMagnitude(transform.position - DiedNPC[j].transform.position) < 
	//					Vector3.SqrMagnitude(transform.position - DiedNPC[j + 1].transform.position))
	//				{
	//					NPC temp = DiedNPC[j];
	//					DiedNPC[j] = DiedNPC[j + 1];
	//					DiedNPC[j + 1] = temp;
	//				}
	//			}
	//		}
	//	}
	//}
	#endregion
}
