using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestStatus
{
    Kill,
    Arrive,
    CarryCargo,
}


public abstract class Quest : MonoBehaviour
{
    [SerializeField]
    protected string name;
    [SerializeField]
    protected string infoPath;
    [SerializeField]
    protected string endPath;


    [SerializeField]
    protected Phone startPhone;
    protected QuestStatus questStatus;
    protected bool isCorrect;

    protected float correctAndOffTime = 5.0f;
    protected float correctAndOffDel = .0f;

    public abstract void StartQuest();
    public abstract bool CheckCondition();
    public abstract void PushReward();
}