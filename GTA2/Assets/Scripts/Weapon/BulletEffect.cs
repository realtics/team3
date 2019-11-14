using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEffect : MonoBehaviour
{
    // Start is called before the first frame update
    protected GameObject bulletTarget = null;
    protected ParticleSystem myParticle = null;

    // Start is called before the first frame update
    protected float releaseTime = 3.0f;
    protected float stopTime = 1.0f;
    protected float releaseDelta = .0f;



    protected virtual void Update()
    {
        UpdateAcitve();
        UpdateMove();
        UpdateRelease();
    }


    void UpdateAcitve()
    {
        if (null != bulletTarget && false == bulletTarget.activeInHierarchy)
        {
            bulletTarget = null;
        }
    }
    void UpdateMove()
    {
        if (null != bulletTarget)
        {
            transform.localPosition = bulletTarget.transform.localPosition;
        }
    }
    void UpdateRelease()
    {
        if (null != bulletTarget)
        {
            return;
        }

        bool Tmp = true;
        if (Tmp)
        {

        }


        releaseDelta += Time.deltaTime;
        if (releaseDelta > stopTime)
        {
            myParticle.Stop();
        }
        if (releaseDelta > releaseTime)
        {
            myParticle.Stop();
            gameObject.SetActive(false);
            bulletTarget = null;
        }
    }
}
