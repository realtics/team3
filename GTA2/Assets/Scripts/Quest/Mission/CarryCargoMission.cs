using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// 어디에 무엇을 가져다 놓는가.
public class CarryCargoMission : QuestCondition
{
    public GameObject arrivePosTarget;
    public GameObject cargoTarget;
    void Start()
    {
        questStatus = QuestStatus.CarryCargo;
    }
    public override bool CheckCondition()
    {
        return true;
    }
}