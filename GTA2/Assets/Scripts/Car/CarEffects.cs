using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarManager))]
public class CarEffects : MonoBehaviour
{
    public CarManager carManager;

    public GameObject sirenL;
    public GameObject sirenR;
    public float sirenSpeed;

    public GameObject lightFL, lightFR, lightRL, lightRR;
    public GameObject deltaFL, deltaFR, deltaRL, deltaRR;
    public GameObject debris;

    public TrailRenderer trailLeft;
    public TrailRenderer trailRight;
    public GameObject shadow;

    public AudioClip collsionClip;
    public AudioClip explosionClip;

    public AudioSource audioSourceEngine;
    public AudioSource audioSourceSkid;
	public AudioSource audioSourceSiren;

    public GameObject fireParticle;
    public GameObject explosionPref;


    protected ExplosionEffect explosionParticle;

    void OnEnable()
    {
        carManager.OnDestroy += FullyDestroy;
        carManager.OnDestroy += EnableParticle;
		carManager.OnDestroy += PlayExplosionSound;
        carManager.OnDamage += EnableParticle;
		carManager.OnDamage += PlayCrashSound;

		Init();
    }

    void OnDisable()
    {
        carManager.OnDestroy -= FullyDestroy;
        carManager.OnDestroy -= EnableParticle;
		carManager.OnDestroy -= PlayExplosionSound;
        carManager.OnDamage -= EnableParticle;
		carManager.OnDamage -= PlayCrashSound;
	}

    void Init()
    {
        lightFL.SetActive(false);
        lightFR.SetActive(false);
        lightRL.SetActive(false);
        lightRR.SetActive(false);
        deltaFL.SetActive(false);
        deltaFR.SetActive(false);
        deltaRL.SetActive(false);
        deltaRR.SetActive(false);
        debris.SetActive(false);

        fireParticle.SetActive(false);

        audioSourceEngine.volume = 0.1f;

        if (explosionPref != null)
        {
            explosionParticle = Instantiate(explosionPref).GetComponent<ExplosionEffect>();
            explosionParticle.gameObject.transform.parent = PoolManager.Instance.transform;
            explosionParticle.gameObject.name = "Test";
        }            
    }

    void Update()
    {
        UpdateShadowPosition();
        DrawSkidMark(carManager.input.GetInputH(), carManager.movement.curSpeed);
        AdjustEngineSound(carManager.movement.curSpeed, carManager.carState);
    }

    IEnumerator SirenCor()
    {
        while (true)
        {
            sirenL.SetActive(true);
            sirenR.SetActive(false);
            yield return new WaitForSeconds(sirenSpeed);

            sirenL.SetActive(false);
            sirenR.SetActive(true);
            yield return new WaitForSeconds(sirenSpeed);
        }
    }

    public void TurnOnSiren()
    {
        StopAllCoroutines();

        if (sirenL != null && sirenR != null)
        {
			if (audioSourceSiren != null)
				audioSourceSiren.Play();
			StartCoroutine(SirenCor());
        }
    }

    public void TurnOffSiren()
    {
        StopAllCoroutines();
		if(audioSourceSiren != null)
			audioSourceSiren.Stop();
        sirenL.SetActive(false);
        sirenR.SetActive(false);
    }

    public void TurnOnFrontLight()
    {
        if (deltaFL.activeSelf || deltaFR.activeSelf)
            return;

        lightFL.SetActive(true);
        lightFR.SetActive(true);
    }

    public void TurnOffFrontLight()
    {
        lightFL.SetActive(false);
        lightFR.SetActive(false);
    }

    public void TurnOnRearLight()
    {
        if (deltaRL.activeSelf || deltaRR.activeSelf)
            return;

        lightRL.SetActive(true);
        lightRR.SetActive(true);
    }

    public void TurnOffRearLight()
    {
        lightRL.SetActive(false);
        lightRR.SetActive(false);
    }

    public void Damage(DamageDirection damageDirection)
    {
        switch (damageDirection)
        {
            case DamageDirection.frontLeft:
                {
                    deltaFL.SetActive(true);
                }
                break;
            case DamageDirection.frontRight:
                {
                    deltaFR.SetActive(true);
                }
                break;
            case DamageDirection.rearLeft:
                {
                    deltaRL.SetActive(true);
                }
                break;
            case DamageDirection.rearRight:
                {
                    deltaRR.SetActive(true);
                }
                break;
        }
    }

    void FullyDestroy(bool sourceIsPlayer)
    {
        if(sirenL != null)
            TurnOffSiren();

        lightFL.SetActive(false);
        lightFR.SetActive(false);
        lightRL.SetActive(false);
        lightRR.SetActive(false);
        deltaFL.SetActive(false);
        deltaFR.SetActive(false);
        deltaRL.SetActive(false);
        deltaRR.SetActive(false);

        debris.SetActive(true);
    }

    public void DrawSkidMark(float inputH, float curSpeed)
    {
        if (Mathf.Abs(inputH) > 0.3f && curSpeed > 150)
        {
            trailLeft.emitting = true;
            trailRight.emitting = true;
            if (!audioSourceSkid.isPlaying)
                audioSourceSkid.Play();
        }
        else
        {
            trailLeft.emitting = false;
            trailRight.emitting = false;
            audioSourceSkid.Stop();
        }
    }

    void UpdateShadowPosition()
    {
        shadow.transform.position = transform.position + new Vector3(0.05f, 0, -0.05f);
    }

    public void AdjustEngineSound(float curSpeed, CarManager.CarState carState)
    {
        float temp = 0.5f;
        temp += Mathf.Clamp(Mathf.Abs(curSpeed) / 300, 0.0f, 2.0f);
        if (curSpeed > 0)
            temp -= (int)(curSpeed / 150) * 0.3f;

        audioSourceEngine.pitch = temp;

        if (carState != CarManager.CarState.controlledByPlayer)
        {
            audioSourceEngine.volume = Mathf.Clamp(curSpeed / 100, 0, 0.1f);
        }
        else
        {
            audioSourceEngine.volume = 1;
        }
    }

    void EnableParticle(bool sourceIsPlayer)
    {
        if (carManager.damage.curHp <= 0)
        {
            fireParticle.SetActive(false);
            explosionParticle.SetExplosion(gameObject.transform.position /*+ new Vector3(.0f, .5f)*/);
        }
        else if (carManager.damage.curHp < 100)
        {
            fireParticle.SetActive(true);
        }
    }

	void PlayCrashSound(bool sourceIsPlayer)
	{
		if(carManager.carState == CarManager.CarState.controlledByPlayer)
			SoundManager.Instance.PlayClipFromPosition(collsionClip, true, gameObject.transform.position);
	}

	void PlayExplosionSound(bool sourceIsPlayer)
	{
		SoundManager.Instance.PlayClipFromPosition(explosionClip, true, gameObject.transform.position);
	}
}
