using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFireGun : Bullet
{
    // Update is called once per frame
    protected override void Awake()
    {
        base.Awake();
    }



    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Explosion();
        }
    }
}
