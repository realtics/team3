using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class NPCGun : MonoBehaviour
{
    // 사용하는 사람
    public GameObject userObject;
    public GameObject bulletPref;

    public float shootInterval;
    public float bulletToPeopleSize;




    protected GUNSTATE gunType;
    protected int bulletPoolCnt;

    protected Vector3 gunPos;
    protected Vector3 gunDir;

    protected List<Bullet> bulletList;
    protected float shootDelta;
    protected int bulletIdx;

    protected GameObject bulletPool;



    protected virtual void InitBullet(string poolname)
    {
        bulletPool = new GameObject();
        bulletPool.name = poolname + "Pool";

        bulletList =
            GetPool<Bullet>.GetListComponent(
            SetPool.PoolMemory(
                bulletPref, bulletPool, bulletPoolCnt, "Bullet"));

        foreach (var item in bulletList)
        {
            item.gameObject.SetActive(false);
        }
    }


    // Start is called before the first frame update

    protected void InitGun()
    {
        transform.eulerAngles = new Vector3(90.0f, 0.0f, 90.0f);
        transform.parent = userObject.transform;

        // 인터벌은 수정가능
        shootDelta = .0f;
    }



    protected void ShootSingleBullet()
    {
        bulletList[bulletIdx].gameObject.SetActive(true);
        bulletList[bulletIdx].SetBullet(gunType, userObject.transform.position, gunDir, bulletToPeopleSize);

        PlusBulletidx();
    }
    protected void ShootAngleBullet(float startAngle, float endAngle, int BulletCnt)
    {
        if (startAngle > endAngle)
        {
            float tmp = startAngle;
            startAngle = endAngle;
            endAngle = tmp;
        }

        float spacing = (endAngle - startAngle) / (BulletCnt - 1);


        for (int i = 0; i < BulletCnt; i++)
        {
            float Value = (startAngle + spacing * i);
            float GunRad = .0f;
            GunRad = (gunDir.y + Value + 90.0f) * Mathf.Deg2Rad;

            Vector3 BulletDir = new Vector3(Mathf.Cos(GunRad), .0f, Mathf.Sin(GunRad));
            BulletDir.x *= -1.0f;
            BulletDir.y = gunDir.y + Value;

            bulletList[bulletIdx].gameObject.SetActive(true);
            bulletList[bulletIdx].SetBullet(gunType, userObject.transform.position, BulletDir, bulletToPeopleSize);

            PlusBulletidx();
        }
    }




    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateDirection();
        UpdateInput();
    }


    void UpdateDirection()
    {
        gunDir = userObject.transform.forward;
        gunDir.y = userObject.transform.eulerAngles.y;
    }

    // 요부분은 사람이 해도 되는 거지만 일단 여기서 구현 - 총알 발사
    protected virtual void UpdateInput()
    {
        if (true == Input.GetKey(KeyCode.A))
        {
            shootDelta += Time.deltaTime;
            if (shootInterval < shootDelta)
            {
                ShootSingleBullet();
                shootDelta = .0f;
            }
        }
    }
    void PlusBulletidx()
    {
        bulletIdx = GetPool<Bullet>.PlusListIdx(bulletList, bulletIdx);
    }
}
