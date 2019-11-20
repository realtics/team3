using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageDirection
{
    frontLeft, frontRight, rearLeft, rearRight
}

public class CarDamage : MonoBehaviour
{
    public CarController carController;
    public DeltasController deltasController;
    public int maxHp = 100;
    public int hp;
    public GameObject fireParticle;
    public GameObject explosionParticle;

    private void Start()
    {
        hp = maxHp;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet") || 
            other.CompareTag("PlayerFireBullet") ||
            other.CompareTag("NPCBullet"))
        {
            Bullet HitBullet = other.GetComponent<Bullet>();
            other.gameObject.SetActive(false);

            DeductHp(HitBullet.bulletDamage);
            EnableParticle();
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag != "Wall" && col.transform.tag != "Car")
            return;

        int force = (int)col.relativeVelocity.sqrMagnitude / 3;
        if(force > 1)
        {
            DeductHp(force);

            float angle = Vector3.SignedAngle(transform.forward, col.contacts[0].normal * -1, Vector3.up);
            EnableDeltaImage(angle);
            EnableParticle();
        }       
    }

    void EnableDeltaImage(float angle)
    {
        if (angle < -90)
        {
            deltasController.Damage(DamageDirection.rearLeft);
        }
        else if (angle < 0)
        {
            deltasController.Damage(DamageDirection.frontLeft);
        }
        else if (angle < 90)
        {
            deltasController.Damage(DamageDirection.frontRight);
        }
        else
        {
            deltasController.Damage(DamageDirection.rearRight);
        }
    }

    public void DeductHp(int amount)
    {
        hp -= amount;
        hp = Mathf.Clamp(hp, 0, maxHp);
        carController.OnCarHpChanged(hp);

        if(hp <= 0)
        {
            deltasController.FullyDestroy();
        }
    }

    void EnableParticle()
    {
        if(hp <= 0)
        {
            fireParticle.SetActive(false);
            explosionParticle.SetActive(true);
        }
        else if(hp < 30)
        {
            fireParticle.SetActive(true);
        }
    }
}
