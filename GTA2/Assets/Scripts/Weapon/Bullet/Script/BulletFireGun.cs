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



    protected override void Update()
    {
        base.Update();
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Explosion();
        }
    }
}
