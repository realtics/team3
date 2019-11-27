using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    public int goalMoney;
    public GameObject goalObject;
    public GameObject missionArrow;
    public float arrowToPlayer;
    public float maxArrowToPlayer;
    //int defaultRemains = 3;
    public int remains = 3;

    UnityEngine.Rendering.CompareFunction comparison = UnityEngine.Rendering.CompareFunction.Always;
    [HideInInspector] public Player player;
    public List<Transform> playerRespawnPoint;

    bool isMissionSuccess = false;
    //GameManager ReFactoring field
    public int money{ get; set; }

    public float wantedLevel = 0; // 수배레벨
    
    void Start()
    {
        // 요부분 리펙토링
        Application.targetFrameRate = 60;
        //Screen.SetResolution(720, 1280, true);

        player = GameObject.FindWithTag("Player").GetComponent<Player>();

        Image image = missionArrow.GetComponent<Image>();
        Material existingGlobalMat = image.materialForRendering;
        Material updatedMaterial = new Material(existingGlobalMat);
        updatedMaterial.SetInt("unity_GUIZTestMode", (int)comparison);
        image.material = updatedMaterial;
    }
    void Update()
    {
        UpdateGoal();
        UpdateArrow();
    }

    void UpdateGoal()
    {
        if (money >= goalMoney)
        {
            isMissionSuccess = true;
        }
        else
        {
            isMissionSuccess = false;
        }
    }
    void UpdateArrow()
    {
        if (!isMissionSuccess)
        {
            goalObject.SetActive(false);
            missionArrow.SetActive(false);
            return;
        }

        goalObject.SetActive(true);
        missionArrow.SetActive(true);

        Vector3 playerToGoal = goalObject.transform.position - player.gameObject.transform.position;
        float playerToGoalDistance = playerToGoal.magnitude;
        playerToGoal.Normalize();

        if (playerToGoalDistance > maxArrowToPlayer)
        {
            missionArrow.transform.position = 
                player.gameObject.transform.position + playerToGoal * arrowToPlayer;
        }
        else
        {
            missionArrow.transform.position = Vector3.Lerp(
                missionArrow.transform.position,
                player.gameObject.transform.position + playerToGoal * playerToGoalDistance,
                .3f);
        }

        missionArrow.transform.LookAt(goalObject.transform.transform, Vector3.up);
        Vector3 tempVector = missionArrow.transform.eulerAngles;
        missionArrow.transform.eulerAngles = new Vector3(90.0f, tempVector.y, .0f);
    }

    public void RespawnSetting()
    {
        if (remains == 0)//RIP
        {
            SceneManager.LoadScene("Rip");
            return;
        }
        if (player.isBusted)
        {
            CarSpawnManager.Instance.policeCarList[0].gameObject.SetActive(true);
            CarSpawnManager.Instance.policeCarList[0].passengerManager.passengers[1] = player;
            CarSpawnManager.Instance.policeCarList[0].GetComponent<CarPassengerManager>().GetOnTheCar(player, 1);
            player.transform.position = CarSpawnManager.Instance.policeCarList[0].passengerManager.doorPositions[1].position;

            for(int i = 1; i < player.gunList.Count; i++)
                player.gunList[1].bulletCount = 0;
            player.curGunIndex = GunState.None;
            StartCoroutine(CarSpawnManager.Instance.policeCarList[0].passengerManager.GetOffTheCar(1));
            //player.Down();
            //Invoke(player.Down(), 1.0f);
        }
        else
        {
            player.transform.position = playerRespawnPoint[Random.Range(0, playerRespawnPoint.Count)].position;
        }
        CarSpawnManager.Instance.CarPositionInit();
        NPCSpawnManager.Instance.NPCPositionInit();
        //카메라 위치 조정
        CameraController.Instance.ChangeTarget(player.gameObject);
        
    }
    public void IncreaseMoney(int earnings)
    {
        money += earnings;
    }

    // [ 수배레벨 ]
    public void IncreaseWantedLevel(float amount)
    {
        int old = (int)wantedLevel;

        if (old > 0)
            amount /= (old*2);

        wantedLevel += amount;

        if(old != (int)wantedLevel)
        {
            // 수배레벨이 증가함.
            UIManager.Instance.SetPoliceLevel((int)wantedLevel);
        }
    }

    public void ResetWantedLevel()
    {
        wantedLevel = 0;
        UIManager.Instance.SetPoliceLevel(0);
    }
}
