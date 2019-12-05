using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WantedLevelData", menuName = "DB/WantedLevel")]
public class WantedLevelData : ScriptableObject
{
	public float[] agroSteps;

	public float gunFire;
	public float hitPeople;
	public float killPeople;
	public float killCop;
	public float stealCar;
	public float hitCar;
	public float destroyCar;
}
