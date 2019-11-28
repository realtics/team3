using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunGranade : PlayerGun
{

    public float moveThrowPower;


    float intervalDelta;

    public override void Init()
    {
        gunType = GunState.Granade;
        bulletPoolCount = 50;

        InitGun();
        base.InitBullet("Granade");


        player = userObject.GetComponent<Player>();
        intervalDelta = .0f;
        isPrevShot = true;
    }


    protected override void UpdateShot()
    {
        if (isKeyShot || isButtonShot)
        {
            intervalDelta += Time.deltaTime;
            isPrevShot = false;
        }

        else if (!isKeyShot && !isPrevShot) 
        {
            isPrevShot = true;

            if (shootInterval < intervalDelta)
            {
                intervalDelta = shootInterval;
            }

            if (player.isWalk)
            {
                intervalDelta += moveThrowPower;
            }

            BombGranade LaunchBullet = (BombGranade)ShootSingleBullet(userObject.transform.position);
            MinusPlayerBulletCount();
            LaunchBullet.SetForce(intervalDelta);
            intervalDelta = .0f;
        }
    }
}
