﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunFire : Gun
{
    // Start is called before the first frame update.

    void Start()
    {
        gunType = GUNSTATE.FIREGUN;
        bulletPoolCount = 50;


        InitGun();
        base.InitBullet("Fire");
    }
}
