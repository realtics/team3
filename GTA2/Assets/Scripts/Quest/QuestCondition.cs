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