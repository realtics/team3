using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletRocketLauncher : Bullet
{
    // Start is called before the first frame update
    CameraController camExplosion;
    float explosionPower = 2.0f;
    protected override void Start()
    {
        base.Start();
        camExplosion = CameraController.Instance;
        bulletDeActiveTime = .1f;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    public override void Explosion()
    {
        base.Explosion();
        CameraController.Instance.StartShake(explosionPower, transform.position);
    }
}
