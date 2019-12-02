using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CarDB", menuName = "DB/Car")]
public class CarDatabase : ScriptableObject
{
	[Header("asdf")]
	public int carDamageScore;
	public int carDestroyScore;
	public int peopleKillScore;
}
