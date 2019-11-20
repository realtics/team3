using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletRocketLauncher : Bullet
{
    // Start is called before the first frame update
    TempCamCtr camExplosion;
    float explosionPower = 5.0f;
    protected override void Start()
    {
        base.Start();
        camExplosion = Camera.main.gameObject.GetComponent<TempCamCtr>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    protected override void Explosion()
    {
        if (myCollider != null)
        {
            myCollider.radius = explosionArea;
        }

        camExplosion.StartShake(Mathf.Abs(explosionPower - (transform.position - bulletStartPos).magnitude));
        isLife = false;
    }
}
