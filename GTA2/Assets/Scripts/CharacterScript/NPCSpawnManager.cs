using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawnManager : MonoSingleton<NPCSpawnManager>
{
    //float carSpawnRange = 10.0f;

    //TODO : Area Manager로 상위에서 관리하게 되면 여러 스폰매니저로 active, deactive 설정예정

    [Header("최초에는 ActiveList에 Pool의 오브젝트 삽입.")]
    public List<NPC> activeNPCList = new List<NPC>();
    public List<NPC> deactiveNPCList = new List<NPC>();

    void Start()
    {
        NPCPositionInit();
    }

    //WayPoints
    public void NPCPositionInit()
    {
        List<Vector3> position = new List<Vector3>();

        for (int i = 0; i < WaypointManager.instance.allWaypointsForHuman.Length; i++)
        {
            position.Add(WaypointManager.instance.allWaypointsForHuman[i].transform.position);
        }
        foreach (var people in activeNPCList)
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
    //TODO : 이후 필요한 클래스로 매개변수 변경
    
        /*
    IEnumerator ActiveObjects()
    {
        while (true)
        {
            CheckDeactiveCitizen();
            CheckDeactiveCar();

            yield return new WaitForSeconds(1.0f);
        }
    }
    IEnumerator DeactiveObjects()
    {
        while (true)
        {
            CheckActiveCitizen();
            CheckActiveCar();

            yield return new WaitForSeconds(1.0f);
        }
    }
    //TODO : 제네릭메소드로 변경
    void CheckDeactiveCitizen()
    {
        List<NPC> tempRemoveCitizenList = new List<NPC>();

        foreach (var citizen in deactiveNPCList)
        {
            if (!IsSpawnRange(citizen.transform.position) && !citizen.gameObject.activeSelf)
            {
                citizen.gameObject.SetActive(true);
                activeNPCList.Add(citizen);
                tempRemoveCitizenList.Add(citizen);
            }
        }
        for(int i = 0; i < tempRemoveCitizenList.Count; i++)
        {
            NPC removeCitizen = tempRemoveCitizenList[i];
            deactiveNPCList.Remove(removeCitizen);
        }
      
    }
    void CheckActiveCitizen()
    {
        List<NPC> tempRemoveCitizenList = new List<NPC>();
        foreach (var citizen in activeNPCList)
        {
            if (IsSpawnRange(citizen.transform.position) && citizen.gameObject.activeSelf)
            {
                citizen.gameObject.SetActive(false);
                deactiveNPCList.Add(citizen);
                tempRemoveCitizenList.Add(citizen);
            }
        }
        for (int i = 0; i < tempRemoveCitizenList.Count; i++)
        {
            NPC removeCitizen = tempRemoveCitizenList[i];
            activeNPCList.Remove(removeCitizen);
        }
    }
    
   
    public bool IsSpawnRange(Vector3 position)
    {
        if (carSpawnRange < Vector3.Distance(position, player.transform.position))
            return true;
        else
            return false;
    }
    */
}