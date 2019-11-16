using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public int goalMoney;
    public GameObject goalObject;
    public GameObject missionArrow;
    public float arrowToPlayer;
    public float maxArrowToPlayer;


    UnityEngine.Rendering.CompareFunction comparison = UnityEngine.Rendering.CompareFunction.Always;
    Player userPlayer;
    bool isMissionSuccess = false;
    // Update is called once per frame

    void Start()
    {
        userPlayer = GameObject.FindWithTag("Player").GetComponent<Player>();


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
        if (userPlayer.money >= goalMoney)
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


        Vector3 playerToGoal = goalObject.transform.position - userPlayer.gameObject.transform.position;
        float playerToGoalDistance = playerToGoal.magnitude;
        playerToGoal.Normalize();




        if (playerToGoalDistance > maxArrowToPlayer)
        {
            missionArrow.transform.position = 
                userPlayer.gameObject.transform.position + playerToGoal * arrowToPlayer;
        }
        else
        {
            missionArrow.transform.position = Vector3.Lerp(
                missionArrow.transform.position,
                userPlayer.gameObject.transform.position + playerToGoal * playerToGoalDistance,
                .3f);
        }




        missionArrow.transform.LookAt(goalObject.transform.transform, Vector3.up);
        Vector3 tempVector = missionArrow.transform.eulerAngles;
        missionArrow.transform.eulerAngles = new Vector3(90.0f, tempVector.y, .0f);
    }
}
