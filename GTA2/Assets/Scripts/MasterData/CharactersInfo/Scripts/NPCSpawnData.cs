using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCSpawnData", menuName = "DB/NPCSpawnData")]
public class NPCSpawnData : ScriptableObject
{
	public float citizenSpawnInterval = 0.5f;
	public float policeSpawnInterval = 10.0f;
	public int maxSpawnCitizenNumInInterval = 4;
	public int maxSpawnPoliceNumInInterval = 3;
	public int maximumNPCNum = 30;
}
