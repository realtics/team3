using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillMission : QuestCondition
{
    public NPC killTarget;

    void Start()
    {
        questStatus = QuestStatus.Kill;
    }

    public override bool CheckCondition()
    {
        if (killTarget.isDie)
        {
            return true;
        }
        return false;
    }
}




