using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunFireBottle : PlayerGun
{
    // Start is called before the first frame update
    public GameObject smokePref;
    public float moveThrowPower;



    float intervalDelta;
    int smokeIdx = 0;

    public override void Init()
    {
        base.Init();
        base.InitGun();
        SetSmoke();

        player = userObject.GetComponent<Player>();
        intervalDelta = .0f;
        isPrevShot = true;
    }



    void SetSmoke()
    {
        
    }

    protected override void UpdateShot()
    {
        if (isKeyShot || isButtonShot)
        {
            intervalDelta += Time.deltaTime;
            isPrevShot = false;
        }

        else if ((!isKeyShot && !isButtonShot && !isPrevShot))
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


            BombFireBottle LaunchBullet = (BombFireBottle)ShootSingleBullet(userObject.transform.position);
            LaunchBullet.SetForce(intervalDelta);
            intervalDelta = .0f;

            FireBottleSmoke shotEffect =
                PoolManager.SpawnObject(smokePref).GetComponent<FireBottleSmoke>();

            shotEffect.SetTargetbullet(LaunchBullet.gameObject);
            shootDelta = .0f;

            MinusPlayerBulletCount();
        }
    }
}
