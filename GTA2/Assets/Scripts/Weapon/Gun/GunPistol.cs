using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPistol : PlayerGun
{
    public override void Init()
    {
        gunType = GunState.Pistol;
        bulletPoolCount = 30;

        base.InitGun();
        base.InitBullet("Pistol");
    }
}
