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
    public GameObject bulletPref;
    public float shootInterval;
    public float bulletToPeopleSize;

    public int bulletPoolCount;
    public int bulletCount;
    public int shotPerOneBullet;

    protected int bulletPoolIndex;
    protected int shotPerCurBullet;

    protected GameObject userObject;
    protected Player userPlayer;
    protected GunState gunType;

    protected Vector3 gunPos;
    protected Vector3 gunDir;

    protected List<Bullet> bulletList;
    protected float shootDelta;

    protected GameObject bulletPool;
    protected AudioSource gunSoundEffect;
    protected bool isShot;
    protected bool isPrevShot;



    // Start is called before the first frame update
    protected void InitGun()
    {
        userObject = GameObject.FindWithTag("Player");
        userPlayer = userObject.GetComponent<Player>();

        transform.eulerAngles = new Vector3(90.0f, 0.0f, 90.0f);
        transform.parent = userObject.transform;

        // 인터벌은 수정가능
        shootDelta = .0f;
        shotPerCurBullet = 0;
    }

    protected virtual void InitBullet(string poolName)
    {
        bulletPool = new GameObject();
        bulletPool.name = poolName + "Pool";

        bulletList =
            GetPool<Bullet>.GetListComponent(
            SetPool.PoolMemory(
                bulletPref, bulletPool, bulletPoolCount, "Bullet"));

        foreach (var item in bulletList)
        {
            item.gameObject.SetActive(true);
            item.gameObject.transform.position = new Vector3(10000.0f, 10000.0f, 10000.0f);
        }
    }



    protected Bullet ShootSingleBullet(Vector3 triggerPos)
    {
        bulletList[bulletPoolIndex].SetBullet(gunType, triggerPos, gunDir, bulletToPeopleSize);
        Bullet returnBullet = bulletList[bulletPoolIndex];

        PlusBulletIdx();
        MinusPlayerBulletCount();
        return returnBullet;
    }
    protected void ShootAngleBullet(float startAngle, float endAngle, int bulletCnt)
    {
        List<Vector3> actVectorList =
            GameMath.DivideAngleFromCount(startAngle, endAngle, gunDir.y, bulletCnt);

        foreach (var item in actVectorList)
        {
            bulletList[bulletPoolIndex].gameObject.SetActive(true);
            bulletList[bulletPoolIndex].SetBullet(
                gunType, userObject.transform.position, item, bulletToPeopleSize);
            PlusBulletIdx();
            MinusPlayerBulletCount();
        }
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
        UpdateKeyInput();
    }


    private void UpdateDelta()
    {
        shootDelta += Time.deltaTime;        
    }
    protected virtual void UpdateDirection()
    {
        gunDir = userObject.transform.forward;
        gunDir.y = userObject.transform.eulerAngles.y;
    }



    // 요부분은 사람이 해도 되는 거지만 일단 여기서 구현 - 총알 발사
    protected virtual void UpdateKeyInput()
    {
        if (Input.GetKey(KeyCode.A) || isShot)
        {
            if (shootInterval < shootDelta)
            {
                ShootSingleBullet(userObject.transform.position);
                shootDelta = .0f;
            }
        }
        
    }

    public void UpdateBottonDown()
    {
        isPrevShot = false;
        isShot = true;
    }

    public void UpdateBottonUp()
    {
        isPrevShot = true;
        isShot = false;
    }

    void PlusBulletIdx()
    {
        bulletPoolIndex = GetPool<Bullet>.PlusListIdx(bulletList, bulletPoolIndex);
    }

    void MinusPlayerBulletCount()
    {
        if (shotPerCurBullet + 1 < shotPerOneBullet)
        {
            shotPerCurBullet++;
        }
        else if (shotPerCurBullet + 1 == shotPerOneBullet)
        {
            userPlayer.gunList[(int)gunType].bulletCount--;
            if (userPlayer.gunList[(int)gunType].bulletCount <= 0)
            {
                userPlayer.SwapNext();
            }

            shotPerCurBullet = 0;
        }
    }
}
