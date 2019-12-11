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
    public string nextScene;

    UnityEngine.Rendering.CompareFunction comparison = UnityEngine.Rendering.CompareFunction.Always;
    public Player player;
	
    public List<Transform> playerRespawnPoint;
    bool isMissionSuccess = false;
	//GameManager ReFactoring field
	public CarPassengerManager playerCar;
	public int money{ get; set; }
    public int killCount { get; set; }
    double gameTime;

    void Start()
    {
        gameTime = .0f;
        killCount = 0;

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
        UpdateTime();
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

    void UpdateTime()
    {
        gameTime += Time.deltaTime;
    }

	public void RespawnSetting()
	{
		NPCSpawnManager.Instance.NPCRespawn();
		if (remains == 0)//RIP
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
	}

	IEnumerator GetOffFromCopCarCor()
	{
		CarManager copCar = CarSpawnManager.Instance.SpawnPoliceCar(WaypointManager.instance.FindRandomWaypointOutOfCameraView(WaypointManager.WaypointType.car).transform.position);
		CameraController.Instance.ChangeTarget(copCar.gameObject);
		player.playerPhysics.targetCar = copCar.passengerManager;

		copCar.gameObject.SetActive(false);
		copCar.gameObject.SetActive(true);
		//copCar.passengerManager.GetOnTheCar(People.PeopleType.Player, 2, true);
        player.playerPhysics.targetCar.GetOnTheCar(People.PeopleType.Player, 2, true);
        yield return new WaitForSeconds(3.0f);
		StartCoroutine(copCar.passengerManager.GetOffTheCar(2));
		player.isBusted = false;
		yield return new WaitForSeconds(0.5f);
        player.Down();

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
        myObject.money = money;
        myObject.gameTime = gameTime;
        myObject.kills = killCount;
        myObject.curScene = SceneManager.GetActiveScene().name;
        myObject.nextScene = nextScene;

        JsonStreamer js = new JsonStreamer();
        js.Save(myObject, "Gta2Data.json");
    }
}
