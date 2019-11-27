using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunFire : PlayerGun
{
    // Start is called before the first frame update.
    public override void Init()
    {
        gunType = GunState.FireGun;
        bulletPoolCount = 50;


        InitGun();
        base.InitBullet("Fire");
    }
}
