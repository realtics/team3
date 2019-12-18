﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawnManager : MonoSingleton<NPCSpawnManager>
{
	public Citizen citizen;
    public Police police;
	public Doctor doctor;
	public List<GameObject> allNPC;
	public NPCSpawnData npcSpawnData;

	float citizenSpawnInterval;
	float policeSpawnInterval;

	int maxSpawnCitizenNumInInterval;
    int maxSpawnPoliceNumInInterval;
	int maximumNPCNum;
	float commitRadius = 0.3f;
	public int NPCNum;

	public List<NPC> DiedNPC;

	public GameObject BloodAnim;
	public List<GameObject> BloodAnimList;

	void Awake()
	{
		PoolManager.WarmPool(citizen.gameObject, 150);
        PoolManager.WarmPool(police.gameObject, 100);
		PoolManager.WarmPool(doctor.gameObject, 10);

		allNPC.AddRange(PoolManager.GetAllObject(citizen.gameObject));
        allNPC.AddRange(PoolManager.GetAllObject(police.gameObject));
		allNPC.AddRange(PoolManager.GetAllObject(doctor.gameObject));

		PoolManager.WarmPool(BloodAnim.gameObject, 10);
		BloodAnimList.AddRange(PoolManager.GetAllObject(BloodAnim.gameObject));
	}
	void Start()
	{
		MasterDataInit();
		//코루틴으로 주기적으로 플레이어 근처 소환
		StartCoroutine(SpawnCitizen());
		StartCoroutine(SpawnPolice());
	}
	
    IEnumerator SpawnCitizen()
    {
		while(true)
		{
			yield return new WaitForSeconds(citizenSpawnInterval);

			int spawnNum = Random.Range(0, maxSpawnCitizenNumInInterval);

			for (int i = 0; i < spawnNum; i++)
			{
				GameObject closeWayPoint = WaypointManager.instance.FindRandomNPCSpawnPosition();

				if (closeWayPoint == null || NPCNum >= maximumNPCNum)
					continue;
				NPCNum++;

				GameObject insNPC = PoolManager.SpawnObject(citizen.gameObject);
				insNPC.transform.position = new Vector3(closeWayPoint.transform.position.x + Random.Range(-commitRadius, commitRadius), closeWayPoint.transform.position.y, closeWayPoint.transform.position.z + Random.Range(-commitRadius, commitRadius));
			}
		}
    }
	IEnumerator SpawnPolice()
	{
		while (true)
		{
			yield return new WaitForSeconds(policeSpawnInterval);

			int spawnNum = Random.Range(0, maxSpawnPoliceNumInInterval);

			for (int i = 0; i < spawnNum; i++)
			{
				GameObject closeWayPoint = WaypointManager.instance.FindRandomNPCSpawnPosition();

				if (closeWayPoint == null || NPCNum >= maximumNPCNum)
					continue;
				NPCNum++;

				//Police
				closeWayPoint = WaypointManager.instance.FindRandomNPCSpawnPosition();

				if (closeWayPoint == null)
					continue;
				GameObject insNPC = PoolManager.SpawnObject(police.gameObject);
				insNPC.transform.position = new Vector3(closeWayPoint.transform.position.x + Random.Range(-commitRadius, commitRadius), closeWayPoint.transform.position.y, closeWayPoint.transform.position.z + Random.Range(-commitRadius, commitRadius));
			}
		}
	}
	
	void MasterDataInit()
	{
		citizenSpawnInterval = npcSpawnData.citizenSpawnInterval;
		maximumNPCNum = npcSpawnData.maximumNPCNum;
		policeSpawnInterval = npcSpawnData.policeSpawnInterval;

		maxSpawnCitizenNumInInterval = npcSpawnData.maxSpawnCitizenNumInInterval;
		maxSpawnPoliceNumInInterval = npcSpawnData.maxSpawnPoliceNumInInterval;
	}
}