using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : Gun
{
    // Start is called before the first frame update
    protected Player player;

    protected bool isKeyShot = false;
    protected bool isButtonShot = false;


    // Start is called before the first frame update
    protected override void InitGun()
    {
        InitPlayer();
        base.InitGun();
    }

    public virtual void ResetBulletCount()
    {
        bulletCount = 0;
    }

    protected void InitPlayer()
    {
        userObject = GameObject.FindWithTag("Player");
        player = userObject.GetComponent<Player>();
    }

    protected override void FixedUpdate()
    {
        if (player == null)
        {
            InitPlayer();
        }

        if (player.isDie == true)
        {
            return;
        }
        if (bulletCount == 0)
        {
            return;
        }

        UpdateBulletCount();
        UpdateDirection();
        UpdateDelta();
        UpdateKeyInput();
        UpdateShot();
    }



    protected override void ShootAngleBullet(float startAngle, float endAngle, int bulletCnt)
    {
        base.ShootAngleBullet(startAngle, endAngle, bulletCnt);
        MinusPlayerBulletCount();
    }



    void UpdateBulletCount()
    {
        if (bulletCount <= 0)
        {
            gameObject.SetActive(false);
            player.SwapNext();
        }
    }
    protected void UpdateKeyInput()
    {
        if (Input.GetKey(KeyCode.A))
        {
            isKeyShot = true;
        }
        else
        {
            isKeyShot = false;
        }
    }

    protected override void UpdateShot()
    {
        if (GameManager.Instance.playerCar != null)
        {
            return;
        }

        if (isKeyShot || isButtonShot)
        {
            if (shootInterval < shootDelta && bulletCount > 0)
            {
                ShootSingleBullet(userObject.transform.position);
                MinusPlayerBulletCount();
                shootDelta = .0f;
            }
        }
    }


    protected void MinusPlayerBulletCount()
    {
        if (shotPerCurBullet + 1 < shotPerOneBullet)
        {
            shotPerCurBullet++;
        }
        else if (shotPerCurBullet + 1 == shotPerOneBullet)
        {
            bulletCount--;
            if (bulletCount <= 0)
            {
                player.SwapNext();
            }

            shotPerCurBullet = 0;
        }
    }




    public void UpdateButtonDown()
    {
        isButtonShot = true;
    }

    public void UpdateButtonUp()
    {
        isButtonShot = false;
    }
}
