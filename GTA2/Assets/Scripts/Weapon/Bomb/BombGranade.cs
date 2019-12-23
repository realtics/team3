using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombGranade : Bullet
{
    // Start is called before the first frame update
    [Header("Physics")]
    public Rigidbody rigidBody;

    [Header("Y Valuable")]
    public float yLaunchPos;
    public float yLaunchPower;


    float explosionPower = 2.0f;

    protected override void Awake()
    {
        base.Awake();
        bulletDeActiveTime = .1f;
        bulletcollider.isTrigger = false;
    }


    // Update is called once per frame
    public override void SetBullet(GunState type, Vector3 pos, Vector3 dir, float bullettoSize)
    {
        bulletcollider.isTrigger = false;
        base.SetBullet(type, pos, dir, bullettoSize);
        transform.position += Vector3.up * yLaunchPos;
    }

    public void SetForce(float forceValue)
    {
        Vector3 NewVec3 = bulletDir * bulletSpeed * forceValue;
        NewVec3.y = yLaunchPower;

        rigidBody.velocity = Vector3.zero;
        rigidBody.AddForce(NewVec3, ForceMode.Impulse);
    }

    public override void Explosion()
    {
        bulletcollider.isTrigger = true;
        base.Explosion();
        CameraController.Instance.StartShake(explosionPower, transform.position);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        return;
    }
}
