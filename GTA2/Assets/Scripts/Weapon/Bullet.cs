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

    public AudioSource bulletSoundSource;
    public GameObject explosionPref;

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

    protected virtual void Start()
    {
        collider = GetComponent<CapsuleCollider>();
        collider.isTrigger = true;

        bulletArea = collider.radius;

        if (explosionPref != null)
        {
            explosionEffect = Instantiate(explosionPref).GetComponent<ExplosionEffect>();
            explosionEffect.gameObject.transform.parent = SetPool.Instance.transform;
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


    protected virtual void Update()
    {
        UpdateBullet();
        UpdateActive();
    }
    protected virtual void UpdateBullet()
    {
        bulletDir.y = .0f;
        transform.position +=  bulletDir * bulletSpeed * Time.deltaTime;
        // transform.position = Vector3.MoveTowards(transform.position, bulletDir,bulletSpeed * Time.deltaTime);
        // transform.Translate(transform.forward * bulletSpeed * Time.deltaTime * -1.0f);

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

        if (bulletSoundSource != null)
        {
            bulletSoundSource.Play();
        }
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
            transform.position = new Vector3(10000.0f, 10000.0f, 10000.0f);
            gameObject.SetActive(false);
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
