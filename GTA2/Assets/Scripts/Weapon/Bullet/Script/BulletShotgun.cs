using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShotgun : Bullet
{
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        bulletDeActiveTime = bulletLifeTime;
    }



    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
