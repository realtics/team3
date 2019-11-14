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
        gunType = GUNSTATE.GRANADE;
        bulletPoolCount = 50;

        InitGun();
        base.InitBullet("Granade");


        userPlayer = userObject.GetComponent<Player>();
        intervalDelta = .0f;
    }


    protected override void UpdateInput()
    {
        if (Input.GetKey(KeyCode.A))
        {
            intervalDelta += Time.deltaTime;
        }

        else if (Input.GetKeyUp(KeyCode.A))
        {
            if (shootInterval < intervalDelta)
            {
                intervalDelta = shootInterval;
            }

            if (userPlayer.playerStateUnder == Player.PLAYERSTATE_UNDER.WALK)
            {
                intervalDelta += moveThrowPower;
            }

            BombGranade LaunchBullet = (BombGranade)ShootSingleBullet(userObject.transform.position);
            LaunchBullet.SetForce(intervalDelta);
            intervalDelta = .0f;
        }
    }
}
