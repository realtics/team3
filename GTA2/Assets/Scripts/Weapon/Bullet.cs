using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update
    public int bulletDamage;

    public float bulletLifeTime;
    public float bulletSpeed;


    public Vector3 bulletDir;
    public GUNSTATE bulletType;

    protected BoxCollider  myCollider;
    protected float bulletLifeDelta = .0f;
    protected virtual void Start()
    {
        myCollider = GetComponent<BoxCollider>();
        myCollider.isTrigger = true;
    }

    public virtual void SetBullet(GUNSTATE type, Vector3 triggerPos, Vector3 dir, float bullettoSize)
    {
        bulletType = type;
        bulletDir = dir;


        Vector3 PosTmp = triggerPos + bulletDir * bullettoSize;
        PosTmp.y = triggerPos.y;
        bulletLifeDelta = .0f;



        transform.eulerAngles = new Vector3(90.0f, dir.y + 90.0f, 90.0f);
        transform.position = PosTmp;
        gameObject.SetActive(true);
    }


    protected virtual void Update()
    {
        UpdateBullet();
    }
    protected virtual void UpdateBullet()
    {
        bulletDir.y = .0f;
        transform.position += bulletDir * bulletSpeed * Time.deltaTime;

        bulletLifeDelta += Time.deltaTime;
        if (bulletLifeTime < bulletLifeDelta)
        {
            bulletLifeDelta = .0f;
            Explosion();
        }
    }


    protected virtual void Explosion()
    {
        transform.position = new Vector3(10000.0f, 10000.0f, 10000.0f);
        gameObject.SetActive(false);
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            Explosion();
        }
    }


    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Explosion();
        }
    }
}
