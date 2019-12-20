using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PoliceData", menuName = "DB/Police")]
public class PoliceData : ScriptableObject
{
	public int maxHp = 150;
	public float moveSpeed = 1.0f;
	public float runawaySpeed = 2.0f;
	public float jumpTime = 0.5f;
	public float downTime = 3.0f;

	public float findRange = 10.0f;
	public float punchRange = 0.08f;
	public float shotRange = 10.0f;
	public float chaseRange = 7.0f;
	public float outofRange = 400.0f;
	public float minIdleTime = 0.3f;
	public float maxIdleTime = 1.0f;
	public float minWalkTime = 10.0f;
	public float maxWalkTime = 15.0f;
	public float carOpenTimer = 0.0f;
	public float carOpenTime = 0.5f;
	public int money = 100;
}
