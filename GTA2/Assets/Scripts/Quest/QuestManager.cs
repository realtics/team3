using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoSingleton<QuestManager>
{
    // Start is called before the first frame update
    Quest[] deactiveQuestList;
    List<Quest> activeQuestList;

    void Awake()
    {
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

    // Update is called once per frame
    void Update()
    {
        UpdateCheck();   
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
}
