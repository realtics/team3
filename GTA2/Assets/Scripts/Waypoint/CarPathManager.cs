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

	Vector3 finalDestPos;
    Vector3 curDestPos;
    public int curLane = 0;
    int laneMax = 1;

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

	public void SelectNewDestinationImmediate()
	{
		GameObject go = WaypointManager.instance.FindClosestWaypoint(WaypointManager.WaypointType.car, transform.position);
		curWaypoint = go.GetComponent<WaypointForCar>();
		curLane = Random.Range(0, 3);
		SetRandomDestWaypoint();
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

        laneMax = curWaypoint.carRoadDict[destWaypoint].laneEndPosition.Count - 1;

        if (curWaypoint.canChangeLane)
        {
            curLane = Random.Range(0, laneMax + 1);
        }

        if (curLane > laneMax)
        {
            curLane = laneMax;
        }

		finalDestPos = curWaypoint.carRoadDict[destWaypoint].laneEndPosition[curLane];
		CalcSubDestPosition();		
	}

	void CalcSubDestPosition()
	{
		Vector3 startPos = curWaypoint.carRoadDict[destWaypoint].laneStartPosition[curLane];
		Vector3 endPos = curWaypoint.carRoadDict[destWaypoint].laneEndPosition[curLane];
		Vector3 dir = endPos - startPos;
		dir.y = 0;

		Vector3 originToPoint = transform.position - startPos;
		originToPoint.y = 0;
		float distanceFromOrigin = Vector3.Dot(dir.normalized, originToPoint.normalized) * originToPoint.magnitude;
		curDestPos = startPos + dir.normalized * Mathf.Clamp(distanceFromOrigin+3, 0, dir.magnitude);
	}

    void Move()
    {
        Vector3 dir = (curDestPos - transform.position);
        dir.y = 0;
        float dist = dir.sqrMagnitude;

		if(dist < 0.5f * 0.5f)
		{
			if (curDestPos == finalDestPos)
			{
				curWaypoint = destWaypoint;
				SetRandomDestWaypoint();
			}
			else
			{
				CalcSubDestPosition();
			}

			carAi.SetDestination(curDestPos);
		}
    }

    void OnDrawGizmosSelected()
    {
        if (destWaypoint != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(destWaypoint.transform.position, 0.25f);
            //Handles.Label(destWaypoint.transform.position, "destWP");
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(curDestPos, 0.25f);
        //Handles.Label(curDestPos, "curDest");
    }
}
