using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GunState
{
    None,
    Pistol,
    DoublePistol,
    Machinegun,
    SleepMachinegun,
    RocketLauncher,
    Electric,
    ShotGun,
    FireGun,
    FireBottle,
    Granade,
}

public abstract class Gun : MonoBehaviour
{
    // 사용하는 사람
    public GameObject userObject;
    public GameObject bulletPref;
    public float shootInterval;
    public float bulletToPeopleSize;

    public int bulletPoolCount;
    public int bulletCount;
    public int shotPerOneBullet;
    public AudioSource gunSoundSource;

    

    protected int bulletPoolIndex;
    protected int shotPerCurBullet;

    protected Player player;
    protected GunState gunType;

    protected Vector3 gunPos;
    protected Vector3 gunDir;

    protected List<Bullet> bulletList;
    protected float shootDelta;

    protected GameObject bulletPool;

    protected bool isShot;
    protected bool isPrevShot;


    public virtual void Init()
    {

    }



    protected virtual void InitGun()
    {
        shootDelta = .0f;
        shotPerCurBullet = 0;
        transform.eulerAngles = new Vector3(90.0f, 0.0f, 90.0f);

        if (userObject != null)
        {
            transform.parent = userObject.transform;
        }
    }

    protected virtual void InitBullet(string poolName)
    {
        bulletPool = new GameObject();
        bulletPool.name = poolName + "Pool";

        bulletList =
            GetPool<Bullet>.GetListComponent(
            SetPool.Instance.PoolMemory(
                bulletPref, bulletPool, bulletPoolCount, "Bullet"));

        foreach (var item in bulletList)
        {
            item.gameObject.transform.position = new Vector3(10000.0f, 10000.0f, 10000.0f);
            item.gameObject.SetActive(true);
        }
    }


    protected Bullet Shoot(Vector3 triggerPos)
    {
        bulletList[bulletPoolIndex].SetBullet(gunType, triggerPos, gunDir, bulletToPeopleSize);
        Bullet returnBullet = bulletList[bulletPoolIndex];

        PlusBulletIdx();
        return returnBullet;
    }

    protected Bullet ShootSingleBullet(Vector3 triggerPos)
    {
        SFXPlay();
        return Shoot(triggerPos);
    }
    protected virtual void ShootAngleBullet(float startAngle, float endAngle, int bulletCnt)
    {
        List<Vector3> actVectorList =
            GameMath.DivideAngleFromCount(startAngle, endAngle, gunDir.y, bulletCnt);

        foreach (var item in actVectorList)
        {
            bulletList[bulletPoolIndex].gameObject.SetActive(true);
            bulletList[bulletPoolIndex].SetBullet(
                gunType, userObject.transform.position, item, bulletToPeopleSize);
            PlusBulletIdx();
        }

        SFXPlay();
    }
    protected void DisableAllBullet()
    {
        foreach (var item in bulletList)
        {
            item.gameObject.SetActive(false);
            item.gameObject.transform.position = new Vector3(10000.0f, 10000.0f, 10000.0f);
        }
    }



    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateDirection();
        UpdateDelta();
        UpdateShot();
    }


    protected void UpdateDelta()
    {
        shootDelta += Time.deltaTime;
    }
    protected virtual void UpdateDirection()
    {
        if (userObject == null)
        {
            return;
        }

        gunDir = userObject.transform.forward;
        gunDir.y = userObject.transform.eulerAngles.y;
    }

    // 요부분은 사람이 해도 되는 거지만 일단 여기서 구현 - 총알 발사
    protected virtual void UpdateShot()
    {
        if (isShot)
        {
            if (shootInterval < shootDelta)
            {
                ShootSingleBullet(userObject.transform.position);
                shootDelta = .0f;
            }
        }
        
    }

    protected void PlusBulletIdx()
    {
        bulletPoolIndex = GetPool<Bullet>.PlusListIdx(bulletList, bulletPoolIndex);
    }

    protected void SFXPlay()
    {
        if (gunSoundSource != null)
        {
            gunSoundSource.PlayOneShot(gunSoundSource.clip);
        }
    }
}
