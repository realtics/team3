using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunFireBottle : PlayerGun
{
    // Start is called before the first frame update
    [Header("Smoke Prefab")]
    public GameObject smokePref;

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

        else if ((!isKeyShot && !isButtonShot && !isPrevShot))
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


            BombFireBottle LaunchBullet = (BombFireBottle)ShootSingleBullet(userObject.transform.position);
            LaunchBullet.SetForce(intervalDelta);
            intervalDelta = .0f;

            FireBottleSmoke shotEffect =
                PoolManager.SpawnObject(smokePref).GetComponent<FireBottleSmoke>();

            shotEffect.SetTargetbullet(LaunchBullet.gameObject);
            shootDelta = .0f;

            MinusPlayerBulletCount();
        }
    }
}
