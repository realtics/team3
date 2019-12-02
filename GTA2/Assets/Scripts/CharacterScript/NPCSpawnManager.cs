using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawnManager : MonoSingleton<NPCSpawnManager>
{
	//float carSpawnRange = 10.0f;
	//TODO : Area Manager로 상위에서 관리하게 되면 여러 스폰매니저로 active, deactive 설정예정
	public GameObject Citizen;
	public GameObject Police;
	public GameObject[] allNPC;

	void Awake()
	{
		PoolManager.WarmPool(Citizen, 10);
		PoolManager.WarmPool(Police, 10);

		allNPC = GameObject.FindGameObjectsWithTag("NPC");

		foreach (var npc in allNPC)
		{
			npc.SetActive(false);
		}
	}

	void Start()
	{
		//InvokeRepeating("RespawnDisabledNPC", 0.25f, 0.25f);

		foreach (var npc in allNPC)
		{
			npc.transform.position = WaypointManager.instance.allWaypointsForHuman[Random.Range(0, allNPC.Length)].transform.position;
			npc.SetActive(true);
		}
	}

	void RespawnDisabledNPC()
	{
		foreach (var npc in allNPC)
		{
			if (npc.gameObject.activeSelf)
				continue;
			GameObject go = WaypointManager.instance.FindRandomCarSpawnPosition();

			Ray ray = new Ray(go.transform.position + (Vector3.up * 5), Vector3.down);
			RaycastHit hit;
			if (Physics.SphereCast(ray, 2f, out hit, 10, 1 << 12))
			{
				Debug.DrawLine(GameManager.Instance.player.transform.position, go.transform.position, Color.red, 0.5f);
			}
			else
			{
				npc.transform.position = go.transform.position;
				npc.gameObject.SetActive(true);
				npc.GetComponent<CarManager>().movement.curSpeed = 100;

				Debug.DrawLine(GameManager.Instance.player.transform.position, npc.transform.position, Color.green, 0.5f);
			}

			break;
		}
	}

	public GameObject SpawnPolice()
	{
		return PoolManager.SpawnObject(Police);
	}

	public void NPCRepositioning(NPC npc)
    {
        int randomIndex = Random.Range(0, WaypointManager.instance.allWaypointsForHuman.Length);
        npc.gameObject.transform.position = WaypointManager.instance.allWaypointsForHuman[randomIndex].transform.position;
    }
    //public NPC GetDriver() //드라이버 반환
    //{
    //    int i = 0;
    //    for (i = 0; i < activeCitizenList.Count; i++)
    //    {
    //        //FIX ME : 경찰 아닌걸로 바꿔야 함
    //        if (!activeCitizenList[i].gameObject.activeSelf)
    //        {
    //            return activeCitizenList[i];
    //        }
    //    }
    //    return activeCitizenList[i];
    //}

    //void RespawnDisabledPeople()
    //{
    //    foreach (var pop in activeCitizenList)
    //    {
    //        if (pop.gameObject.activeSelf)
    //            continue;

    //        GameObject go = WaypointManager.instance.FindRandomWaypointOutOfCameraView(WaypointManager.WaypointType.human);

    //        pop.transform.position = go.transform.position;
    //        pop.gameObject.SetActive(true);

    //        break;
    //    }
    //}
}