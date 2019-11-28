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
    public bool isDriver;
    public abstract void Down();
    public abstract void Rising();
    protected abstract void Die();
    protected float downTimer = 0.0f;
    protected float downTime = 3.0f;
    float respawnTimer = 0.0f;
    float respawnTime = 5.0f;
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
    }
    public abstract void Respawn();
    protected void RunOver()
    {

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
   
}
