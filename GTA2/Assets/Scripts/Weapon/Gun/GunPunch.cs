using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPunch : Gun
{
    void Start()
    {
        gunType = GunState.None;
        bulletPoolCount = 2;

        base.InitGun();
        base.InitBullet("Punch");
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
        if (isShot)
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
