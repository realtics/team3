using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private float spawnRange = 30.0f;

    public List<Citizen> citizenList = new List<Citizen>();
    public List<Police> policeList = new List<Police>();
    public List<Doctor> doctorList = new List<Doctor>();
    public GameObject citizenPrefab;
    public GameObject policePrefab;
    public GameObject doctorPrefab;

    [SerializeField]
    GameObject player;

    float Timer = 0.0f;//테스트 용
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //3초마다 1명 생성 (테스트)
        /*
        Timer += Time.deltaTime;

        if(Timer > 3.0f)
        {
            Timer = 0.0f;
            SpawnCitizen(1);
        }*/
        foreach (var citizen in citizenList)
        {
            if(IsSpawnRange(citizen.transform.position) && !citizen.gameObject.activeSelf)
            {
                citizen.gameObject.SetActive(true);
            }
            else if(!IsSpawnRange(citizen.transform.position) && citizen.gameObject.activeSelf)
            {
                citizen.gameObject.SetActive(false);
            }
        }
    }
    void Init()
    {
        //처음 사람 생성
        //SpawnCitizen(Random.Range(1, 10));
    }
    //플레이어의 일정 반경내에 생성
    void SpawnCitizen(int spawnNum)
    {
        /*GameObject citizen = Instantiate(citizenPrefab);

        print(player.transform.position);
        citizen.transform.position = new Vector3(player.transform.position.x + Random.Range(-10, 10), 
                                       player.transform.position.y, 
                                       player.transform.position.z + Random.Range(-10, 10));
        citizenList.Add(citizen);*/
    }
    void SpawnPolice(int spawnNum)
    {

    }
    void SpawnDoctor(int spawnNum)
    {

    }
    public bool IsSpawnRange(Vector3 position)
    {
        if (spawnRange > Vector3.Distance(position, player.transform.position))
        {
            return true;
        }
        else
        {
            return false;
            //gameObject.SetActive(true);
        }
    }

}
