using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bullet", menuName = "DB/Bullet")]
public class BulletInformation : ScriptableObject
{
    public int bulletDamage;
    public float bulletLifeTime;
    public float bulletSpeed;
    public float explosionArea;
}
