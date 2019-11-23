using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class People : MonoBehaviour
{
    protected float rotateSpeed = 0.1f;
    protected float moveSpeed = 0.5f;
    protected float runSpeed = 1.2f;
    
    protected Vector3 movement;
    protected Vector3 direction;
    protected Vector3 targetDirectionVector = Vector3.zero;
    
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
    public abstract void Down();
    public abstract void Rising();
    protected abstract void Die();

    protected virtual void Move()
    {
        Vector3 Pos = transform.position;

        Pos.x += transform.forward.x * Time.deltaTime * moveSpeed;
        Pos.z += transform.forward.z * Time.deltaTime * moveSpeed;

        transform.position = Pos;
    }
    public virtual void Hurt(int damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            Die();
            isDie = true;
        }
    }
    public void Hurt(int damage, bool isBusted)
    {
        //Busted
        hp -= damage;
        if(hp <= 0)
        {
            isBusted = true;
            Die();
            isDie = true;
        }
    }

    //FIX ME : 현재 isBusted를 어떤 값을 보내도Busted로 동작
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
   
}
