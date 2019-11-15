using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPunch : Gun
{
    void Start()
    {
        gunType = GunState.None;
        bulletPoolCount = 2;

        base.InitGun();
        base.InitBullet("Punch");
    }
}
