using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class People : MonoBehaviour
{
	protected float jumpTime = 1.5f;
	protected float jumpTimer = 0.0f;
	protected float jumpMinTime = 0.3f;

	protected float rotateSpeed = 0.1f;
    protected float moveSpeed = 0.5f;
    protected float runSpeed = 1.2f;
    
    protected Vector3 movement;
    protected Vector3 direction;
    protected Vector3 targetDirectionVector = Vector3.zero;
	protected RaycastHit hit;
	public LayerMask collisionLayer;

	[SerializeField]
    protected int hp = 100;

    protected float hDir = 0;
    protected float vDir = 0;

    public bool isWalk { get; set; }
    public bool isShot { get; set; }
    public bool isPunch { get; set; }
    public bool isJump { get; set; }
    public bool isDown { get; set; }
    public bool isDie { get; set; }
    public bool isRunover { get; set; }
    public bool isDriver;
    public abstract void Down();
    public abstract void Rising();
    protected abstract void Die();
    protected float downTimer = 0.0f;
    protected float downTime = 3.0f;
    float respawnTimer = 0.0f;
    float respawnTime = 5.0f;
    float runoverTimer = 0.0f;
    float runoverTime = 0.5f;
    protected float runoverSpeed;
    protected Vector3 runoverVector;

    protected virtual void Move()
    {
        Vector3 Pos = transform.position;

        Pos.x += transform.forward.x * Time.deltaTime * moveSpeed;
        Pos.z += transform.forward.z * Time.deltaTime * moveSpeed;

        transform.position = Pos;
    }
    public virtual void Hurt(int damage)
    {
        if (isDown)
        {
            hp -= damage * 2;
        }
        else
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
        {
            downTimer += Time.deltaTime;

            if (downTimer > downTime)
            {
                downTimer = 0;
                isDown = false;
                Rising();
            }
        }
        else if (isDie)
        {
            respawnTimer += Time.deltaTime;

            if (respawnTimer > respawnTime)
            {
                respawnTimer = 0;
                isDie = false;

                Respawn();
            }
        }
        else if(isRunover)
        {
            runoverTimer += Time.deltaTime;

			//충돌된 방향으로 날아가기
			//transform.Translate(runoverVector);
			transform.position += runoverVector;
			//Debug.DrawRay(transform.position, runoverVector, Color.black);

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
		else if (isJump)
			LandCheck();
    }
    public abstract void Respawn();
    public void Runover(float runoverSpeed, Vector3 carPosition)
    {
		Vector3 runoverVector = transform.position - carPosition;
        if (runoverSpeed < 50)
        {
            return;
        }
        //보정수치
        this.runoverSpeed = Mathf.Clamp((runoverSpeed / 3000.0f), 0, 0.3f);
        
        
		this.runoverVector = (runoverVector.normalized * this.runoverSpeed * Mathf.Abs(Vector3.Dot(runoverVector, Vector3.right)));
		print(this.runoverVector.magnitude);
		//if(this.runoverVector.magnitude )

		isRunover = true;
		hDir = 0;
		vDir = 0;
		transform.LookAt(carPosition);
		DebugX.Log("차에치임");

		//보정 수치
		if (runoverSpeed > 200)
		{
			if(isDown)
			{
				Hurt((int)(runoverSpeed));
			}
			else
			{
				Hurt((int)(runoverSpeed / 4));
			}
			
		}
			
    }
    protected void UpdateTargetRotation()
    {
        targetDirectionVector = new Vector3(hDir, 0, vDir).normalized;
    }
    protected void UpdateSlerpedRotation()
    {
        if (0 != hDir || 0 != vDir)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetDirectionVector), 0.4f);
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
	protected virtual void Land()
	{
		isJump = false;
		transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
		GetComponent<Rigidbody>().useGravity = true;
	}

	public bool JumpTimerCheck()
	{
		jumpTimer += Time.deltaTime;
		DebugX.DrawRay(transform.position, transform.up * -1, Color.black);
		
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
		{
			jumpTimer = 0.0f;
			return true;
		}
		else
			return false;
	}
	protected virtual void LandCheck()
	{
		Debug.DrawRay(transform.position, transform.up * -1, Color.red);

		if (JumpTimerCheck())
		{
			if (!IsCarExistBelow())
				Land();
		}
	}
	public bool IsCarExistBelow()
	{
		DebugX.DrawRay(transform.position, transform.up * -1, Color.black);

		if (Physics.Raycast(transform.position, transform.up * -1, out hit, 1f, collisionLayer)
			&& hit.transform.CompareTag("Car"))
			return true;
		else
			return false;
	}
}