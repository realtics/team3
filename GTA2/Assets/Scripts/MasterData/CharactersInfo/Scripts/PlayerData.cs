using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "DB/Player")]
public class PlayerData : ScriptableObject
{
	public int maxHp = 200;
	public float moveSpeed = 2.0f;
	public float jumpTime = 1.5f;
	public float downTime = 2.0f;
    public float runoverTime = 0.5f;
	public float respawnTime = 2.5f;
	public float autoLandTime = 0.5f;
}
