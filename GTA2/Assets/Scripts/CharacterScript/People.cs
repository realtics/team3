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
    //Sound
    [SerializeField]
    protected AudioClip punchClip;
    protected SpriteRenderer spriteRenderer;
    //Timer
    public float[] checkingTimes;
    public float[] Timers = { 0.0f };
    public enum TimerType
    {
        Jump,
        JumpMin,
        Down,
        Runover,
        CarOpen,
        Respawn, //player only
        PatternChange, //NPC only
        RunAway //Citizen only
    }
    //protected float jumpTime = 1.0f;
    //protected float jumpMinTime = 0.5f;
    //protected float downTime;
    //   protected float jumpTime = 1.0f;
    //   protected float jumpMinTime = 1.0f;
    //   protected float downTime = 1.0f;
	//protected float runoverTime = 1.0f;
    
	
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
    //State
    public bool isWalk { get; set; }
	public bool isShot { get; set; }
	public bool isPunch { get; set; }
	public bool isJump { get; set; }
	public bool isDown { get; set; }
	public bool isDie { get; set; }
	public bool isRunover { get; set; }
	public bool isGetOnTheCar { get; set; }//문여는 모션

	BulletEffect bloodEffect;

	//abstract
	protected abstract void Die();

    protected virtual void Move()
    {
        Vector3 Pos = transform.position;
        Pos.x += transform.forward.x * Time.deltaTime * moveSpeed;
        Pos.z += transform.forward.z * Time.deltaTime * moveSpeed;
        
        transform.position = Pos;
    }
	public virtual void Down()
	{
		GetComponent<BoxCollider>().isTrigger = true;
		GetComponent<Rigidbody>().isKinematic = true;
		isRunover = false;
		isDown = true;
	}
	public virtual void Rising()
	{
		GetComponent<BoxCollider>().isTrigger = false;
		GetComponent<Rigidbody>().isKinematic = false;
		isDown = false;
	}
	public void Hurt(int damage)
    {
        hp -= damage;
		GameObject bloodGameObject = PoolManager.SpawnObject(NPCSpawnManager.Instance.BloodAnim);
		bloodGameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
		bloodGameObject.transform.Rotate(90, 0, 0);

		if (hp <= 0)
        {
            Die();
            isDie = true;
        }
    }
    public void PeopleUpdate()
    {
		if (isDie)
			return;
        else if (isDown)
            TimerCheck(TimerType.Down);
        else if(isRunover)
		{
			if(TimerCheck(TimerType.Runover))
				isRunover = false;
		}
        else if (isJump)
			LandCheck();
    }
	
	protected bool IsStuckedAnimation()
	{
		if (isDie || isDown || isGetOnTheCar || isRunover)
			return true;
		else
			return false;
	}
	#region lowlevelCode
	public virtual void Runover(float runoverSpeed, Vector3 carPosition, bool Player = false)
    {
		if (runoverSpeed < runoverMinSpeed)
			return;
		Vector3 runoverVector = transform.position - carPosition;

        //속도에 비례한 피해 데미지 보정수치
        this.runoverSpeed = Mathf.Clamp((runoverSpeed / 3000.0f), 0, 0.3f);
		this.runoverVector = (runoverVector.normalized * this.runoverSpeed * Mathf.Abs(Vector3.Dot(runoverVector, Vector3.right)));
		isRunover = true;
        hDir = 0; vDir = 0;

        transform.LookAt(carPosition);

        if (runoverSpeed > runoverHurtMinSpeed)
		{
			Hurt((int)(runoverSpeed / 2));
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
		Timers = new float[(int)TimerType.RunAway];
		checkingTimes = new float[(int)TimerType.RunAway + 1];
	}

	protected bool TimerCheck(TimerType timerType)
    {
        Timers[(int)timerType] += Time.deltaTime;

        if (Timers[(int)timerType] > checkingTimes[(int)timerType])
        {
            Timers[(int)timerType] = 0.0f;
            return true;
        }
        return false;
    }
    protected void SetTimerDefault(TimerType timerType)
    {
        Timers[(int)timerType] = 0.0f;
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