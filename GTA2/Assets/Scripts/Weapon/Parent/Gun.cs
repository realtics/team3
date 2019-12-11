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
    public GunInformation gunInfo;
    public AudioClip gunSound;
    
    [HideInInspector]
    public int bulletCount;

    // 사용하는 사람
    [HideInInspector]
    public GameObject userObject;


    protected GameObject bulletPref;
    protected float shootInterval;
    protected float bulletToPeopleSize;
    
    protected int shotPerOneBullet;

    protected int shotPerCurBullet;
    protected GunState gunType;

    protected Vector3 gunPos;
    protected Vector3 gunDir;

    protected float shootDelta;

    protected GameObject bulletPool;

    protected bool isShot;
    protected bool isPrevShot;
    protected string shotSFXName;


    public virtual void Init()
    {
        gunType = gunInfo.gunState;
        bulletPref = gunInfo.bulletPref;
        shootInterval = gunInfo.shootInterval;
        bulletToPeopleSize = gunInfo.bulletToPeopleSize;
        shotPerOneBullet = gunInfo.shotPerOneBullet;
        shotSFXName = gunInfo.shotSFX;
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

    protected Bullet Shoot(Vector3 triggerPos)
    {
        Bullet returnBullet = 
            PoolManager.SpawnObject(bulletPref).GetComponent<Bullet>();
        
        returnBullet.SetBullet(gunType, triggerPos, gunDir, bulletToPeopleSize);

        return returnBullet;
    }

    protected Bullet ShootSingleBullet(Vector3 triggerPos)
    {
        triggerPos.y += .25f;

        SFXPlay();
        return Shoot(triggerPos);
    }
    protected virtual void ShootAngleBullet(float startAngle, float endAngle, int bulletCnt)
    {
        List<Vector3> angleVectorList =
            GameMath.DivideAngleFromCount(startAngle, endAngle, gunDir.y, bulletCnt);

        foreach (var bulletDir in angleVectorList)
        {
            Bullet returnBullet =
               PoolManager.SpawnObject(bulletPref).GetComponent<Bullet>();

            returnBullet.SetBullet(gunType, userObject.transform.position, bulletDir, bulletToPeopleSize);
        }

        SFXPlay();
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
            if (shootInterval < shootDelta && bulletCount > 0)
            {
                ShootSingleBullet(userObject.transform.position);
                shootDelta = .0f;
            }
        }
        
    }
    protected virtual void SFXPlay()
    {
        SoundManager.Instance.PlayClip(gunSound, true);
    }
}
