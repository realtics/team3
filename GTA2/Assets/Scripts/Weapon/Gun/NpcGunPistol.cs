using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcGunPistol : NPCGun
{
    void Start()
    {
        gunType = GUNSTATE.PISTOL;
        bulletPoolCnt = 30;

        base.InitGun();
        base.InitBullet("Pistol");
    }
}
