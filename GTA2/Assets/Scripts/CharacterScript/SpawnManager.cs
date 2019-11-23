using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    float carSpawnRange = 10.0f;

    [Header("최초에는 ActiveList에 Pool의 오브젝트 삽입.")]
    public List<NPC> activeNPCList = new List<NPC>();
    public List<NPC> deactiveNPCList = new List<NPC>();
    
    public List<CarController> activeCarList = new List<CarController>();
    public List<CarController> deactiveCarList = new List<CarController>();

    public List<NPC> carDriverPool = new List<NPC>();

    [SerializeField]
    GameObject player;

    void Start()
    {
        Init();
        player = GameObject.FindWithTag("Player");
        //StartCoroutine(ActiveObjects());
        //StartCoroutine(DeactiveObjects());
    }
    public void Init()
    {
        //오브젝트 최초 생성
        NPCPositionInit();
        CarPositionInit();
    }
    //WayPoints
    void NPCPositionInit()
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
    void CarPositionInit() //중복 방지 생성
    {
        List<Vector3> position = new List<Vector3>();

        for (int i = 0; i < WaypointManager.instance.allWaypointsForCar.Length; i++)
        {
            position.Add(WaypointManager.instance.allWaypointsForCar[i].transform.position);
        }

        foreach (var car in activeCarList)
        {
            int randomIndex = Random.Range(0, position.Count);
            //print(randomIndex + " " + position.Count + " " + );
            car.gameObject.transform.position = position[randomIndex];
            car.gameObject.SetActive(false);
            car.gameObject.SetActive(true);
            position.RemoveAt(randomIndex);
        }
    }
    
    public void NPCRepositioning(NPC npc)
    {
        int randomIndex = Random.Range(0, WaypointManager.instance.allWaypointsForHuman.Length);
        npc.gameObject.transform.position = WaypointManager.instance.allWaypointsForHuman[randomIndex].transform.position;
    }
    //TODO : 이후 필요한 클래스로 매개변수 변경
    public void CarRepositioning(CarDamage car)
    {
        int randomIndex = Random.Range(0, WaypointManager.instance.allWaypointsForCar.Length);
        car.gameObject.transform.position = WaypointManager.instance.allWaypointsForCar[randomIndex].transform.position;
    }
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
    void CheckDeactiveCar()
    {
        List<CarController> tempRemoveCarList = new List<CarController>();

        foreach (var car in deactiveCarList)
        {
            if (!IsSpawnRange(car.transform.position) && !car.gameObject.activeSelf)
            {
                car.gameObject.SetActive(true);
                activeCarList.Add(car);
                tempRemoveCarList.Add(car);
            }
        }
        for (int i = 0; i < tempRemoveCarList.Count; i++)
        {
            CarController removeCar = tempRemoveCarList[i];
            deactiveCarList.Remove(removeCar);
        }
    }
    void CheckActiveCar()
    {
        List<CarController> tempRemoveCarList = new List<CarController>();

        foreach (var car in activeCarList)
        {
            if (IsSpawnRange(car.transform.position) && car.gameObject.activeSelf)
            {
                car.gameObject.SetActive(false);
                deactiveCarList.Add(car);
                tempRemoveCarList.Add(car);
            }
        }
        for (int i = 0; i < tempRemoveCarList.Count; i++)
        {
            CarController removeCar = tempRemoveCarList[i];
            activeCarList.Remove(removeCar);
        }
    }
   
    public bool IsSpawnRange(Vector3 position)
    {
        if (carSpawnRange < Vector3.Distance(position, player.transform.position))
            return true;
        else
            return false;
    }
}