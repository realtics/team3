using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombGranade : Bullet
{
    // Start is called before the first frame update
    public Rigidbody myRigidBody;
    public float yLaunchPos;
    public float yLaunchPower;


    float explosionPower = 2.0f;

    protected override void Start()
    {
        base.Start();
        bulletDeActiveTime = .1f;
        myCollider.isTrigger = false;
    }


    // Update is called once per frame
    public override void SetBullet(GunState type, Vector3 pos, Vector3 dir, float bullettoSize)
    {
        myCollider.isTrigger = false;
        base.SetBullet(type, pos, dir, bullettoSize);
        transform.position += Vector3.up * yLaunchPos;
    }

    public void SetForce(float forceValue)
    {
        Vector3 NewVec3 = bulletDir * bulletSpeed * forceValue;
        NewVec3.y = yLaunchPower;

        myRigidBody.velocity = Vector3.zero;
        myRigidBody.AddForce(NewVec3, ForceMode.Impulse);
    }

    public override void Explosion()
    {
        myCollider.isTrigger = true;
        base.Explosion();
        CameraController.Instance.StartShake(explosionPower, transform.position);


    }

    protected override void OnCollisionEnter(Collision collision)
    {
        return;
    }
}
