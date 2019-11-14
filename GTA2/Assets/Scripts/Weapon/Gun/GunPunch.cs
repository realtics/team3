using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPunch : Gun
{
    void Start()
    {
        gunType = GUNSTATE.NONE;
        bulletPoolCount = 2;

        base.InitGun();
        base.InitBullet("Punch");
    }
}
