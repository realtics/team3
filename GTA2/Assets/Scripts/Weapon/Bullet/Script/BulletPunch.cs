using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPunch : Bullet
{
    // Start is called before the first frame update
    GameObject userObject;
    GunPunch launchGun;

    protected override void Awake()
    {
        base.Awake();
    }

    public void SetGun(GameObject userObject, GunPunch launchGun)
    {
        this.userObject = userObject;
        this.launchGun = launchGun;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        UpdatePunchPos();
    }

    void UpdatePunchPos()
    {
        if (userObject == null)
        {
            return;
        }

        transform.position = launchGun.GetGunToTarget();
    }
}
