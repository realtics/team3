using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WorldUIManager : MonoSingleton<WorldUIManager>
{
    [SerializeField]
    GameObject scoreTextPref;
    [SerializeField]
    GameObject questArrowPref;
    [SerializeField]
    int arrowPoolCount;
    [SerializeField]
    int scorePoolCount;



    public GameObject mainMissionArrowPref;
    public float arrowToPlayer;
    public float maxArrowToPlayer;


    bool isMainMissionSuccess;
    Vector3 goalPos;
    Player userPlayer;

    UnityEngine.Rendering.CompareFunction comparison = UnityEngine.Rendering.CompareFunction.Always;



    // Start is called before the first frame update
    void Awake()
    {
        InitPref();
        InitMainArrow();

        isMainMissionSuccess = false;
        userPlayer = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    void InitPref()
    {
        Image image = questArrowPref.GetComponent<Image>();
        Material existingGlobalMat = image.materialForRendering;
        Material updatedMaterial = new Material(existingGlobalMat);
        updatedMaterial.SetInt("unity_GUIZTestMode", (int)comparison);
        image.material = updatedMaterial;

        PoolManager.WarmPool(questArrowPref, arrowPoolCount);
        PoolManager.WarmPool(scoreTextPref, scorePoolCount);
    }

    void InitMainArrow()
    {
        goalPos = GameManager.Instance.goalObject.transform.position;

        Image image = mainMissionArrowPref.GetComponent<Image>();
        Material existingGlobalMat = image.materialForRendering;
        Material updatedMaterial = new Material(existingGlobalMat);
        updatedMaterial.SetInt("unity_GUIZTestMode", (int)comparison);
        image.material = updatedMaterial;
    }

    void Update()
    {
        if (isMainMissionSuccess)
        {
            UpdateArrow(mainMissionArrowPref, goalPos);
        }
    }
    // Update is called once per frame
   
    public void SetScoreText(Vector3 targetPos, int scoreValue)
    {
        FloatingScoreText floatingScoreText = 
            PoolManager.SpawnObject(scoreTextPref).GetComponent<FloatingScoreText>();

        floatingScoreText.FloatingText(targetPos, scoreValue);
    }

    public void SuccessMainMission()
    {
        mainMissionArrowPref.SetActive(true);
        isMainMissionSuccess = true;
    }

    public GameObject SpwanArrow()
    {
        return PoolManager.SpawnObject(questArrowPref);
    }
    public void despwanArrow(GameObject arrow)
    {
        PoolManager.ReleaseObject(arrow);
    }


    public void UpdateArrow(GameObject arrow, Vector3 targetPos)
    {
        Vector3 playerToGoal = targetPos - userPlayer.gameObject.transform.position;
        float playerToGoalDistance = playerToGoal.magnitude;
        playerToGoal.Normalize();

        if (playerToGoalDistance > maxArrowToPlayer)
        {
            arrow.transform.position = 
                userPlayer.gameObject.transform.position + playerToGoal * arrowToPlayer;
        }
        else
        {
            arrow.transform.position = Vector3.Lerp(
                arrow.transform.position - playerToGoal * .25f,
                userPlayer.gameObject.transform.position + playerToGoal * playerToGoalDistance,
                .3f);
        }

        arrow.transform.LookAt(targetPos, Vector3.up);
        Vector3 tempVector = arrow.transform.eulerAngles;
        arrow.transform.eulerAngles = new Vector3(90.0f, tempVector.y, .0f);
    }
}
