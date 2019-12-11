using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCSpawnData", menuName = "DB/NPCSpawnData")]
public class NPCSpawnData : ScriptableObject
{
	public float spawnInterval = 0.5f;
	public int spawnCitizenNumInInterval = 4;
	public int spawnPoliceNumInInterval = 3;
	public int maximumNPCNum = 10;
}
