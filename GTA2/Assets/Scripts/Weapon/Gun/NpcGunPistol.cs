using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcGunPistol : NPCGun
{
    void Start()
    {
        gunType = GunState.Pistol;
        bulletPoolCnt = 30;

        base.InitGun();
        base.InitBullet("Pistol");
    }
}
