using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CarData", menuName = "DB/Car")]
public class CarData : ScriptableObject
{
	public int maxHp;

	public float maxSpeed;
	public float rotSpeed;
	public float acceleration;

	public int score;
}
