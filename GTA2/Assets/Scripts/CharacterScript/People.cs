﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class People : MonoBehaviour
{
    protected float rotateSpeed = 0.1f;
    protected float moveSpeed = 1.0f;
    protected float runSpeed = 2.0f;

    protected Vector3 movement;
    protected Vector3 direction;
    protected Vector3 targetDirectionVector = Vector3.zero;
    
    [SerializeField]
    protected int hp = 100;
    protected int hDir = 0;
    protected int vDir = 0;

    //TODO : isDie없이 작동할 수 있도록 수정
    protected bool isDie = false;
    protected abstract void Die();
    protected virtual void Move()
    {
        Vector3 Pos = transform.position;

        Pos.x += transform.forward.x * Time.deltaTime * moveSpeed;
        Pos.z += transform.forward.z * Time.deltaTime * moveSpeed;

        transform.position = Pos;
    }
    
    public void Hurt(int damage)
    {
        hp -= damage;

        //사망시 true
        if (hp <= 0)
        {
            isDie = true;
            Die();
        }
    }
    
    
    //Fixed Update에서 반드시 호출
    protected void UpdateTargetRotation()
    {
        targetDirectionVector = Vector3.zero;

        if (0 != hDir && 0 != vDir)
        {
            if (0 < hDir)
            {
                targetDirectionVector = new Vector3(0, (hDir * 90.0f) - (vDir * 45.0f), 0);
            }
            else if (0 > hDir)
            {
                targetDirectionVector = new Vector3(0, (hDir * 90.0f) + (vDir * 45.0f), 0);
            }
        }

        else if (0 != hDir)
        {
            targetDirectionVector = new Vector3(0, (hDir * 90.0f), 0);
        }

        else if (0 != vDir)
        {
            targetDirectionVector = new Vector3(0, (vDir * 90.0f - 90.0f), 0);
        }
    }
    protected void UpdateSlerpedRotation()
    {
        Quaternion rotation1 = Quaternion.Euler(transform.eulerAngles);
        Quaternion rotation2 = Quaternion.Euler(targetDirectionVector);

        if (0 != hDir || 0 != vDir)
        {
            transform.rotation = Quaternion.Slerp(rotation1, rotation2, rotateSpeed);
        }
    }
   
}
