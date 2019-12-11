using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillMission : Quest
{
    [SerializeField]
    NPC killTarget;
    [SerializeField]
    int rewardMoney;

    void Awake()
    {
        isCorrect = false;
        questStatus = QuestStatus.Kill;
        killTarget.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isCorrect)
        {
            correctAndOffDel += Time.deltaTime;
            if (correctAndOffTime < correctAndOffDel)
            {
                gameObject.SetActive(false);
            }
        }    
    }


    public override void StartQuest()
    {
        killTarget.gameObject.SetActive(true);
        QuestManager.Instance.StartQuest(this);
    }

    public override bool CheckCondition()
    {
        if (killTarget.isDie)
        {
            return true;
        }
        return false;
    }

    public override void PushReward()
    {
        GameManager.Instance.IncreaseMoney(rewardMoney);
        isCorrect = true;
    }

    void OnDrawGizmos()
    {
        if (killTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(startPhone.transform.position, killTarget.transform.position);
        }
    }
}




