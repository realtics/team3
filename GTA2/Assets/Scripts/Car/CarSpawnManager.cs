using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawnManager : MonoSingleton<CarSpawnManager>
{
	public GameObject policeCarPrefab;
	public GameObject policeVanPrefab;
	public GameObject AmbulancePrefab;
	public GameObject Car51Prefab;
	public GameObject IceCarPrefab;
	public GameObject TaxiPrefab;
	public GameObject TruckPrefab;

	public List<CarManager> allCars = new List<CarManager>();
	public List<CarManager> allPoliceCar = new List<CarManager>();

	void Awake()
	{
		PoolManager.WarmPool(policeCarPrefab, 12);
		PoolManager.WarmPool(policeVanPrefab, 6);
		//PoolManager.WarmPool(AmbulancePrefab, 2);
		PoolManager.WarmPool(Car51Prefab, 12);
		PoolManager.WarmPool(IceCarPrefab, 5);
		PoolManager.WarmPool(TaxiPrefab, 12);
		PoolManager.WarmPool(TruckPrefab, 5);

		allCars.AddRange(PoolManager.GetAllObject<CarManager>(policeCarPrefab));
		allCars.AddRange(PoolManager.GetAllObject<CarManager>(policeVanPrefab));
		//allCars.AddRange(PoolManager.GetAllObject<CarManager>(AmbulancePrefab));
		allCars.AddRange(PoolManager.GetAllObject<CarManager>(Car51Prefab));
		allCars.AddRange(PoolManager.GetAllObject<CarManager>(IceCarPrefab));
		allCars.AddRange(PoolManager.GetAllObject<CarManager>(TaxiPrefab));
		allCars.AddRange(PoolManager.GetAllObject<CarManager>(TruckPrefab));

		foreach (var obj in allCars)
		{
			if (obj.ai.isPolice)
				allPoliceCar.Add(obj);

			obj.gameObject.SetActive(false);
		}

		for (int i = 0; i < allCars.Count; i++)
		{
			CarManager temp = allCars[i];
			int randIdx = Random.Range(0, allCars.Count);
			allCars[i] = allCars[randIdx];
			allCars[randIdx] = temp;
		}
	}

	void Start()
    {
		InvokeRepeating("RespawnDisabledCar", 0.2f, 0.2f);
	}

    void RespawnDisabledCar()
    {
		float policeCarCount = 0;
		foreach (var p in allPoliceCar)
		{
			if (p.gameObject.activeSelf)
			{
				if(p.carState == CarManager.CarState.controlledByAi)
				{
					policeCarCount += 1.0f;
				}
				else
				{
					policeCarCount += 0.33f;
				}				
			}				
		}

		foreach (var car in allCars)
        {
            if (car.gameObject.activeSelf)
                continue;

			if (policeCarCount >= WantedLevel.instance.level &&
				car.ai.isPolice)
				continue;

			GameObject go = WaypointManager.instance.FindRandomCarSpawnPosition();

            Ray ray = new Ray(go.transform.position + (Vector3.up * 5), Vector3.down);
            RaycastHit hit;
            if(Physics.SphereCast(ray, 2f, out hit, 10, 1<<12))
            {
                //Debug.DrawLine(GameManager.Instance.player.transform.position, go.transform.position, Color.red, 0.5f);
            }
            else
            {
                car.transform.position = go.transform.position;
                car.gameObject.SetActive(true);
                car.GetComponent<CarManager>().movement.curSpeed = 100;

                //Debug.DrawLine(GameManager.Instance.player.transform.position, car.transform.position, Color.green, 0.5f);
            }

            break;
        }
    }


    public CarManager SpawnPoliceCar(Vector3 position)
    {
		//GameObject policeCar = PoolManager.SpawnObject(policeCarPrefab, 
		//	position, 
		//	Quaternion.identity);

		foreach (var car in allPoliceCar)
		{
			if (car.gameObject.activeSelf)
				continue;

			car.gameObject.transform.position = position;
			car.gameObject.SetActive(true);

			return car;
		}

		return null;

		//CarManager cm = policeCar.GetComponent<CarManager>();
		//if (!allPoliceCar.Contains(cm))
		//{
		//	allCars.Add(cm);
		//	allPoliceCar.Add(cm);
		//}

		//return cm;
    }
	public CarManager SpawnAmbulanceCar(Vector3 position)
	{
		GameObject ambulanceCar = PoolManager.SpawnObject(AmbulancePrefab,
			position,
			Quaternion.identity);

		CarManager cm = ambulanceCar.GetComponent<CarManager>();

		if (!allPoliceCar.Contains(cm))
		{
			allCars.Add(cm);
		}

		return cm;
	}

	public CarManager FindClosestCar(Vector3 position)
	{
		float min = 1000.0f;
		CarManager closestCar = null;

		foreach (CarManager car in allCars)
		{
			if (!car.gameObject.activeSelf)
				continue;

			float dist = (car.transform.position - position).sqrMagnitude;

			if (dist < min)
			{
				if (car.carState == CarManager.CarState.destroied)
					continue;

				min = dist;
				closestCar = car;
			}
		}

		return closestCar;
	}

	public void StopAllPoliceCarChasing()
	{
		foreach (var car in allPoliceCar)
		{
			car.ai.StopChase();
			car.GetComponent<CarPathManager>().SelectNewDestinationImmediate();
		}
	}
}
