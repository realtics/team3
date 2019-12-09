using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawnManager : MonoSingleton<NPCSpawnManager>
{
	public Citizen citizen;
    public Police police;
    public List<GameObject> allNPC;
    public float despawnInterval = 1.0f;
    public float spawnInterval = 0.5f;
    public int spawnCitizenNumInInterval = 4;
    public int spawnPoliceNumInInterval = 3;
	public int maximumNPCNum = 10;
	public int NPCNum = 0;

    void Awake()
	{
		PoolManager.WarmPool(citizen.gameObject, 50);
        PoolManager.WarmPool(police.gameObject, 30);

        allNPC.AddRange(PoolManager.GetAllObject(citizen.gameObject));
        allNPC.AddRange(PoolManager.GetAllObject(police.gameObject));
	}

	void Start()
	{
		//코루틴으로 주기적으로 플레이어 근처 소환
		InvokeRepeating(nameof(SpawnNPC), spawnInterval, spawnInterval);
	}
	
    void SpawnNPC()
    {
		//Citizen
        GameObject closeWayPoint = WaypointManager.instance.FindRandomNPCSpawnPosition();
		//Debug.DrawLine(GameManager.Instance.player.transform.position, closeWayPoint.transform.position, Color.red, 0.5f);

		if (closeWayPoint == null || NPCNum >= maximumNPCNum)
            return;
		NPCNum++;

		GameObject insNPC = PoolManager.SpawnObject(citizen.gameObject);
        insNPC.transform.position = closeWayPoint.transform.position;

		//Police
        closeWayPoint = WaypointManager.instance.FindRandomNPCSpawnPosition();

        if (closeWayPoint == null)
            return;
		insNPC = PoolManager.SpawnObject(police.gameObject);
        insNPC.transform.position = closeWayPoint.transform.position;
    }
}