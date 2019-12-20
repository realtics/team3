using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum QuestStatus
{
    Ready,
    Complete,
    Perform,
    Failed,
    GiveUp,
    End,
}

public enum QuestType
{
    Kill,
}


public abstract class Quest : MonoBehaviour
{
    [Header("Quest Information")]
    [SerializeField]
    protected string title;
    [SerializeField]
    protected string infoPath;
    [SerializeField]
    protected string endPath;


    [Header("Quest Object")]
    [SerializeField]
    protected Phone startPhone;
    [SerializeField]
    protected GameObject phoneArrowPref;

    [Header("Quest Sound")]
    [SerializeField]
    protected AudioClip startClip;
    [SerializeField]
    protected AudioClip completeClip;

    protected QuestStatus questStatus;
    protected QuestType questType;
    protected GameObject questArrow;
    protected GameObject phoneArrow;

    protected float correctAndOffTime = 5.0f;
    protected float correctAndOffDel = .0f;

    public abstract void DeleteQuest();
    public abstract void StartQuest();
    public abstract bool CheckCondition();
    public abstract void PushReward();
}