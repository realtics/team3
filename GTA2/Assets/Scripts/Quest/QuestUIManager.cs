using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class QuestUIManager : MonoSingleton<QuestUIManager>
{
    [SerializeField]
    Text title;
    [SerializeField]
    Text info;

    float toastTime = 5.0f;
    float toastDel;


    // Start is called before the first frame update
    void Awake()
    {
        toastDel = .0f;
        ResetVar();
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

    // Update is called once per frame
    void Update()
    {
        toastDel += Time.deltaTime;
        if (toastTime < toastDel)
        {
            ResetVar();
        }
        else if (toastDel > 10000000000.0f)
        {
            ResetVar();
        }
        else
        {
            title.gameObject.SetActive(true);
            info.gameObject.SetActive(true);
        }
    }

    void ResetVar()
    {
        title.text = "";
        info.text = "";
        toastDel = .0f;
    }
}
