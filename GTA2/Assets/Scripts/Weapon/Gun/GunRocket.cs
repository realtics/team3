using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRocket : Gun
{
    // Start is called before the first frame update
    public GameObject smokePref;
    public List<RocketSmoke> smokeList;
    public int smokePoolCnt;

    int SmokeIdx = 0;
    void Start()
    {
        gunType = GunState.RocketLauncher;
        bulletPoolCount = 10;

        base.InitGun();
        base.InitBullet("Rocket");

        SetSmoke();
    }



    void SetSmoke()
    {
        GameObject EffectPool = new GameObject();
        EffectPool.name = "SmokePool";

        smokeList =
            GetPool<RocketSmoke>.GetListComponent(
            SetPool.PoolMemory(smokePref, EffectPool, smokePoolCnt, "Smoke"));

        foreach (var item in smokeList)
        {
            item.gameObject.SetActive(false);
        }
    }

    protected override void UpdateShot()
    {
        if (isKeyShot || isButtonShot)
        {
            if (shootInterval < shootDelta)
            {
                smokeList[SmokeIdx].SetTargetbullet(bulletList[bulletPoolIndex].gameObject);
                ShootSingleBullet(userObject.transform.position);
                shootDelta = .0f;
                SmokeIdx = GetPool<RocketSmoke>.PlusListIdx(smokeList, SmokeIdx);
            }
        }
    }
}
