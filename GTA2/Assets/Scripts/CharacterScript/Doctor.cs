using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doctor : NPC
{
    // Start is called before the first frame update
    public enum DOCTORSTATE
    {
        IDLE,
        WALK,
        RUN,
        DIE,
        DOWN,
        HEAL
    }
    public DOCTORSTATE doctorState;
    public Animator anim;

    void Start()
    {
        base.NPCInit();
    }

    // Update is called once per frame
    void Update()
    {
        //죽으면 대기
        if (doctorState == DOCTORSTATE.DIE)
            return;
        base.NPCUpdate();
        anim.SetInteger("DoctorState", (int)doctorState);
        TimerCheck();
        ActivityByState();
    }

    protected override void Die()
    {
        doctorState = DOCTORSTATE.DIE;
    }
    void ActivityByState()
    {
        switch (doctorState)
        {
            case DOCTORSTATE.IDLE:
                break;
            case DOCTORSTATE.WALK:
                break;
            case DOCTORSTATE.RUN:
                break;
            case DOCTORSTATE.DIE:
                break;
            case DOCTORSTATE.HEAL:
                break;
            default:
                break;
        }
    }
    void TimerCheck()
    {
        /*PatternChangeTimer += Time.deltaTime;

        if (DectectedPlayerAttack())
        {
            SetRunAway();
        }
        PatternChange(patternChangeInterval);
        */
    }
    void SetRunAway()
    {
        if (doctorState == DOCTORSTATE.RUN)
            return;
        targetDirectionVector = player.transform.position;
        UpdateTargetDirection();
        doctorState = DOCTORSTATE.RUN;
        //patternChangeTimer = 0.0f;
    }

    public override void Down()
    {
        throw new System.NotImplementedException();
    }

    public override void Rising()
    {
        throw new System.NotImplementedException();
    }

    public override void Respawn()
    {
        throw new System.NotImplementedException();
    }
}
