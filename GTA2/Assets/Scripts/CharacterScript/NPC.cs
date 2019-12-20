using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class NPC : People
{
    //FindWay
    float distToObstacle = Mathf.Infinity;
    bool isDestReached;
	protected Vector3 destination;
	protected float runSpeed;
	//Range
	protected float findRange;
    protected float punchRange;
    protected float shotRange;
    protected float chaseRange;
    protected float outofRange;

	public bool isRunaway { get; set; }
	protected Vector3 RunawayVector;
	public bool isChasePlayer { get; set; }
	public bool isAttack { get; set; }

	//Time Check
	protected float minIdleTime;
	protected float maxIdleTime;
    protected float minWalkTime;
    protected float maxWalkTime;
	GameObject targetCar;
	public GunState curGunIndex { get; set; }
	
	//protected float patternChangeTimer;
	//protected float patternChangeTime;
	protected int money; //사망시 플레이어에게 주는 돈
	
	public Animator animator;
	public Animator animator_cloth;
	public List<NPCGun> gunList;

	public AudioClip[] runAwayClip;
	public AudioClip[] dieClip;
	void AnimationInit()
	{
		isWalk = false;
		isShot = false;
		isPunch = false;
		isJump = false;
		isDown = false;
		isDie = false;
		isDown = false;
		isRunover = false;
		isDown = false;
		isGetOnTheCar = false;
	}
	void SetDefaultHp()
	{
		hp = defaultHp;
	}
	protected void NPCOnEnable()
	{
		rigidbody = GetComponent<Rigidbody>();
		boxCollider = GetComponent<BoxCollider>();
		boxCollider.enabled = true;
		boxCollider.isTrigger = false;
		rigidbody.isKinematic = false;
		//patternChangeTimer = patternChangeTime;
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		spriteRenderer.enabled = true;
		StartCoroutine(DisableIfOutOfCamera());
		AnimationInit();
		SetDefaultHp();
	}
	protected void NPCOnDisable()
	{
		StopCoroutine(DisableIfOutOfCamera());
	}
	
	protected void NPCUpdate()
	{
		AnimateUpdate();
	}
	protected void SetRunaway()
	{
        SetTimerDefault(TimerType.PatternChange);
		Vector3 playerPosition = GameManager.Instance.player.transform.position;
        
        RunawayVector = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);
		transform.LookAt(RunawayVector);
		transform.Rotate(0, 180, 0);
		isRunaway = true;
		isWalk = true;
	}
	protected bool DetectedPlayerAttack()
    {
        if (GameManager.Instance.player.isAttack &&
            findRange > Vector3.Distance(transform.position, GameManager.Instance.player.transform.position))
            return true;
        else
            return false;
    }
	public override void Runover(float runoverSpeed, Vector3 carPosition, bool isRunoverByPlayer = false)
	{
		base.Runover(runoverSpeed, carPosition, isRunoverByPlayer);

		if (isRunoverByPlayer && isDie)
		{
			GameManager.Instance.IncreaseMoney(money);
			GameManager.Instance.killCount++;
			WorldUIManager.Instance.SetScoreText(transform.position, money);
			WantedLevel.instance.CommitCrime(WantedLevel.CrimeType.killPeople, transform.position);
		}
	}
	protected virtual void RunAway()
    {
        Vector3 Pos = transform.position;

        Pos.x += transform.forward.x * Time.deltaTime * runSpeed;
        Pos.z += transform.forward.z * Time.deltaTime * runSpeed;

        transform.position = Pos;
    }
    protected virtual void ChasePlayer()
    {
		Vector3 lookAtVector = new Vector3(GameManager.Instance.player.transform.position.x, transform.position.y, GameManager.Instance.player.transform.position.z);

        transform.LookAt(lookAtVector);
        Vector3 Pos = transform.position;
        Pos.x += transform.forward.x * Time.deltaTime * runSpeed;
        Pos.z += transform.forward.z * Time.deltaTime * runSpeed;
        transform.position = Pos;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet") || other.CompareTag("PlayerFireBullet"))
        {
            Bullet HitBullet = other.GetComponentInParent<Bullet>();

            //HitBullet.누가쐈는지
            Hurt(HitBullet.bulletDamage);
            HitBullet.Explosion();

            if (isDie)
            {
                WorldUIManager.Instance.SetScoreText(transform.position, money);
				GameManager.Instance.IncreaseMoney(money);
				GameManager.Instance.killCount++;
            }
        }
        else if(other.CompareTag("PlayerPunch"))
        {
            SoundManager.Instance.PlayClipToPosition(punchClip, SoundPlayMode.ObjectSFX, transform.position);
            Down();
        }
    }

	protected override void Die()
	{
		base.Die();
		GameManager.Instance.IncreaseMoney(money);
		NPCSpawnManager.Instance.DiedNPC.Add(this);
		SoundManager.Instance.PlayClipToPosition(dieClip[Random.Range(0, dieClip.Length)], SoundPlayMode.HumanSFX, gameObject.transform.position);
	}
	#region RefHumanCtr
	protected override void Move()
    {
        if (isDestReached)
            return;
		Vector3 dir = new Vector3(destination.x, transform.position.y, destination.z) - transform.position;
        transform.rotation = Quaternion.LookRotation(dir);

        if (distToObstacle != Mathf.Infinity)
            return;

        transform.Translate(transform.forward * moveSpeed * Time.deltaTime, Space.World);
    }
    protected void Raycast()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit, 0.5f, collisionLayer))
        {
            if (hit.transform.tag == "TrafficLight" || hit.transform.tag == "Car")
            {
                if (Vector3.Dot(transform.forward, hit.transform.forward) < -0.8f)
                {
                    distToObstacle = hit.distance;
					isWalk = false;
				}
                else
                {
                    distToObstacle = Mathf.Infinity;
					isWalk = true;
				}
            }
            else
            {
				distToObstacle = hit.distance;
            }
        }
        else
        {
            distToObstacle = Mathf.Infinity;
        }
    }
       
    void DrawRaycastDebugLine()
    {
        if (distToObstacle < Mathf.Infinity)
        {
            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.forward * 0.5f, Color.blue);
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(destination, 0.25f);
        Gizmos.DrawWireSphere(destination, 1);
    }
    public void SetDestination(Vector3 pos)
    {
        destination = pos;
        isDestReached = false;
    }
    public void Stop()
    {
        destination = transform.position;
        isDestReached = true;
    }

    #endregion
    #region RangeCheck
    protected bool InPunchRange()
    {
        if (Vector3.SqrMagnitude(GameManager.Instance.player.transform.position - transform.position) < punchRange)
		{
			if(isJump)
				Land();
			return true;
		}
        else
            return false;
    }
    protected bool InShotRange()
    {
        if (Vector3.SqrMagnitude(GameManager.Instance.player.transform.position - transform.position) < shotRange)
            return true;
        else
            return false;
    }
    protected bool InChaseRange() //차에 타고있을때 플레이어 내리기 시도하는 거리
    {
        if (Vector3.SqrMagnitude(GameManager.Instance.player.transform.position - transform.position) < chaseRange)
            return true;
        else
            return false;
    }
    protected bool PlayerOutofRange()
    {
        if (Vector3.SqrMagnitude(GameManager.Instance.player.transform.position - transform.position) > outofRange)
            return true;
        else
            return false;
    }
    #endregion
    protected void UpdateTargetDirection()
    {
        transform.Rotate(0, Random.Range(0, 360), 0);
    }
	void AnimateUpdate()
	{
		animator.SetBool("isWalk", isWalk);
		animator.SetBool("isShot", isShot);
		animator.SetBool("isPunch", isPunch);
		animator.SetBool("isJump", isJump);
		animator.SetBool("isDie", isDie);
		animator.SetBool("isDown", isDown);
		animator.SetBool("isRunover", isRunover);
		animator.SetBool("isGetOnTheCar", isGetOnTheCar);

		if (animator_cloth == null)
			return;
		animator_cloth.SetBool("isWalk", isWalk);
		animator_cloth.SetBool("isShot", isShot);
		animator_cloth.SetBool("isPunch", isPunch);
		animator_cloth.SetBool("isJump", isJump);
		animator_cloth.SetBool("isDie", isDie);
		animator_cloth.SetBool("isDown", isDown);
		animator_cloth.SetBool("isRunover", isRunover);
		animator_cloth.SetBool("isGetOnTheCar", isGetOnTheCar);
	}
	IEnumerator DisableIfOutOfCamera()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);

            Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
            float offset = 2f;
            if (pos.x < 0 - offset ||
                pos.x > 1 + offset ||
                pos.y < 0 - offset ||
                pos.y > 1 + offset)
			{
				NPCSpawnManager.Instance.NPCNum--;
				if (isDie)
					NPCSpawnManager.Instance.DiedNPC.Remove(this);
				gameObject.SetActive(false);
			}
                
        }
	}
	protected void PatternChangeTimerCheck()
	{
		if (TimerCheck(TimerType.PatternChange))
		{
			if (isWalk)
			{
				checkingTimes[(int)TimerType.PatternChange] = Random.Range(minIdleTime, maxIdleTime);
				isWalk = false;
			}
			else
			{
				checkingTimes[(int)TimerType.PatternChange] = Random.Range(minWalkTime, maxWalkTime);
				isWalk = true;
			}
		}
	}
	protected void LookAtPlayer()
	{
		transform.LookAt(new Vector3(GameManager.Instance.player.transform.position.x, transform.position.y, GameManager.Instance.player.transform.position.z));
	}
	protected void StartPunch()
	{
		isWalk = false;
		isPunch = true;
		gunList[1].GetComponent<NPCGun>().StopShot();
		gunList[0].GetComponent<NPCGun>().StartShot();
	}
	protected void StopPunch()
	{
		isPunch = false;
		isWalk = true;
		gunList[0].GetComponent<NPCGun>().StopShot();
	}
	protected void StartShot()
	{
		isWalk = false;
		isShot = true;
		isPunch = false;
		gunList[0].GetComponent<NPCGun>().StopShot();
		gunList[1].GetComponent<NPCGun>().StartShot();
	}
	protected void StopShot()
	{
		isWalk = true;
		isShot = false;
		isPunch = false;
		gunList[1].GetComponent<NPCGun>().StopShot();
	}
    public void Respawn()
	{
		rigidbody.isKinematic = false;
		boxCollider.enabled = true;
		boxCollider.isTrigger = false;
		//patternChangeTimer = patternChangeTime;
        if (spriteRenderer != null)
        {
		    spriteRenderer.enabled = true;
        }
		NPCSpawnManager.Instance.DiedNPC.Remove(this);
		AnimationInit();
		SetDefaultHp();
	}
	protected override void LandCheck()
	{
		if (TimerCheck(TimerType.Jump))
		{
			Land();
		}
	}
}