using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NBulletPunch : Bullet
{
    // Start is called before the first frame update
    GameObject userObject;
    NpcGunPunch launchGun;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        UpdatePunchPos();
    }

    public void SetGun(GameObject userObject, NpcGunPunch launchGun)
    {
        this.userObject = userObject;
        this.launchGun = launchGun;
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
