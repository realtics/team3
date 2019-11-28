using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 누구를 몇명 - Kill
// 어디에 - Arrive
// 어디에 무엇을 - 상, 연계


// 퀘스트는... 
// 시작 위치, 이름, 달성 조건(누굴 몇 명 죽이기, 어디 도착하기, 무엇을 어디에 가져다 놓기), 달성 보
public class Quest : MonoBehaviour
{
    [SerializeField]
    string questName;

    [SerializeField]
    string questInfo;

    // Start is called before the first frame update
    [SerializeField]
    int questID;
    
    [SerializeField]
    Phone startPhone;

    [SerializeField]
    List<QuestCondition> conditionList;

    [SerializeField]
    int rewardMoney;

    public void AddKillCondtion()
    {
        KillMission killMission = new KillMission();
        conditionList.Add(killMission);
    }
    public void AddArriveCondtion()
    {
        ArriveMission arriveMission = new ArriveMission();
        conditionList.Add(arriveMission);
    }
    public void AddCarryCargoCondtion()
    {
        CarryCargoMission carryMission = new CarryCargoMission();
        conditionList.Add(carryMission);
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
