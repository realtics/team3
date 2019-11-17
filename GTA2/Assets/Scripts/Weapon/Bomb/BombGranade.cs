using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombGranade : Bullet
{
    // Start is called before the first frame update
    public Rigidbody myRigidBody;
    public float yLaunchPos;
    public float yLaunchPower;


    private float explosionPower = 5.0f;
    private TempCamCtr camExplosion;

    protected override void Start()
    {
        base.Start();
        myCollider.isTrigger = false;

        camExplosion = Camera.main.gameObject.GetComponent<TempCamCtr>();
    }


    // Update is called once per frame
    public override void SetBullet(GunState type, Vector3 pos, Vector3 dir, float bullettoSize)
    {
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

    protected override void Explosion()
    {
        base.Explosion();
        camExplosion.StartShake(Mathf.Abs(explosionPower - (transform.position - bulletStartPos).magnitude));
    }
}
