using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageDirection
{
    frontLeft, frontRight, rearLeft, rearRight
}

[RequireComponent(typeof(CarManager))]
public class CarDamage : MonoBehaviour
{
    public CarManager carManager;

    public int maxHp = 100;
    public int hp;
    public float maxSpdMultiplier = 1.0f;

    void OnEnable()
    {
        hp = maxHp;

        carManager.OnDamage += OnCarDamage;
        carManager.OnDestroy += OnCarDestroy;
    }

    void OnDisable()
    {
        carManager.OnDamage -= OnCarDamage;
        carManager.OnDestroy -= OnCarDestroy;
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
        }       
    }

    void EnableDeltaImage(float angle)
    {
        if (angle < -90)
        {
            carManager.carEffects.Damage(DamageDirection.rearLeft);
        }
        else if (angle < 0)
        {
            carManager.carEffects.Damage(DamageDirection.frontLeft);
        }
        else if (angle < 90)
        {
            carManager.carEffects.Damage(DamageDirection.frontRight);
        }
        else
        {
            carManager.carEffects.Damage(DamageDirection.rearRight);
        }
    }

    public void DeductHp(int amount)
    {
        hp -= amount;
        hp = Mathf.Clamp(hp, 0, maxHp);

        if(hp <= 0)
        {
            carManager.OnDestroyEvent();
        }
        else
        {
            carManager.OnDamageEvent();
        }
    }

    void OnCarDamage()
    {
        if (hp < 30)
        {
            maxSpdMultiplier = 0.5f;
        }
        else if (hp < 60)
        {
            maxSpdMultiplier = 0.8f;
        }
    }

    void OnCarDestroy()
    {
        maxSpdMultiplier = 0.0f;
    }
}
