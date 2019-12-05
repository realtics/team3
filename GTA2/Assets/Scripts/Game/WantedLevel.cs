using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WantedLevel : MonoBehaviour
{
	public static WantedLevel instance;
	public WantedLevelData data;

	public float[] agroSteps;
	public float agro;
	public int level;

	void Awake()
	{
		if(instance == null)
		{
			instance = this;
			agroSteps = data.agroSteps;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public enum CrimeType
	{
		gunFire, hitPeople, killPeople, killCop, stealCar, hitCar, destroyCar
	}

	public void CommitCrime(CrimeType type, Vector3 position)
	{
		bool isPoliceExist = false;

		foreach (var car in CarSpawnManager.Instance.allPoliceCar)
		{
			if (!car.enabled)
				continue;

			if((car.transform.position - GameManager.Instance.player.transform.position).magnitude < 7)
			{
				isPoliceExist = true;
				break;
			}
		}

		// todo: 주변에 경찰관이 있는지 확인하는 내용이 여기에 있어야함.

		float agroAmount = 0;
		switch (type)
		{
			case CrimeType.gunFire:
				agroAmount = data.gunFire;
				break;
			case CrimeType.hitPeople:
				agroAmount = data.hitPeople;
				break;
			case CrimeType.killPeople:
				agroAmount = data.killPeople;
				break;
			case CrimeType.killCop:
				agroAmount = data.killCop;
				break;
			case CrimeType.stealCar:
				agroAmount = data.stealCar;
				break;
			case CrimeType.hitCar:
				agroAmount = data.hitCar;
				break;
			case CrimeType.destroyCar:
				agroAmount = data.destroyCar;
				break;
		}

		if(isPoliceExist && agroAmount < 1 && level < 1)
		{
			agroAmount = 1;
		}

		IncreaseAgro(agroAmount);
	}

	void IncreaseAgro(float amount)
	{
		if (agro + amount >= agroSteps[agroSteps.Length-1])
			return;

		agro += amount;

		if (agro >= agroSteps[level])
		{
			IncreaseWantedLevel();
		}
	}

	public void IncreaseWantedLevel()
	{
		level++;
		UIManager.Instance.SetPoliceLevel(level);
	}

	public void ResetWantedLevel()
	{
		level = 0;
		UIManager.Instance.SetPoliceLevel(0);
		CarSpawnManager.Instance.StopAllPoliceCarChasing();
	}
}
