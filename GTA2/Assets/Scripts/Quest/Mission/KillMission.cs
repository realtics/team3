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
        else
        {
            if (questArrow == null)
            {
                return;
            }

            WorldUIManager.Instance.UpdateArrow(questArrow, killTarget.transform.position);
        }
    }


    public override void StartQuest()
    {
        killTarget.gameObject.SetActive(true);
        QuestManager.Instance.StartQuest(this);
        QuestUIManager.Instance.ToastStartQuest(title, infoPath);


        questArrow = WorldUIManager.Instance.SpwanArrow();
        questArrow.transform.parent = WorldUIManager.Instance.transform;
    }

    public override bool CheckCondition()
    {
        if (killTarget.isDie)
        {
            WorldUIManager.Instance.despwanArrow(questArrow);
            return true;
        }
        return false;
    }

    public override void PushReward()
    {
        QuestUIManager.Instance.ToastEndQuest(endPath);
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




