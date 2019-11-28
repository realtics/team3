using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawnManager : MonoSingleton<CarSpawnManager>
{
    [Header("최초에는 ActiveList에 Pool의 오브젝트 삽입.")]
    public List<CarManager> normalCarList = new List<CarManager>();
    public List<CarManager> policeCarList = new List<CarManager>();
    // Start is called before the first frame update
    void Start()
    {
        CarPositionInit();
        InvokeRepeating("RespawnDisabledCar", 0.25f, 0.25f);
    }

    public void CarPositionInit() //중복 방지 생성
    {
        List<Vector3> position = new List<Vector3>();

        for (int i = 0; i < WaypointManager.instance.allWaypointsForCar.Length; i++)
        {
            position.Add(WaypointManager.instance.allWaypointsForCar[i].transform.position);
        }

        foreach (var car in normalCarList)
        {
            int randomIndex = Random.Range(0, position.Count);
            //print(randomIndex + " " + position.Count + " " + );
            car.gameObject.transform.position = position[randomIndex];
            car.gameObject.SetActive(false);
            car.gameObject.SetActive(true);
            position.RemoveAt(randomIndex);
        }
    }
    public void CarRepositioning(CarDamage car)
    {
        int randomIndex = Random.Range(0, WaypointManager.instance.allWaypointsForCar.Length);
        car.gameObject.transform.position = WaypointManager.instance.allWaypointsForCar[randomIndex].transform.position;
    }
    

    // 꺼진차 켜기
    void RespawnDisabledCar()
    {
        foreach (var car in normalCarList)
        {
            if (car.gameObject.activeSelf)
                continue;

            GameObject go = WaypointManager.instance.FindRandomCarSpawnPosition();

            Ray ray = new Ray(go.transform.position + (Vector3.up * 5), Vector3.down);
            RaycastHit hit;
            if(Physics.SphereCast(ray, 2f, out hit, 10, 1<<12))
            {
                Debug.DrawLine(GameManager.Instance.player.transform.position, go.transform.position, Color.red, 0.5f);
            }
            else
            {
                car.transform.position = go.transform.position;
                car.gameObject.SetActive(true);
                car.GetComponent<CarManager>().movement.curSpeed = 100;

                Debug.DrawLine(GameManager.Instance.player.transform.position, car.transform.position, Color.green, 0.5f);
            }

            break;
        }
    }
    public void SpawnPoliceCar()
    {
        foreach (var car in policeCarList)
        {
            if (car.gameObject.activeSelf)
                continue;

            car.gameObject.SetActive(true);
            return;
        }
    }
}
