using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawnManager : MonoSingleton<NPCSpawnManager>
{
	public Citizen citizen;
    public Police police;
    public List<GameObject> allNPC;
	public NPCSpawnData npcSpawnData;
	float spawnInterval;
    int spawnCitizenNumInInterval;
    int spawnPoliceNumInInterval;
	int maximumNPCNum;
	public int NPCNum;

    void Awake()
	{
		PoolManager.WarmPool(citizen.gameObject, 50);
        PoolManager.WarmPool(police.gameObject, 30);

        allNPC.AddRange(PoolManager.GetAllObject(citizen.gameObject));
        allNPC.AddRange(PoolManager.GetAllObject(police.gameObject));
	}

	void Start()
	{
		MasterDataInit();
		//코루틴으로 주기적으로 플레이어 근처 소환
		StartCoroutine(SpawnNPC());
		
	}
	public void NPCRespawn()
	{
		foreach(GameObject npc in allNPC)
		{
			npc.SetActive(false);
		}
	}
    IEnumerator SpawnNPC()
    {
		while(true)
		{
			yield return new WaitForSeconds(spawnInterval);

			//Citizen
			GameObject closeWayPoint = WaypointManager.instance.FindRandomNPCSpawnPosition();

			if (closeWayPoint == null || NPCNum >= maximumNPCNum)
				continue;
			NPCNum++;

			GameObject insNPC = PoolManager.SpawnObject(citizen.gameObject);
			insNPC.transform.position = closeWayPoint.transform.position;

			//Police
			closeWayPoint = WaypointManager.instance.FindRandomNPCSpawnPosition();

			if (closeWayPoint == null)
				continue;
			insNPC = PoolManager.SpawnObject(police.gameObject);
			insNPC.transform.position = new Vector3(closeWayPoint.transform.position.x + Random.Range(-0.5f, 0.5f), closeWayPoint.transform.position.y, closeWayPoint.transform.position.z + Random.Range(-0.5f, 0.5f));
		}
    }
	void MasterDataInit()
	{
		spawnInterval = npcSpawnData.spawnInterval;
		spawnCitizenNumInInterval = npcSpawnData.spawnCitizenNumInInterval;
		spawnPoliceNumInInterval = npcSpawnData.spawnPoliceNumInInterval;
		maximumNPCNum = npcSpawnData.maximumNPCNum;
	}
}