﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShot : Gun
{
    void Start()
    {
        gunType = GunState.ShotGun;
        bulletPoolCount = 10;

        base.InitGun();
        base.InitBullet("Shut");
    }
    protected override void InitBullet(string name)
    {
        bulletPool = new GameObject();
        bulletPool.name = name + "Pool";

        bulletList =
            GetPool<Bullet>.GetListComponent(
            SetPool.PoolMemory(
                bulletPref, bulletPool, bulletPoolCount, "Bullet"));

        foreach (var item in bulletList)
        {
            item.gameObject.SetActive(false);
        }
    }
    protected override void UpdateShot()
    {
        if (isShot)
        {
            if (shootInterval < shootDelta)
            {
                ShootAngleBullet(-45.0f, 45.0f, 7);
                shootDelta = .0f;
            }
        }
    }
}
