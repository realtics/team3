using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRocket : PlayerGun
{
    // Start is called before the first frame update
    [Header("Smoke Prefab")]
    public GameObject smokePref;

    public override void Init()
    {
        base.Init();
        base.InitGun();
    }




    protected override void UpdateShot()
    {
        if (isKeyShot || isButtonShot)
        {
            if (shootInterval < shootDelta)
            {
                Bullet shotBullet = ShootSingleBullet(userObject.transform.position);

                RocketSmoke shotEffect =
                    PoolManager.SpawnObject(smokePref).GetComponent<RocketSmoke>();

                shotEffect.SetTargetbullet(shotBullet.gameObject);
                MinusPlayerBulletCount();
                shootDelta = .0f;
            }
        }
    }
}
