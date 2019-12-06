using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "DB/Gun")]
public class GunInformation : ScriptableObject
{
    public GunState gunState;

    public GameObject bulletPref;
    public float shootInterval;
    public float bulletToPeopleSize;
    public int shotPerOneBullet;
    public string shotSFX;
}
