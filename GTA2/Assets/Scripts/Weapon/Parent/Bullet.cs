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


    public BulletInformation bulletInfo;
    public GameObject explosionPref;
    public AudioClip explosionSound;


    protected Vector3 bulletStartPos;
    protected Vector3 bulletDir;
    protected GunState bulletType;

    protected CapsuleCollider collider;
    protected float bulletLifeDelta = .0f;
    protected float bulletArea;

    protected bool isLife = false;
    protected float bulletActiveDelta = .0f;
    protected float bulletDeActiveTime = .01f;

    protected ExplosionEffect explosionEffect;

    protected virtual void Awake()
    {
        collider = GetComponentInChildren<CapsuleCollider>();
        collider.isTrigger = true;

        bulletArea = collider.radius;

        bulletDamage = bulletInfo.bulletDamage;
        bulletLifeTime = bulletInfo.bulletLifeTime;
        bulletSpeed = bulletInfo.bulletSpeed;
        explosionArea = bulletInfo.explosionArea;

        if (explosionPref != null)
        {
            explosionEffect = Instantiate(explosionPref).GetComponent<ExplosionEffect>();
            explosionEffect.gameObject.transform.parent = PoolManager.Instance.transform;
        }
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

        if (collider != null)
        {
            collider.radius = bulletArea;
        }
    }


    protected virtual void FixedUpdate()
    {
        UpdateBullet();
        UpdateActive();
    }
    protected virtual void UpdateBullet()
    {
        bulletDir.y = .0f;
        transform.position +=  bulletDir * bulletSpeed * Time.deltaTime;

        bulletLifeDelta += Time.deltaTime;
        if (bulletLifeTime < bulletLifeDelta)
        {
            bulletLifeDelta = .0f;
            Explosion();
        }
    }

    public virtual void Explosion()
    {
        if (collider != null)
        {
            collider.radius = explosionArea;
        }

        isLife = false;

        if (explosionEffect != null)
        {
            explosionEffect.SetExplosion(transform.position);
        }

         SoundManager.Instance.PlayClipToPosition(explosionSound, SoundPlayMode.OneShotPosPlay, transform.position);
    }

    protected void UpdateActive()
    {
        if (isLife)
        {
            return;
        }

        bulletActiveDelta += Time.deltaTime;
        if (bulletActiveDelta > bulletDeActiveTime)
        {
            PoolManager.ReleaseObject(gameObject);
        }          
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            Explosion();
        }
    }


    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Explosion();
        }
    }
}
