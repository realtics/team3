﻿using System.Collections;
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
	protected float jumpTime = 1.0f;
	protected float jumpTimer;
	protected float jumpMinTime = 0.5f;
	protected float downTimer;
	protected float downTime;

	float runoverTimer = 0.0f;
	float runoverTime = 1.0f;

	//Physics
	public Rigidbody rigidbody;
	protected BoxCollider boxCollider;
	protected float rotateSpeed;
	protected float moveSpeed;

	protected Vector3 movement;
	protected Vector3 direction;
	protected Vector3 targetDirectionVector = Vector3.zero;
	protected Vector3 runoverVector;
	protected float runoverSpeed;
	protected float runoverMinSpeed = 100;

	protected RaycastHit hit;
	[Header("이 오브젝트와 작동할 Layer")]
	public LayerMask collisionLayer;

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

        if (hp <= 0)
        {
            Die();
            isDie = true;
        }
    }
    public void PeopleUpdate()
    {
        if (isDown)
            DownCheck();
        else if(isRunover)
            RunoverCheck();
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
	public virtual void Runover(float runoverSpeed, Vector3 carPosition)
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

        if(isDown && runoverSpeed > 10)
        {
            Hurt(9999);
        }
		else if (runoverSpeed > 200)
		{
			Hurt((int)(runoverSpeed / 4));
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
		jumpTimer = 0.0f;
		transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
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
    public bool JumpTimerCheck()
	{
		jumpTimer += Time.deltaTime;
		DebugX.DrawRay(transform.position, transform.up * -1, Color.magenta);

		if (jumpTimer > jumpTime)
		{
			jumpTimer = 0.0f;
			return true;
		}
		return false;
	}
	public bool JumpMinTimeCheck()
	{
		if (jumpTimer > jumpMinTime)
			return true;
		else
			return false;
	}
    void DownCheck()
    {
        downTimer += Time.deltaTime;

        if (downTimer > downTime)
        {
            downTimer = 0;
            isDown = false;
            Rising();
        }
    }
    
    void RunoverCheck()
    {
        runoverTimer += Time.deltaTime;

        //충돌된 방향으로 날아가기
        transform.position += runoverVector;

        if (runoverTimer > runoverTime)
        {
            runoverTimer = 0.0f;
            isRunover = false;

            //보정 수치
            if (runoverSpeed > 150.0f)
            {
                hDir = 0; vDir = 0;

                Down();
            }
        }
    }
    protected virtual void LandCheck()
	{
		if (JumpTimerCheck())
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