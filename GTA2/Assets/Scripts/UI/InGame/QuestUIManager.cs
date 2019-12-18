using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class QuestUIManager : MonoSingleton<QuestUIManager>
{
    [SerializeField]
    Text title;
    [SerializeField]
    Text info;
    [SerializeField]
    GameObject killFrenzyBoard;
    [SerializeField]
    Text frenzyKillCount;
    [SerializeField]
    Text frenzyMinute;
    [SerializeField]
    Text frenzySecond;
    [SerializeField]
    GameObject frenzyAlarm;

    float toastTime = 3.0f;
    float toastDel;


    // Start is called before the first frame update
    void Awake()
    {
        toastDel = .0f;
        ResetVar();

        ToastStartQuest("START!!", "");
        killFrenzyBoard.SetActive(false);
    }
    public void ToastTitle(string title)
    {
        toastDel = .0f;
        this.title.text = title;
        this.info.text = "";
    }
    public void ToastStartQuest(string title, string info)
    {
        toastDel = .0f;
        this.title.text = title;
        this.info.text = info;
    }
    public void ToastEndQuest(string endtext)
    {
        toastDel = .0f;
        this.title.text = endtext;
    }

    public void SetKillFrenzy()
    {
        frenzyAlarm.SetActive(false);
        killFrenzyBoard.SetActive(true);
        ToastStartQuest("KILL FRENZY!!", "");
    }
    public void OutKillFrenzy()
    {
        killFrenzyBoard.SetActive(false);
        frenzyAlarm.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        toastDel += Time.deltaTime;
        if (toastTime < toastDel)
        {
            ResetVar();
        }
        else
        {
            title.gameObject.SetActive(true);
            info.gameObject.SetActive(true);
        }
    }

    public void UpdateFrenzy(int goalKill, float maxTime)
    {
        frenzyKillCount.text = goalKill.ToString();

        var ts = TimeSpan.FromSeconds(maxTime);

        frenzyMinute.text = string.Format("{0:00}", ts.Minutes);
        frenzySecond.text = string.Format("{0:00}", ts.Seconds);

        if (maxTime < 10.0f)
        {
            frenzyAlarm.SetActive(true);
        }
    }

    void ResetVar()
    {
        title.text = "";
        info.text = "";
        toastDel = .0f;
    }
}
