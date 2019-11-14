using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Police : NPC
{
    public enum POLICESTATE
    {
        IDLE,
        WALK,
        RUN,
        ATTACK,
        DIE,
        DOWN,
        SHOT,
    }
    private float patternChangeTimer = 3.0f;
    private float patternChangeInterval = 3.0f;
    private float attackInterval = 1.0f;
    private float punchRange = 2.0f;
    private float curAttackCoolTime = 0.0f;
    
    public POLICESTATE policeState;
    public Animator anim;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        curAttackCoolTime = attackInterval;
    }

    // Start is called before the first frame update
    private void Update()
    {
        if (policeState == POLICESTATE.DIE)
            return;
        base.NPCUpdate();
        anim.SetInteger("PoliceState", (int)policeState);
        TimerCheck();
        ActivityByState();
        if (DectectedPlayerAttack())
        {
            policeState = POLICESTATE.RUN;
        }
    }
    void ActivityByState()
    {
        switch (policeState)
        {
            case POLICESTATE.IDLE:
                break;
            case POLICESTATE.WALK:
                Move();
                break;
            case POLICESTATE.RUN:
                ChasePlayer();
                if (IsAttackRange())
                {
                    policeState = POLICESTATE.ATTACK;
                }
                break;
            case POLICESTATE.ATTACK:
                if (!IsAttackRange())
                {
                    policeState = POLICESTATE.RUN;
                }
                if (curAttackCoolTime > attackInterval)
                {
                    curAttackCoolTime = 0;
                    Attack();
                }
                break;
            case POLICESTATE.SHOT:
                break;
            case POLICESTATE.DIE:
                break;
            default:
                break;
        }

    }
    
    void TimerCheck()
    {
        if (policeState == POLICESTATE.ATTACK || policeState == POLICESTATE.RUN)
        {
            curAttackCoolTime += Time.deltaTime;
        }
        else
        {
            patternChangeTimer += Time.deltaTime;
        }
        if (patternChangeTimer > patternChangeInterval)
        {
            PatternChange();
        }
    }
    void PatternChange()
    {
        patternChangeTimer = 0.0f;

        switch (policeState)
        {
            case POLICESTATE.IDLE:
                UpdateTargetDirection();
                policeState = POLICESTATE.WALK;
                break;
            case POLICESTATE.WALK:
                policeState = POLICESTATE.IDLE;
                break;
        }
    }
    bool IsAttackRange()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < punchRange)
        {
            return true;
        }
        return false;
    }
    
    void Attack()
    {
        //transform.LookAt(player.transform);
        player.GetComponent<Player>().Hurt(10);
    }
    protected override void Die()
    {
        policeState = POLICESTATE.DIE;
    }
}
