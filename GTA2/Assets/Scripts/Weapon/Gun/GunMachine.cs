using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunMachine : Gun
{
    void Start()
    {
        gunType = GunState.Machinegun;
        bulletPoolCount = 50;

        InitGun();
        base.InitBullet("Machine");
    }
}
