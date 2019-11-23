using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShot : Gun
{
    void Start()
    {
        gunType = GunState.ShotGun;

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
        if (isKeyShot)
        {
            if (shootInterval < shootDelta)
            {
                ShootAngleBullet(-45.0f, 45.0f, 20);
                shootDelta = .0f;
            }
        }
    }
}
