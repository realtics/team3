using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunDoublePistol : Gun
{
    void Start()
    {
        gunType = GunState.DoublePistol;
        bulletPoolCount = 30;

        InitGun();
        base.InitBullet("DoublePistol");
    }
    protected override void UpdateKeyInput()
    {
        shootDelta += Time.deltaTime;
        if (true == Input.GetKey(KeyCode.A) || isShot)
        {
            if (shootInterval < shootDelta)
            {
                ShootAngleBullet(-15.0f, 15.0f, 2);
                shootDelta = .0f;
            }
        }
    }

}
