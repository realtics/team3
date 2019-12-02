using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WantedLevel : MonoSingleton<WantedLevel>
{
	public int[] agroSteps;
	public float agro;
	public int level;

	public enum CrimeType
	{
		gunFire, hitPeople, killPeople, killCop, hitCar, destroyCar
	}

	public void CommitCrime(CrimeType type, Vector3 position)
	{
		// 경찰차는 car, 경찰관은 npc

		//if (Physics.OverlapSphere())
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

		float agroAmount = 0;
		switch (type)
		{
			case CrimeType.gunFire:
				agroAmount = 0.1f;
				break;
			case CrimeType.hitPeople:
				agroAmount = 0.25f;
				break;
			case CrimeType.killPeople:
				agroAmount = 1.0f;
				break;
			case CrimeType.killCop:
				agroAmount = 2.0f;
				break;
			case CrimeType.hitCar:
				agroAmount = 0.2f;
				break;
			case CrimeType.destroyCar:
				agroAmount = 0.1f;
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
		agro += amount;
		CalcWantedLevel();
	}

	void CalcWantedLevel()
	{
		for (int i = 0; i < agroSteps.Length; i++)
		{
			if(agro < agroSteps[i])
			{
				level = i;
				break;
			}
		}
	}
}
