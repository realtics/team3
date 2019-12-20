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
	bool isSirenOn = false;

    public GameObject lightFL, lightFR, lightRL, lightRR;
    public GameObject deltaFL, deltaFR, deltaRL, deltaRR;
    public GameObject debris;

    public TrailRenderer trailLeft;
    public TrailRenderer trailRight;
    public GameObject shadow;

    public AudioClip collsionClip;
    public AudioClip explosionClip;
	public AudioClip doorOpenClip;
	public AudioClip doorCloseClip;

	float engineIdlePitch = 0.5f;
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
		carManager.OnDriverGetOff += TurnOffSiren;
		carManager.OnDoorOpen += OnDoorOpen;
		carManager.OnDoorClose += OnDoorClose;

		Init();
    }

    void OnDisable()
    {
        carManager.OnDestroy -= FullyDestroy;
        carManager.OnDestroy -= EnableParticle;
		carManager.OnDestroy -= PlayExplosionSound;
        carManager.OnDamage -= EnableParticle;
		carManager.OnDamage -= PlayCrashSound;
		carManager.OnDriverGetOff -= TurnOffSiren;
		carManager.OnDoorOpen -= OnDoorOpen;
		carManager.OnDoorClose -= OnDoorClose;

		trailLeft.Clear();
		trailRight.Clear();
	}

    void Init()
    {
		trailLeft.emitting = false;
		trailRight.emitting = false;
		trailLeft.Clear();
		trailRight.Clear();

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
		engineIdlePitch = Random.Range(0.4f, 0.6f);

        if (explosionPref != null)
        {
            explosionParticle = Instantiate(explosionPref).GetComponent<ExplosionEffect>();
            explosionParticle.gameObject.transform.parent = PoolManager.Instance.transform;
            explosionParticle.gameObject.name = "CarExplosion";
        }

		TurnOffSiren(People.PeopleType.None, 0);
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
		if (isSirenOn)
			return;

        StopAllCoroutines();

        if (sirenL != null && sirenR != null)
        {
			if (audioSourceSiren != null)
				audioSourceSiren.Play();

			isSirenOn = true;

			StartCoroutine(SirenCor());
        }
    }

    public void TurnOffSiren(People.PeopleType peopleType, int idx)
    {
		if (sirenL == null)
			return;

		isSirenOn = false;

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
            TurnOffSiren(People.PeopleType.None, 0);

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
        if (Mathf.Abs(inputH) > 0.5f && curSpeed > 160)
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
        float engienPitch = engineIdlePitch;
        engienPitch += Mathf.Clamp(Mathf.Abs(curSpeed) / 300, 0.0f, 2.0f);
        if (curSpeed > 0)
            engienPitch -= (int)(curSpeed / 150) * 0.3f;

        audioSourceEngine.pitch = engienPitch;

        if (carState == CarManager.CarState.controlledByPlayer)
        {
			audioSourceEngine.volume = Mathf.Clamp(Mathf.Abs(curSpeed) / 100, 0.3f, 0.6f);
        }
        else
        {
			audioSourceEngine.volume = Mathf.Clamp(curSpeed / 100, 0, 0.1f);
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
			SoundManager.Instance.PlayClipToPosition(collsionClip, SoundPlayMode.ObjectSFX, gameObject.transform.position);
	}

	void PlayExplosionSound(bool sourceIsPlayer)
	{
		SoundManager.Instance.PlayClipToPosition(explosionClip, SoundPlayMode.ExplosionSFX, gameObject.transform.position);
	}

	void OnDoorOpen(int idx)
	{
		SoundManager.Instance.PlayClipToPosition(doorOpenClip, SoundPlayMode.ExplosionSFX, gameObject.transform.position);
	}

	void OnDoorClose(int idx)
	{
		SoundManager.Instance.PlayClipToPosition(doorCloseClip, SoundPlayMode.ExplosionSFX, gameObject.transform.position);
	}
}
