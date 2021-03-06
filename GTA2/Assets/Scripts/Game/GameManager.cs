﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    [Header("Goal")]
    public int goalMoney;
    public GameObject goalObject;
    //int defaultRemains = 3;

    [Header("Life")]
    public int remains = 3;

    [Header("Scene")]
    public string nextScene;

    [Header("Player")]
    public Player player;
    public List<Transform> playerRespawnPoint;
	public CarPassengerManager playerCar;
	
    
    public int money{ get; set; }
    public int killCount { get; set; }
    double gameTime;


	public int spawnedAmbulanceNum { get; set; } = 0;
	public NPC ambulanceTargetNPC;

    void Awake()
    {
        gameTime = .0f;
        killCount = 0;

        // 요부분 리펙토링
        Application.targetFrameRate = 60;
        //Screen.SetResolution(720, 1280, true);

        goalObject.SetActive(false);
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
		StartCoroutine(CheckEmbulanceCall());

        LoadData();
    }

    void LoadData()
    {
        JsonStreamer js = gameObject.AddComponent<JsonStreamer>();
        Gta2Data gta2GetData = js.Load<Gta2Data>("Gta2Data.json");

        if (gta2GetData != null &&
            SceneManager.GetActiveScene().name != "Stage1" &&
            gta2GetData.life > 0)
        {
            remains = gta2GetData.life;
        }
    }

    void Update()
    {
        UpdateGoal();
        UpdateTime();
    }

    void UpdateGoal()
    {
        if (money >= goalMoney)
        {
            goalObject.SetActive(true);
            WorldUIManager.Instance.SuccessMainMission();
        }
    }
    void UpdateTime()
    {
        gameTime += Time.deltaTime;
    }

	public void RespawnSetting()
	{
		if (remains <= 0)//RIP
		{
			SaveGta2Data();
			SceneManager.LoadScene("Rip");
			return;
		}

        

        if (player.isBusted)
		{
			StartCoroutine(GetOffFromCopCarCor());
			UIManager.Instance.CarUIMode(playerCar.carManager);
			CarManager copCar = CarSpawnManager.Instance.SpawnPoliceCar(WaypointManager.instance.FindRandomWaypointOutOfCameraView(WaypointManager.WaypointType.car).transform.position);

			player.ResetAllGunBullet();
			player.curGunIndex = GunState.None;
		}
		else
		{
			player.transform.position = playerRespawnPoint[Random.Range(0, playerRespawnPoint.Count)].position;
			player.curGunIndex = GunState.None;
			CameraController.Instance.ChangeTarget(player.gameObject);
		}

		//카메라 위치 조정
		CameraController.Instance.ZoomOut();
		WantedLevel.instance.ResetWantedLevel();
        player.RespawnGunList();
	}

	IEnumerator GetOffFromCopCarCor()
	{
		CarManager copCar = CarSpawnManager.Instance.SpawnPoliceCar(WaypointManager.instance.FindRandomWaypointOutOfCameraView(WaypointManager.WaypointType.car).transform.position);
		CameraController.Instance.ChangeTarget(copCar.gameObject);
		player.playerPhysics.targetCar = copCar.passengerManager;

		copCar.gameObject.SetActive(false);
		copCar.gameObject.SetActive(true);
        player.playerPhysics.targetCar.GetOnTheCar(People.PeopleType.Player, 2);
        yield return new WaitForSeconds(3.0f);
		copCar.passengerManager.GetOffTheCar(2);
		player.Down();
		player.isBusted = false;
        CameraController.Instance.ChangeTarget(player.gameObject);
	}

    public void IncreaseMoney(int earnings)
    {
        money += earnings;
    }

    public void CompliteGame()
    {
        SaveGta2Data();
        SceneManager.LoadScene("End");
    }

    void SaveGta2Data()
    {
        Gta2Data myObject = new Gta2Data();
        myObject.life = remains;
        myObject.money = money;
        myObject.gameTime = gameTime;
        myObject.kills = killCount;
        myObject.curScene = SceneManager.GetActiveScene().name;
        myObject.nextScene = nextScene;

        JsonStreamer js = new JsonStreamer();
        js.Save(myObject, "Gta2Data.json");
    }
	IEnumerator CheckEmbulanceCall()
	{
		while(true)
		{
			foreach(GameObject npc in NPCSpawnManager.Instance.allNPC)
			{
				if(npc.GetComponent<NPC>().isDie)
				{
					ambulanceTargetNPC = npc.GetComponent<NPC>();
					break;
				}
			}
			if (NPCSpawnManager.Instance.DiedNPC.Count > 1 &&
				spawnedAmbulanceNum < 3)
			{
				spawnedAmbulanceNum++;
				CarSpawnManager.Instance.SpawnAmbulanceCar(WaypointManager.instance.FindRandomCarSpawnPosition().transform.position);
			}
			
			yield return new WaitForSeconds(15.0f);
		}
	}
}
