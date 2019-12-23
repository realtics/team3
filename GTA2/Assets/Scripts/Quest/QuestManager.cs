using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoSingleton<QuestManager>
{
    [Header("Sound")]
    [SerializeField]
    AudioClip frenzyFail;
    [SerializeField]
    AudioClip frenzyPassed;

    // Start is called before the first frame update
    Quest[] deactiveQuestList;
    List<Quest> activeQuestList;
	
    GunState frenzyType;
    int startKillCount;
    int goalKill;
    float frenzyDelta;
    float frenzyMaxTime;


    void Awake()
    {
        frenzyType = GunState.None;
        activeQuestList = new List<Quest>();
        deactiveQuestList = GetComponentsInChildren<Quest>();
    }


    public void StartQuest(Quest quest)
    {
        foreach (var item in deactiveQuestList)
        {
            if (item == quest)
            {
                activeQuestList.Add(item);
                break;
            }
        }
    }

    public void StartKillFrenzy(GunState gunType, int killCount, float maxTime)
    {
        QuestUIManager.Instance.SetKillFrenzy();

        startKillCount = GameManager.Instance.killCount;
        frenzyType = gunType;
        goalKill = killCount;
        frenzyMaxTime = maxTime;
        frenzyDelta = .0f;
    }

    public void ResetQuest()
    {
        foreach (var item in activeQuestList)
        {
            item.DeleteQuest();
        }
        activeQuestList.Clear();

        frenzyType = GunState.None;
        QuestUIManager.Instance.OutKillFrenzy();
    }


    // Update is called once per frame
    void Update()
    {
        UpdateCheck();
        UpdateFrenzy();
    }

    void UpdateCheck()
    {
        foreach (var item in activeQuestList)
        {
            if(item.CheckCondition())
            {
                item.PushReward();
                activeQuestList.Remove(item);
                break;
            }
        }
    }

    void UpdateFrenzy()
    {
        if (frenzyType != GunState.None)
        {
            frenzyDelta += Time.deltaTime;

            int killCount = GameManager.Instance.killCount - startKillCount;
            int curGoalKillCount = goalKill - killCount;
            QuestUIManager.Instance.UpdateFrenzy(curGoalKillCount, frenzyMaxTime - frenzyDelta);

            if (frenzyDelta > frenzyMaxTime)
            {
                frenzyType = GunState.None;
                QuestUIManager.Instance.OutKillFrenzy();
                QuestUIManager.Instance.ToastStartQuest("Frenzy Fail..", "");
                SoundManager.Instance.PlayClip(frenzyFail, SoundPlayMode.UISFX);
            }
            else if (curGoalKillCount <= 0)
            {
                frenzyType = GunState.None;
                QuestUIManager.Instance.OutKillFrenzy();
                QuestUIManager.Instance.ToastStartQuest("Frenzy Passed!!", "");
                SoundManager.Instance.PlayClip(frenzyPassed, SoundPlayMode.UISFX);
                WantedLevel.instance.ResetWantedLevel();
                GameManager.Instance.IncreaseMoney(50000);
            }
        }
    }
}
