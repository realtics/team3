using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunMachine : PlayerGun
{
    void Start()
    {
        gunType = GunState.Machinegun;
        bulletPoolCount = 50;

        InitGun();
        base.InitBullet("Machine");
    }
}
