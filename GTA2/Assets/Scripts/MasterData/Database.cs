using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "database", menuName = "DB")]
public class Database : ScriptableObject
{
    [Header("플레이어")]
    public int playerMaxHp;
    public float playerMoveSpeed;

    [Header("점수")]
    public int carDamageScore;
    public int carDestroyScore;
    public int peopleKillScore;

    [Header("총")]
    public float pistolDamage;
}
