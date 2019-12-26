using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPunch : PlayerGun
{
    public override void Init()
    {
        base.Init();
        base.InitGun();
    }
    public override void ResetBulletCount()
    {
        bulletCount = 1;
    }
    protected override void FixedUpdate()
    {
        if (player == null)
        {
            InitPlayer();
        }
        if (player.isDie == true)
        {
            return;
        }
        if (player.curGunIndex != gunType)
        {
            gameObject.SetActive(false);
            return;
        }
        base.UpdateDirection();
        UpdateDelta();
        UpdateCount();
        UpdateKeyInput();
        UpdateShot();
    }

    protected override void UpdateShot()
    {
        if (GameManager.Instance.playerCar != null)
        {
            return;
        }


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

    void UpdateCount()
    {
        if (bulletCount <= 0)
        {
            bulletCount = 1;
        }
    }
}
