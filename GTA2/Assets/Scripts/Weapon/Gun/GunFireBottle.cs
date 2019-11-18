using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunFireBottle : Gun
{
    // Start is called before the first frame update
    public GameObject smokePref;
    public List<FireBottleSmoke> smokeList;
    public int smokePoolCnt;
    public float moveThrowPower;

    private float intervalDelta;
    private int smokeIdx = 0;

    void Start()
    {
        gunType = GunState.FireBottle;
        bulletPoolCount = 50;

        InitGun();
        base.InitBullet("FireBottle");
        SetSmoke();

        player = userObject.GetComponent<Player>();
        intervalDelta = .0f;
    }



    void SetSmoke()
    {
        GameObject EffectPool = new GameObject();
        EffectPool.name = "SmokePool";

        smokeList =
            GetPool<FireBottleSmoke>.GetListComponent(
            SetPool.PoolMemory(smokePref, EffectPool, smokePoolCnt, "Smoke"));

        foreach (var item in smokeList)
        {
            item.gameObject.SetActive(false);
        }
    }

    protected override void UpdateShot()
    {
        if (isShot)
        {
            intervalDelta += Time.deltaTime;
        }

        else if ((!isShot && isPrevShot))
        {
            isPrevShot = false;

            if (shootInterval < intervalDelta)
            {
                intervalDelta = shootInterval;
            }
            if (player.isWalk)
            {
                intervalDelta += moveThrowPower;
            }

            smokeList[smokeIdx].SetTargetbullet(bulletList[bulletPoolIndex].gameObject);
            smokeIdx = GetPool<FireBottleSmoke>.PlusListIdx(smokeList, smokeIdx);
            
            BombFireBottle LaunchBullet = (BombFireBottle)ShootSingleBullet(userObject.transform.position);
            LaunchBullet.SetForce(intervalDelta);
            intervalDelta = .0f;
        }
    }
}
