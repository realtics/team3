using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoSingleton<WeaponManager>
{
    [Header("Bullet Pool")]
    [SerializeField]
    GameObject machineBulletObject;
    [SerializeField]
    int machineBulletCount;

    [SerializeField]
    GameObject pistolBulletObject;
    [SerializeField]
    int pistolBulletCount;

    [SerializeField]
    GameObject fireBottleBulletObject;
    [SerializeField]
    int fireBottleBulletCount;

    [SerializeField]
    GameObject granadeBulletObject;
    [SerializeField]
    int granadeBulletCount;

    [SerializeField]
    GameObject electricBulletObject;
    [SerializeField]
    int electricBulletCount;

    [SerializeField]
    GameObject punchBulletObject;
    [SerializeField]
    int punchBulletCount;

    [SerializeField]
    GameObject rocketBulletObject;
    [SerializeField]
    int rocketBulletCount;

    [SerializeField]
    GameObject shotBulletObject;
    [SerializeField]
    int shotBulletCount;

    [SerializeField]
    GameObject npcPistolBulletObject;
    [SerializeField]
    int npcPistolBulletCount;

    [SerializeField]
    GameObject npcPunchBulletObject;
    [SerializeField]
    int npcPunchBulletCount;


    [Header("Effect Pool")]
    [SerializeField]
    GameObject rocketSmokeObject;
    [SerializeField]
    int rocketSmokeCount;

    [SerializeField]
    GameObject fireBottleSmokeObject;
    [SerializeField]
    int fireBottleSmokeCount;

    [SerializeField]
    GameObject bombExplosionObject;
    [SerializeField]
    int bombExplosionCount;

    [SerializeField]
    GameObject fireExplosionObject;
    [SerializeField]
    int fireExplosionCount;

    // Start is called before the first frame update
    void Awake()
    {
        PoolManager.WarmPool(machineBulletObject, machineBulletCount);
        PoolManager.WarmPool(pistolBulletObject, pistolBulletCount);
        PoolManager.WarmPool(fireBottleBulletObject, fireBottleBulletCount);
        PoolManager.WarmPool(granadeBulletObject, granadeBulletCount);
        PoolManager.WarmPool(electricBulletObject, electricBulletCount);
        PoolManager.WarmPool(punchBulletObject, punchBulletCount);
        PoolManager.WarmPool(rocketBulletObject, rocketBulletCount);
        PoolManager.WarmPool(shotBulletObject, shotBulletCount);
        PoolManager.WarmPool(npcPistolBulletObject, npcPistolBulletCount);
        PoolManager.WarmPool(npcPunchBulletObject, npcPunchBulletCount);


        PoolManager.WarmPool(rocketSmokeObject, rocketSmokeCount);
        PoolManager.WarmPool(fireBottleSmokeObject, fireBottleSmokeCount);
        PoolManager.WarmPool(bombExplosionObject, bombExplosionCount);
        PoolManager.WarmPool(fireExplosionObject, fireExplosionCount);
    }
}
