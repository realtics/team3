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
	public CarData data;
    public int curHp;
    public float maxSpdMultiplier = 1.0f;

    void OnEnable()
    {
        curHp = data.maxHp;
        maxSpdMultiplier = 1.0f;

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
            Bullet HitBullet = other.GetComponentInParent<Bullet>();
            HitBullet.Explosion();
            DeductHp(HitBullet.bulletDamage, other.tag != "NPCBullet");
			
			if(carManager.movement.curSpeed < 10 && 
				carManager.carState == CarManager.CarState.controlledByAi)
			{
				StartCoroutine(carManager.passengerManager.GetOffTheCar(0, true));
			}
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag != "Wall" && col.transform.tag != "Car")
            return;

        int force = (int)col.relativeVelocity.sqrMagnitude / 2;

        if(force > 3)
        {
            DeductHp(force, false);

			float angle = Vector3.SignedAngle(transform.forward, col.contacts[0].normal * -1, Vector3.up);
			EnableDeltaImage(angle);
		}       
    }

    void EnableDeltaImage(float angle)
    {
        if (angle < -90)
        {
            carManager.effects.Damage(DamageDirection.rearLeft);
        }
        else if (angle < 0)
        {
            carManager.effects.Damage(DamageDirection.frontLeft);
        }
        else if (angle < 90)
        {
            carManager.effects.Damage(DamageDirection.frontRight);
        }
        else
        {
            carManager.effects.Damage(DamageDirection.rearRight);
        }
    }

    public void DeductHp(int amount, bool isDamagedByPlayer)
    {
        if (curHp <= 0)
            return;

        curHp -= amount;
        curHp = Mathf.Clamp(curHp, 0, data.maxHp);

        if(curHp <= 0)
        {
            carManager.OnDestroyEvent(isDamagedByPlayer);
        }
        else
        {
            carManager.OnDamageEvent(isDamagedByPlayer);
        }
    }

    void OnCarDamage(bool isDamagedByPlayer)
    {
        if (curHp < 100)
        {
            maxSpdMultiplier = 0.5f;
        }
        else if (curHp < 200)
        {
            maxSpdMultiplier = 0.8f;
        }
        if (isDamagedByPlayer)
        {
			WantedLevel.instance.CommitCrime(WantedLevel.CrimeType.hitCar, transform.position);
		}            
    }

    void OnCarDestroy(bool isDamagedByPlayer)
    {
		maxSpdMultiplier = 0.0f;
        GameManager.Instance.IncreaseMoney(data.score);
        WorldUIManager.Instance.SetScoreText(transform.position, data.score);

        if (isDamagedByPlayer)
        {
			WantedLevel.instance.CommitCrime(WantedLevel.CrimeType.destroyCar, transform.position);    
        }

        CameraController.Instance.StartShake(0.3f, transform.position);
        StartCoroutine(Explode(isDamagedByPlayer));
    }
    IEnumerator Explode(bool isDamagedByPlayer)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2f);
        foreach (var col in colliders)
        {
            if (col.gameObject == this)
                continue;

            if (col.tag != "NPC" && col.tag != "Car" && col.tag != "Player")
                continue;

            float dist = (col.transform.position - transform.position).magnitude;
            dist /= 2f;

            if(col.tag == "Car")
            {
                yield return new WaitForSeconds(0.1f);
                col.GetComponent<CarDamage>().DeductHp((int)(data.maxHp * 3 * (1 - dist)), isDamagedByPlayer);
            }
            else//폭발에 의한 밀림
            {
				col.GetComponent<People>().Runover((int)(300 * (1 - dist)), transform.position);
			}
        }
    }
}
