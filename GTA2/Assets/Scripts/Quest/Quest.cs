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
    protected string title;
    [SerializeField]
    protected string infoPath;
    [SerializeField]
    protected string endPath;


    [SerializeField]
    protected Phone startPhone;
    [SerializeField]
    protected AudioClip startClip;
    [SerializeField]
    protected AudioClip completeClip;
    [SerializeField]
    protected GameObject phoneArrowPref;

    protected QuestStatus questStatus;
    protected bool isCorrect;
    protected GameObject questArrow;
    protected GameObject phoneArrow;

    protected float correctAndOffTime = 5.0f;
    protected float correctAndOffDel = .0f;

    public abstract void StartQuest();
    public abstract bool CheckCondition();
    public abstract void PushReward();
}