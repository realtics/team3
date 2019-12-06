using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEffect : MonoBehaviour
{
    // Start is called before the first frame update
    protected GameObject bulletTarget = null;
    protected ParticleSystem particle = null;

    // Start is called before the first frame update
    protected float releaseTime = 3.0f;
    protected float stopTime = 1.0f;
    protected float releaseDelta = .0f;

    protected virtual void Awake()
    {
        particle = GetComponentInChildren<ParticleSystem>();
        particle.Play();
    }


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
            transform.position = bulletTarget.transform.localPosition;
        }
    }
    void UpdateRelease()
    {
        if (null != bulletTarget)
        {
            return;
        }
        

        releaseDelta += Time.deltaTime;
        if (releaseDelta > stopTime)
        {
            particle.Stop();
        }
        if (releaseDelta > releaseTime)
        {
            particle.Stop();
            PoolManager.ReleaseObject(gameObject);
            bulletTarget = null;
        }
    }
}
