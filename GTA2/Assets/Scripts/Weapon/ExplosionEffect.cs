using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    // Start is called before the first frame update
    protected ParticleSystem myParticle = null;

    // Start is called before the first frame update
    protected float releaseTime = 6.0f;
    protected float releaseDelta = .0f;

    void Start()
    {
        myParticle = GetComponent<ParticleSystem>();
        gameObject.SetActive(false);
    }


    void Update()
    {
        releaseDelta += Time.deltaTime;
        if (releaseDelta > releaseTime)
        {
            gameObject.transform.position = new Vector3(10000.0f, 10000.0f, 10000.0f);
            gameObject.SetActive(false);
        }
    }

    public void SetExplosion(Vector3 pos)
    {
        transform.position = pos;
        releaseDelta = .0f;
        gameObject.SetActive(true);
        myParticle.Play();
    }


}
