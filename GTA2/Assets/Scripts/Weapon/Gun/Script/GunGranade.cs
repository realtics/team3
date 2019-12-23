using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunGranade : PlayerGun
{
    [Header("Throw Power")]
    public float moveThrowPower;


    float intervalDelta;

    public override void Init()
    {
        base.Init();
        base.InitGun();
        player = userObject.GetComponent<Player>();
        intervalDelta = .0f;
        isPrevShot = true;
    }


    protected override void UpdateShot()
    {
        if (GameManager.Instance.playerCar != null)
        {
            return;
        }

        if (isKeyShot || isButtonShot)
        {
            intervalDelta += Time.deltaTime;
            isPrevShot = false;
        }

        else if (!isKeyShot && !isPrevShot) 
        {
            isPrevShot = true;

            if (shootInterval < intervalDelta)
            {
                intervalDelta = shootInterval;
            }

            if ((
                Mathf.Abs(UIManager.Instance.playerMoveJoystick.Horizontal) > .01f &&
                Mathf.Abs(UIManager.Instance.playerMoveJoystick.Vertical) > .01f) ||
                (
                Input.GetAxisRaw("Vertical") != .0f ||
                Input.GetAxisRaw("Horizontal") != .0f))
            {
                intervalDelta += moveThrowPower;
            }


            BombGranade LaunchBullet = (BombGranade)ShootSingleBullet(userObject.transform.position);
            MinusPlayerBulletCount();
            LaunchBullet.SetForce(intervalDelta);
            intervalDelta = .0f;
        }
    }
}
