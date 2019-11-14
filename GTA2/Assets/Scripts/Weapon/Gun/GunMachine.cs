using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunMachine : Gun
{
    void Start()
    {
        gunType = GUNSTATE.MACHINGUN;
        bulletPoolCount = 50;

        InitGun();
        base.InitBullet("Machine");
    }
}
