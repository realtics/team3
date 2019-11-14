using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private float spawnRange = 10.0f;

    public List<Citizen> activeCitizenList = new List<Citizen>();
    public List<Citizen> deactiveCitizenList = new List<Citizen>();

    public List<Police> activePoliceList = new List<Police>();
    public List<Police> deactivePoliceList = new List<Police>();

    public List<Doctor> activeDoctorList = new List<Doctor>();
    public List<Doctor> deactiveDoctorList = new List<Doctor>();

    public List<GameObject> activeCarList = new List<GameObject>();
    public List<GameObject> deactiveCarList = new List<GameObject>();

    public GameObject citizenPrefab;
    public GameObject policePrefab;
    public GameObject doctorPrefab;

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
    }
    IEnumerator ActiveObjects()
    {
        while (true)
        {
            CheckDeactiveCitizen();
            //CheckDeactiveCar();

            yield return new WaitForSeconds(1.0f);
        }
    }
    IEnumerator DeactiveObjects()
    {
        while (true)
        {
            CheckActiveCitizen();
            //CheckActiveCar();

            yield return new WaitForSeconds(1.0f);
        }
         
    }
    void CheckDeactiveCitizen()
    {
        List<Citizen> tempRemoveCitizenList = new List<Citizen>();

        foreach (var citizen in deactiveCitizenList)
        {
            if (IsSpawnRange(citizen.transform.position) && !citizen.gameObject.activeSelf)
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
            deactiveCitizenList.Remove(removeCitizen);
        }
    }
    //void CheckDeactiveCar()
    //void CheckActiveCar()

    public bool IsSpawnRange(Vector3 position)
    {
        if (spawnRange < Vector3.Distance(position, player.transform.position))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}