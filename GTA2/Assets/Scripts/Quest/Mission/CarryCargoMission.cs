using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// 어디에 무엇을 가져다 놓는가.
public class CarryCargoMission : Quest
{
    public GameObject arrivePosTarget;
    public GameObject cargoTarget;
    void Start()
    {
        questType = QuestType.CarryCargo;
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