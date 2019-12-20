using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestNPC : Citizen
{
    // Start is called before the first frame update
    private void OnEnable()
    {
        rigidbody = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        //patternChangeTimer = patternChangeTime;
        AnimationInit();
        SetDefaultHp();
        StartCoroutine(ActivityByState());
    }

    IEnumerator ActivityByState()
    {
        while (true)
        {
            if (isDie || isDown)
                yield break;

            else if (isWalk)
            {
                base.Raycast();
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

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
}
