using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private float carSpawnRange = 10.0f;
    public int spawnAmount = 20;
    public GameObject carPrefab;

    //TODO : NPC로 수정예정
    public List<Citizen> activeCitizenList = new List<Citizen>();
    public List<Citizen> deactiveCitizenList = new List<Citizen>();

    public List<GameObject> activeCarList = new List<GameObject>();
    public List<GameObject> deactiveCarList = new List<GameObject>();

    //TODO : 사람 웨이 포인트 삽입
    //자동차 풀 만들고 Instantiate 삭제
    public Vector3[] npcSpawnPoint;
    public Vector3[] carSpawnPoint;

    [SerializeField]
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        Init();
        player = GameObject.FindWithTag("Player");
        StartCoroutine(ActiveObjects());
        StartCoroutine(DeactiveObjects());
    }
    void Init()
    {
        //오브젝트 최초 생성
        //SpawnCitizen(Random.Range(1, 10));
        SpawnCar();
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
        List<Citizen> tempRemoveCitizenList = new List<Citizen>();

        foreach (var citizen in deactiveCitizenList)
        {
            if (!IsSpawnRange(citizen.transform.position) && !citizen.gameObject.activeSelf)
            {
                citizen.gameObject.SetActive(true);
                activeCitizenList.Add(citizen);
                tempRemoveCitizenList.Add(citizen);
            }
        }
        foreach(var removeCitizen in tempRemoveCitizenList)
        {
            deactiveCitizenList.Remove(removeCitizen);
        }
    }
    void CheckActiveCitizen()
    {
        List<Citizen> tempRemoveCitizenList = new List<Citizen>();
        foreach (var citizen in activeCitizenList)
        {
            if (IsSpawnRange(citizen.transform.position) && citizen.gameObject.activeSelf)
            {
                citizen.gameObject.SetActive(false);
                deactiveCitizenList.Add(citizen);
                tempRemoveCitizenList.Add(citizen);
            }
        }
        foreach (var removeCitizen in tempRemoveCitizenList)
        {
            activeCitizenList.Remove(removeCitizen);
        }
    }
    void CheckDeactiveCar()
    {
        List<GameObject> tempRemoveCarList = new List<GameObject>();

        foreach (var car in deactiveCarList)
        {
            if (!IsSpawnRange(car.transform.position) && !car.gameObject.activeSelf)
            {
                car.gameObject.SetActive(true);
                activeCarList.Add(car);
                tempRemoveCarList.Add(car);
            }
        }
        foreach (var removeCar in tempRemoveCarList)
        {
            tempRemoveCarList.Remove(removeCar);
        }
    }
    void CheckActiveCar()
    {
        List<GameObject> tempRemoveCitizenList = new List<GameObject>();

        foreach (var car in activeCarList)
        {
            if (IsSpawnRange(car.transform.position) && car.gameObject.activeSelf)
            {
                car.gameObject.SetActive(false);
                deactiveCarList.Add(car);
                tempRemoveCitizenList.Add(car);
            }
        }
        foreach (var removeCitizen in tempRemoveCitizenList)
        {
            activeCarList.Remove(removeCitizen);
        }
    }
    void SpawnCar()
    {
        GameObject[] allWP = GameObject.FindGameObjectsWithTag("waypoint");

        for (int i = 0; i < spawnAmount; i++)
        {
            GameObject go = Instantiate(carPrefab);
            go.transform.position = allWP[Random.Range(0, allWP.Length)].transform.position;
            go.transform.parent = transform;
            activeCarList.Add(go);
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