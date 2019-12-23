using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcGunPunch : NPCGun
{
    void Start()
    {
        base.Init();
        base.InitGun();
    }

    protected override void FixedUpdate()
    {
        base.UpdateDirection();
        UpdateDelta();
        this.UpdateShot();
    }

    protected override void UpdateShot()
    {
        if (GameManager.Instance.playerCar != null)
        {
            return;
        }

        if (isShot)
        {
            if (shootInterval < shootDelta)
            {
                NBulletPunch bullet = (NBulletPunch)ShootSingleBullet(userObject.transform.position);
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
