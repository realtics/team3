using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestStatus
{
    Kill,
    Arrive,
    CarryCargo,
}


public abstract class QuestCondition : MonoBehaviour
{
    protected QuestStatus questStatus;

    public abstract bool CheckCondition();
}


// 누굴 죽이는가
public class KillMission : QuestCondition
{
    public NPC killTarget;
    public override bool CheckCondition()
    {
        if(killTarget.isDie)
        {
            return true;
        }

        return false;
    }
}

// 어디에 도착하는가.
public class ArriveMission : QuestCondition
{
    public GameObject arrivePosTarget;

    public override bool CheckCondition()
    {



        return true;
    }
}


// 어디에 무엇을 가져다 놓는가.
public class CarryCargoMission : QuestCondition
{
    public GameObject arrivePosTarget;
    public GameObject cargoTarget;

    public override bool CheckCondition()
    {
        return true;
    }
}