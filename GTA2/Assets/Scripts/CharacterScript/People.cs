using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class People : MonoBehaviour
{
    public enum PeopleType
    {
        None,
        Player,
        Citizen,
        Police,
        Doctor
    };
    public enum DiePattern
    {
        Normal,
        Burn,
        airBorne
    }
    //Sound
    [SerializeField]
    protected AudioClip punchClip;
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer burnedSprite;
    //Timer
    public float[] checkingTimes { get; set; }
	public float[] Timers { get;set; } = { 0.0f };
    public enum TimerType
    {
        Jump,
        JumpMin,
        Down,
        Land,
        Runover,
        CarOpen,
		AutoLand,
        Respawn, //player only
        PatternChange, //NPC only
		Heal,
        RunAway //Citizen only RunAway는 항상 마지막에
    }
    	
	//Physics
	public Rigidbody rigidbody;
	public BoxCollider boxCollider;
	protected float rotateSpeed;
	protected float moveSpeed;

	protected Vector3 movement;
	protected Vector3 direction;
	protected Vector3 targetDirectionVector = Vector3.zero;
	protected Vector3 runoverVector;
	protected float runoverSpeed;
	protected float runoverMinSpeed = 80;
	protected float runoverHurtMinSpeed = 100;

	protected RaycastHit hit;
	[Header("이 오브젝트와 작동할 Layer")]
	public LayerMask collisionLayer;
	public LayerMask groundLayer;


	[SerializeField]protected int hp;
	protected int defaultHp;
	protected float hDir = 0;
	protected float vDir = 0;
	protected bool isAirborne = false;
    //State
    public bool isWalk { get; set; }
	public bool isShot { get; set; }
	public bool isPunch { get; set; }
	public bool isJump { get; set; }
	public bool isDown { get; set; }
	public bool isDie { get; set; }
	public bool isRunover { get; set; }
	public bool isGetOnTheCar { get; set; } //문여는 모션
	
	protected bool isburned;
    protected bool isElectric;
    protected GameObject bloodEffect;
    protected GameObject burnedEffect;
    protected GameObject electricEffect;

    //abstract
    protected virtual void Die()
	{
		if (isDie)
			return;
		if (isJump)
			Land();
		isDie = true;

		rigidbody.velocity = Vector3.zero;
		rigidbody.isKinematic = true;
		boxCollider.enabled = false;
		hDir = 0; vDir = 0;
	}

    protected virtual void Move()
    {
        Vector3 Pos = transform.position;
        Pos.x += transform.forward.x * Time.deltaTime * moveSpeed;
        Pos.z += transform.forward.z * Time.deltaTime * moveSpeed;
        
        transform.position = Pos;
    }
	public virtual void Down()
	{
		boxCollider.isTrigger = true;
		rigidbody.isKinematic = true;
		isRunover = false;
		isDown = true;
	}
	public virtual void Rising()
	{
		boxCollider.isTrigger = false;
		rigidbody.isKinematic = false;
		isDown = false;
	}
	public void Hurt(int damage, DiePattern diePattern = DiePattern.Normal)
    {
        hp -= damage;
		GameObject bloodGameObject = PoolManager.SpawnObject(NPCEffectManager.Instance.BloodAnim);
		bloodGameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
		bloodGameObject.transform.Rotate(90, 0, 0);

		if (hp <= 0)
        {
            if(diePattern == DiePattern.Burn)
            {
                burnedDieSetting();
            }
            else if(diePattern == DiePattern.airBorne)
            {
                isAirborne = true;
                rigidbody.AddForce(new Vector3(0, 10, 0), ForceMode.VelocityChange);
                return;
            }
            Die();
            isDie = true;
        }
    }
	protected virtual IEnumerator Burning()
	{
		if (isburned)
			yield break;

		isburned = true;
		burnedEffect = NPCEffectManager.Instance.SpawnBurnedEffect(gameObject);

		while (true)
		{
			Hurt(10, DiePattern.Burn);

			if(isDie)
			{
				NPCEffectManager.Instance.ReleaseBurnedEffect(burnedEffect);
                isburned = false;
                
                break;
			}
			yield return new WaitForSeconds(0.5f);
		}
	}
    protected virtual IEnumerator ElectricDie()
    {
        if (isElectric)
            yield break;

        isElectric = true;
        electricEffect = NPCEffectManager.Instance.SpawnElectricEffect(gameObject);

        yield return new WaitForSeconds(3.0f);
        isElectric = false;
        NPCEffectManager.Instance.ReleaseBurnedEffect(electricEffect);
    }
    public void PeopleUpdate()
    {
		if (isDie)
			return;
        else if (isDown)
		{
			if(TimerCheck(TimerType.Down))
				Rising();
		}
        else if(isRunover)
		{
			if(TimerCheck(TimerType.Runover))
				isRunover = false;
		}
        else if (isJump)
			LandCheck();
    }
	protected virtual void InitAnimation()
	{
		isWalk = false;
		isShot = false;
		isPunch = false;
	 	isJump = false;
		isDown = false;
		isDie = false;
		isRunover = false;
		isGetOnTheCar = false;
	}
	protected bool IsStuckedAnimation()
	{
		if (isDie || isDown || isGetOnTheCar || isRunover)
			return true;
		else
			return false;
	}
	#region lowlevelCode
    protected virtual void burnedDieSetting()
    {
        spriteRenderer.enabled = false;
        burnedSprite.gameObject.SetActive(true);
    }
	public virtual void Runover(float runoverSpeed, Vector3 carPosition, bool Player = false)
    {
		if (runoverSpeed < runoverMinSpeed)
			return;
		else if(isDown)
		{
			Hurt((int)(runoverSpeed * 5));
		}

		Vector3 runoverVector = transform.position - carPosition;

        //속도에 비례한 피해 데미지 보정수치
        this.runoverSpeed = Mathf.Clamp((runoverSpeed / 3000.0f), 0, 0.3f);
		this.runoverVector = (runoverVector.normalized * this.runoverSpeed * Mathf.Abs(Vector3.Dot(runoverVector, Vector3.right)));
		isRunover = true;
        hDir = 0; vDir = 0;

        transform.LookAt(carPosition);

        if (runoverSpeed > runoverHurtMinSpeed)
		{
			Hurt((int)(runoverSpeed / 3));
		}
		
    }
	public void Jump()
	{
		if (isJump)
			return;
		isJump = true;
		GetComponent<Rigidbody>().useGravity = false;
		transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
	}
	protected void Land()
	{
		isJump = false;
        //jumpTimer = 0.0f;
        Timers[(int)TimerType.Jump] = 0.0f;

        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 5f, groundLayer))
		{
			transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
		}
		GetComponent<Rigidbody>().useGravity = true;
	}
    protected virtual void UpdateTargetRotation()
    {
        targetDirectionVector = new Vector3(hDir, 0, vDir).normalized;
    }
    protected virtual void UpdateSlerpedRotation()
    {
        if (0 != hDir || 0 != vDir)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetDirectionVector), 0.4f);
        }
    }
	protected void TimerInit()
	{
		Timers = new float[(int)TimerType.RunAway + 1];
		checkingTimes = new float[(int)TimerType.RunAway + 1];
	}

	protected bool TimerCheck(TimerType timerType)
    {
        Timers[(int)timerType] += Time.deltaTime;

		if (Timers[(int)timerType] > checkingTimes[(int)timerType])
        {
			SetTimerDefault(timerType);
            return true;
        }
        return false;
    }
	protected bool DelayTimerCheck(TimerType timerType)
	{
		Timers[(int)timerType] -= Time.deltaTime;

		if (Timers[(int)timerType] < 0.0f)
		{
			SetTimerTocheckingTimes(timerType);
			return true;
		}
		return false;
	}
	protected void SetTimerDefault(TimerType timerType)
    {
        Timers[(int)timerType] = 0.0f;
    }
	protected void SetTimerTocheckingTimes(TimerType timerType)
	{
		Timers[(int)timerType] = checkingTimes[(int)timerType];
	}
	
	protected virtual void LandCheck()
	{
        if(TimerCheck(TimerType.Jump))
		{
			if (!IsCarExistBelow())
				Land();
		}
	}
	public bool IsCarExistBelow()
	{
		if (Physics.Raycast(transform.position, transform.up * -1, out hit, 1f, collisionLayer)
			&& hit.transform.CompareTag("Car"))
			return true;
		else
			return false;
	}
    #endregion
}