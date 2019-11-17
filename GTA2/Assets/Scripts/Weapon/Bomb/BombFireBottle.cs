using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombFireBottle : Bullet
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
        Vector3 newVec3 = bulletDir * bulletSpeed * forceValue;
        newVec3.y = yLaunchPower;

        myRigidBody.velocity = Vector3.zero;
        myRigidBody.AddForce(newVec3, ForceMode.Impulse);
    }



    protected void OnTriggerEnter(Collider other)
    {
        base.Explosion();
        camExplosion.StartShake(explosionPower - (transform.position - bulletStartPos).magnitude);
    }
}
