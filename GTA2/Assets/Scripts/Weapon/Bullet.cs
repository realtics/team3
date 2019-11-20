using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update
    public int bulletDamage;

    public float bulletLifeTime;
    public float bulletSpeed;
    public float explosionArea;

    protected Vector3 bulletStartPos;
    protected Vector3 bulletDir;
    protected GunState bulletType;

    protected SphereCollider myCollider;
    protected float bulletLifeDelta = .0f;
    protected float bulletArea;

    protected bool isLife = false;
    protected float bulletActiveDelta = .0f;
    protected float bulletDeActiveTime = .1f;



    protected virtual void Start()
    {
        myCollider = GetComponent<SphereCollider>();
        myCollider.isTrigger = true;

        bulletArea = myCollider.radius;
    }

    public virtual void SetBullet(GunState type, Vector3 triggerPos, Vector3 dir, float bullettoSize)
    {
        bulletType = type;
        bulletDir = dir;

        bulletStartPos = triggerPos + bulletDir * bullettoSize;
        bulletStartPos.y = triggerPos.y;
        bulletLifeDelta = .0f;

        transform.eulerAngles = new Vector3(90.0f, dir.y + 90.0f, 90.0f);
        transform.position = bulletStartPos;
        gameObject.SetActive(true);
        isLife = true;
        bulletActiveDelta = .0f;

        if (myCollider != null)
        {
            myCollider.radius = bulletArea;
        }
    }


    protected virtual void Update()
    {
        UpdateBullet();
        UpdateActive();
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
        if (myCollider != null)
        {
            myCollider.radius = explosionArea;
        }

        isLife = false;
    }

    void UpdateActive()
    {
        if (isLife)
        {
            return;
        }

        bulletActiveDelta += Time.deltaTime;
        if (bulletActiveDelta > bulletDeActiveTime)
        {
            transform.position = new Vector3(10000.0f, 10000.0f, 10000.0f);
            gameObject.SetActive(false);
        }          
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
