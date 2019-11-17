using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunGranade : Gun
{

    public float moveThrowPower;


    private float intervalDelta;
    private Player userPlayer;

    void Start()
    {
        gunType = GunState.Granade;
        bulletPoolCount = 50;

        InitGun();
        base.InitBullet("Granade");


        userPlayer = userObject.GetComponent<Player>();
        intervalDelta = .0f;
    }


    protected override void UpdateShot()
    {
        if (Input.GetKey(KeyCode.A) || isShot)
        {
            intervalDelta += Time.deltaTime;
        }

        else if (Input.GetKeyUp(KeyCode.A) || (!isShot && isPrevShot))
        {
            isPrevShot = false;

            if (shootInterval < intervalDelta)
            {
                intervalDelta = shootInterval;
            }

            if (userPlayer.isWalk)
            {
                intervalDelta += moveThrowPower;
            }

            BombGranade LaunchBullet = (BombGranade)ShootSingleBullet(userObject.transform.position);
            LaunchBullet.SetForce(intervalDelta);
            intervalDelta = .0f;
        }
    }
}
