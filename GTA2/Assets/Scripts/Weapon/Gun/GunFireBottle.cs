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
    private Player userPlayer;
    private int SmokeIdx = 0;

    void Start()
    {
        gunType = GUNSTATE.FIREBOTTLE;
        bulletPoolCount = 50;

        InitGun();
        base.InitBullet("FireBottle");
        SetSmoke();

        userPlayer = userObject.GetComponent<Player>();
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

    protected override void UpdateInput()
    {
        if (Input.GetKey(KeyCode.A))
        {
            intervalDelta += Time.deltaTime;
        }

        else if (Input.GetKeyUp(KeyCode.A))
        {
            if (shootInterval < intervalDelta)
            {
                intervalDelta = shootInterval;
            }

            if (userPlayer.playerStateUnder == Player.PLAYERSTATE_UNDER.WALK)
            {
                intervalDelta += moveThrowPower;
            }


            smokeList[SmokeIdx].SetTargetbullet(bulletList[bulletPoolIndex].gameObject);
            SmokeIdx = GetPool<FireBottleSmoke>.PlusListIdx(smokeList, SmokeIdx);
            
            BombFireBottle LaunchBullet = (BombFireBottle)ShootSingleBullet(userObject.transform.position);
            LaunchBullet.SetForce(intervalDelta);
            intervalDelta = .0f;
        }
    }
}
