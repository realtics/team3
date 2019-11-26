using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

public class CarPathManager : MonoBehaviour
{
    public CarAi carAi;

    WaypointForCar curWaypoint;
    WaypointForCar destWaypoint;
    WaypointForCar lastWaypoint;

    Vector3 curDestPos;
    public int curLane = 0;

    void Start()
    {
        Init();
    }

    void OnEnable()
    {
        Init();
    }

    void Update()
    {
        Move();
    }

    public void Init()
    {
        GameObject go = WaypointManager.instance.FindClosestWaypoint(WaypointManager.WaypointType.car, transform.position);
        curWaypoint = go.GetComponent<WaypointForCar>();

        curLane = Random.Range(0, 3);
        SetRandomDestWaypoint();

        transform.position = curWaypoint.carRoadDict[destWaypoint].laneStartPosition[curLane];
        transform.LookAt(new Vector3(curDestPos.x, transform.position.y, curDestPos.z));

        carAi.SetDestination(curDestPos);
    }

    void SetRandomDestWaypoint()
    {
        while (true)
        {
            destWaypoint = curWaypoint.next[Random.Range(0, curWaypoint.next.Count)] as WaypointForCar;
            if (curWaypoint.next.Count == 1 || destWaypoint != lastWaypoint)
                break;
        }
        lastWaypoint = curWaypoint;

        int laneMax = curWaypoint.carRoadDict[destWaypoint].laneEndPosition.Count - 1;

        if (curWaypoint.canChangeLane)
        {
            curLane = Random.Range(0, laneMax + 1);
        }

        if (curLane > laneMax)
        {
            curLane = laneMax;
        }

        curDestPos = curWaypoint.carRoadDict[destWaypoint].laneEndPosition[curLane];
    }

    void Move()
    {
        Vector3 dir = (curDestPos - transform.position);
        dir.y = 0;
        float dist = dir.magnitude;

        if (dist < 0.5f)
        {
            curWaypoint = destWaypoint;
            SetRandomDestWaypoint();
            carAi.SetDestination(curDestPos);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (destWaypoint != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(destWaypoint.transform.position, 0.5f);
            //Handles.Label(destWaypoint.transform.position, "destWP");
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(curDestPos, 0.25f);
        //Handles.Label(curDestPos, "curDest");
    }
}
