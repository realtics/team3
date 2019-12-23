using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShot : PlayerGun
{
    public override void Init()
    {
        base.Init();
        base.InitGun();
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
                ShootAngleBullet(-45.0f, 45.0f, 20);
                shootDelta = .0f;
            }
        }
    }
}
