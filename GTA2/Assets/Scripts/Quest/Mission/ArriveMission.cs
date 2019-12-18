using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 누굴 죽이는가
// 어디에 도착하는가.
public class ArriveMission : Quest
{
    public GameObject arrivePosTarget;
    void Start()
    {
        questType = QuestType.Arrive;
    }


    public override void DeleteQuest()
    {
    }


    public override void StartQuest()
    {

    }
    public override bool CheckCondition()
    {



        return true;
    }

    public override void PushReward()
    {
        
    }
}