using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPunch : Gun
{
    void Start()
    {
        gunType = GunState.None;
        
        base.InitGun();
        this.InitBullet("Punch");
    }
    protected override void InitBullet(string poolName)
    {
        bulletList =
            GetPool<Bullet>.GetListComponent(
            SetPool.PoolMemory(
                bulletPref, this.gameObject, bulletPoolCount, "Bullet"));

        foreach (var item in bulletList)
        {
            item.gameObject.SetActive(true);
            item.gameObject.transform.position = new Vector3(10000.0f, 10000.0f, 10000.0f);
        }
    }

    protected override void Update()
    {
        if (player.isDie == true)
        {
            return;
        }
        base.UpdateDirection();
        UpdateDelta();
        UpdateKeyInput();
        this.UpdateShot();
    }

    protected override void UpdateShot()
    {
        if (isKeyShot || isButtonShot)
        {
            if (shootInterval < shootDelta)
            {
                BulletPunch bullet = (BulletPunch)ShootSingleBullet(userObject.transform.position);
                bullet.SetGun(userObject, this);
                shootDelta = .0f;
            }
        }

    }

    public Vector3 GetGunToTarget()
    {
        Vector3 tempGunDir = gunDir;
        tempGunDir.y = .0f;

        Vector3 returnVector = 
            userObject.transform.position +
            tempGunDir * 
            bulletToPeopleSize;
        return returnVector;
    }
}
