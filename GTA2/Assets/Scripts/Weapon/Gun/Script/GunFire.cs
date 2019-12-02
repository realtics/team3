using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunFire : PlayerGun
{
    // Start is called before the first frame update.
    public override void Init()
    {
        base.Init();
        base.InitGun();
    }
}
