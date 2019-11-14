using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombFireBottle : Bullet
{
    // Start is called before the first frame update
    public Rigidbody MyRigidBody;
    public float YLaunchPos;
    public float YLaunchPower;

    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void SetBullet(GUNSTATE type, Vector3 pos, Vector3 dir, float bullettoSize)
    {
        base.SetBullet(type, pos, dir, bullettoSize);
        transform.position += Vector3.up * YLaunchPos;
    }

    public void SetForce(float forceValue)
    {
        Vector3 NewVec3 = bulletDir * bulletSpeed * forceValue;
        NewVec3.y = YLaunchPower;

        MyRigidBody.velocity = Vector3.zero;
        MyRigidBody.AddForce(NewVec3, ForceMode.Impulse);
    }



    protected void OnTriggerEnter(Collider other)
    {
        gameObject.SetActive(false);
    }
}
