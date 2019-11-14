using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPistol : Gun
{
    void Start()
    {
        gunType = GUNSTATE.PISTOL;
        bulletPoolCount = 30;

        base.InitGun();
        base.InitBullet("Pistol");
    }
}
