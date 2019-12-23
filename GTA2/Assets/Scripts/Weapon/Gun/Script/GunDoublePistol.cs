using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunDoublePistol : PlayerGun
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

        shootDelta += Time.deltaTime;
        if (isKeyShot || isButtonShot)
        {
            if (shootInterval < shootDelta)
            {
                ShootAngleBullet(-15.0f, 15.0f, 2);
                shootDelta = .0f;
            }
        }
    }

}
