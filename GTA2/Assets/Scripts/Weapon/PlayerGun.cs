using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : Gun
{
    // Start is called before the first frame update


    protected bool isKeyShot = false;
    protected bool isButtonShot = false;


    // Start is called before the first frame update
    protected override void InitGun()
    {
        userObject = GameObject.FindWithTag("Player");
        player = userObject.GetComponent<Player>();

        base.InitGun();
    }

    public void ResetBulletCount()
    {
        if (gunType == GunState.None)
        {
            bulletCount = 1;
        }
        else
        {
            bulletCount = 0;
        }
    }

    protected override void Update()
    {
        if (player.isDie == true)
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
            player.gunList[(int)gunType].bulletCount--;
            if (player.gunList[(int)gunType].bulletCount <= 0)
            {
                player.SwapNext();
            }

            shotPerCurBullet = 0;
        }
    }




    public void UpdateBottonDown()
    {
        isButtonShot = true;
    }

    public void UpdateBottonUp()
    {
        isButtonShot = false;
    }
}
