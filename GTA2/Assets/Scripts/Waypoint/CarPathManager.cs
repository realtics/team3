using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CarPathManager : MonoBehaviour
{
    public CarCtr carCtr;

    WaypointForCar curWaypoint;
    WaypointForCar destWaypoint;
    WaypointForCar lastWaypoint;

    Vector3 curDestPos;
    public int curLane = 0;

    enum DestType
    {
        DT_EndOfRoad, DT_StartOfRoad
    }
    DestType destType = DestType.DT_EndOfRoad;

    void Start()
    {
        SetRandomDestWaypoint();

        destType = DestType.DT_EndOfRoad;
        curLane = Random.Range(0, curWaypoint.carRoadDict[destWaypoint].laneEndPosition.Count);

        carCtr.SetDestination(curDestPos);
    }

    void Update()
    {
        Move();
    }

    void SetRandomDestWaypoint()
    {
        curWaypoint = FindClosestWaypoint(transform.position);
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

        if (dist < 0.3f)
        {
            //if (destType == DestType.DT_EndOfRoad)
            //{
            //    SetRandomDestWaypoint();

            //    destType = DestType.DT_StartOfRoad;
            //}
            //else if (destType == DestType.DT_StartOfRoad)
            //{
            //    curDestPos = curWaypoint.carRoadDict[destWaypoint].laneEndPosition[curLane];

            //    destType = DestType.DT_EndOfRoad;
            //}


            SetRandomDestWaypoint();
            carCtr.SetDestination(curDestPos);
        }
    }

    WaypointForCar FindClosestWaypoint(Vector3 target)
    {
        GameObject closest = null;
        float closestDist = Mathf.Infinity;

        foreach (var waypoint in GameObject.FindGameObjectsWithTag("waypoint"))
        {
            var dist = (waypoint.transform.position - target).magnitude;
            if (dist < closestDist)
            {
                closest = waypoint;
                closestDist = dist;
            }
        }
        if (closest != null)
        {
            return closest.GetComponent<WaypointForCar>();
        }

        Debug.LogWarning("웨이포인트를 찾지못함!");
        return null;
    }

    void OnDrawGizmosSelected()
    {
        if (destWaypoint != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(destWaypoint.transform.position, 0.5f);
            Handles.Label(destWaypoint.transform.position, "destWP");
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(curDestPos, 0.25f);
        Handles.Label(curDestPos, "curDest");
    }
}
