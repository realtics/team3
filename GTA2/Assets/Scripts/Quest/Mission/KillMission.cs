using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class KillMission : Quest
{
    [SerializeField]
    NPC killTarget;
    [SerializeField]
    int rewardMoney;


    void Awake()
    {
        questStatus = QuestStatus.Ready;
        questType = QuestType.Kill;
        killTarget.gameObject.SetActive(false);

        phoneArrow = Instantiate(phoneArrowPref);
        phoneArrow.transform.parent = WorldUIManager.Instance.transform;
        WorldUIManager.Instance.SetZUPMode(phoneArrow);
    }

    void Update()
    {
        switch (questStatus)
        {
            case QuestStatus.Ready:                
                WorldUIManager.Instance.UpdateArrow(phoneArrow, startPhone.transform.position);
                break;
            case QuestStatus.Complete:
                correctAndOffDel += Time.deltaTime;
                if (correctAndOffTime < correctAndOffDel)
                {
                    gameObject.SetActive(false);
                }
                break;
            case QuestStatus.Perform:
                WorldUIManager.Instance.UpdateArrow(questArrow, killTarget.transform.position);
                break;
            case QuestStatus.Failed:
                break;
            case QuestStatus.GiveUp:
                break;
            case QuestStatus.End:
            default:
                break;
        }
    }


    public override void DeleteQuest()
    {
        questStatus = QuestStatus.Failed;
        killTarget.gameObject.SetActive(false);
        phoneArrow.gameObject.SetActive(false);
        WorldUIManager.Instance.despwanArrow(questArrow);
    }
    public override void StartQuest()
    {
        questStatus = QuestStatus.Perform;
        killTarget.gameObject.SetActive(true);
        phoneArrow.gameObject.SetActive(false);
        QuestManager.Instance.StartQuest(this);
        QuestUIManager.Instance.ToastStartQuest(title, infoPath);


        SoundManager.Instance.PlayClip(startClip, SoundPlayMode.UISFX);
        questArrow = WorldUIManager.Instance.SpwanArrow();
        questArrow.transform.parent = WorldUIManager.Instance.transform;
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
        questStatus = QuestStatus.Complete;
        WorldUIManager.Instance.despwanArrow(questArrow);
        SoundManager.Instance.PlayClip(completeClip, SoundPlayMode.UISFX);
        WantedLevel.instance.ResetWantedLevel();
        QuestUIManager.Instance.ToastEndQuest(endPath);
        GameManager.Instance.IncreaseMoney(rewardMoney);
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




