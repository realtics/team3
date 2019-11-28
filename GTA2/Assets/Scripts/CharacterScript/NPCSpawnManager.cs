using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawnManager : MonoSingleton<NPCSpawnManager>
{
    //float carSpawnRange = 10.0f;

    //TODO : Area Manager로 상위에서 관리하게 되면 여러 스폰매니저로 active, deactive 설정예정

    [Header("최초에는 ActiveList에 Pool의 오브젝트 삽입.")]
    public List<Citizen> activeCitizenList = new List<Citizen>();
    public List<Police> activePoliceList = new List<Police>();
    void Start()
    {
        CitizenPositionInit();
        PolicePositionInit();
        InvokeRepeating("RespawnDisabledPeople", 0.5f, 0.5f);
    }

    //WayPoints
    public void CitizenPositionInit()
    {
        List<Vector3> position = new List<Vector3>();

        for (int i = 0; i < WaypointManager.instance.allWaypointsForHuman.Length; i++)
        {
            position.Add(WaypointManager.instance.allWaypointsForHuman[i].transform.position);
        }
        foreach (var people in activeCitizenList)
        {
            int randomIndex = Random.Range(0, position.Count);
            people.gameObject.transform.position = WaypointManager.instance.allWaypointsForHuman[randomIndex].transform.position;
            people.gameObject.SetActive(false);
            people.gameObject.SetActive(true);
            position.RemoveAt(randomIndex);
        }
    }
    public void PolicePositionInit()
    {
        List<Vector3> position = new List<Vector3>();

        for (int i = 0; i < WaypointManager.instance.allWaypointsForHuman.Length; i++)
        {
            position.Add(WaypointManager.instance.allWaypointsForHuman[i].transform.position);
        }
        foreach (var people in activePoliceList)
        {
            int randomIndex = Random.Range(0, position.Count);
            people.gameObject.transform.position = WaypointManager.instance.allWaypointsForHuman[randomIndex].transform.position;
            people.gameObject.SetActive(false);
            people.gameObject.SetActive(true);
            position.RemoveAt(randomIndex);
        }
    }
    public void NPCRepositioning(NPC npc)
    {
        int randomIndex = Random.Range(0, WaypointManager.instance.allWaypointsForHuman.Length);
        npc.gameObject.transform.position = WaypointManager.instance.allWaypointsForHuman[randomIndex].transform.position;
    }
    public NPC GetDriver() //드라이버 반환
    {
        int i = 0;
        for (i = 0; i < activeCitizenList.Count; i++)
        {
            //FIX ME : 경찰 아닌걸로 바꿔야 함
            if (!activeCitizenList[i].gameObject.activeSelf)
            {
                return activeCitizenList[i];
            }
        }
        return activeCitizenList[i];
    }

    void RespawnDisabledPeople()
    {
        foreach (var pop in activeCitizenList)
        {
            if (pop.gameObject.activeSelf)
                continue;

            GameObject go = WaypointManager.instance.FindRandomWaypointOutOfCameraView(WaypointManager.WaypointType.human);

            pop.transform.position = go.transform.position;
            pop.gameObject.SetActive(true);

            break;
        }
    }
}