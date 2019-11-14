using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRocket : Gun
{
    // Start is called before the first frame update
    public GameObject SmokePref;
    public List<RocketSmoke> SmokeList;
    public int SmokePoolCnt;

    private int SmokeIdx = 0;
    void Start()
    {
        gunType = GUNSTATE.ROCKETLAUNCHER;
        bulletPoolCount = 10;

        base.InitGun();
        base.InitBullet("Rocket");

        SetSmoke();
    }



    void SetSmoke()
    {
        GameObject EffectPool = new GameObject();
        EffectPool.name = "SmokePool";

        SmokeList =
            GetPool<RocketSmoke>.GetListComponent(
            SetPool.PoolMemory(SmokePref, EffectPool, SmokePoolCnt, "Smoke"));

        foreach (var item in SmokeList)
        {
            item.gameObject.SetActive(false);
        }
    }

    protected override void UpdateInput()
    {
        shootDelta += Time.deltaTime;
        if (true == Input.GetKey(KeyCode.A))
        {
            if (shootInterval < shootDelta)
            {
                SmokeList[SmokeIdx].SetTargetbullet(bulletList[bulletPoolIndex].gameObject);
                ShootSingleBullet(userObject.transform.position);
                shootDelta = .0f;
                SmokeIdx = GetPool<RocketSmoke>.PlusListIdx(SmokeList, SmokeIdx);
            }
        }
    }
}
