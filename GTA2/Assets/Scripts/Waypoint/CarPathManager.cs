using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
	float lastLaneChangeTime = 0;

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
		destWaypoint = curWaypoint.next[Random.Range(0, curWaypoint.next.Count)] as WaypointForCar;

		if(curWaypoint.next.Count > 1 && curWaypoint.mainNext != null && destWaypoint != curWaypoint.mainNext)
		{
			// 가장 우측 차선이 아니면 우회전 불가
			if (curLane != 0 && 
				Vector3.SignedAngle(transform.forward, destWaypoint.transform.position - transform.position, Vector3.up) > 10)
			{
				destWaypoint = curWaypoint.mainNext;
			}

			// 가장 좌측 차선이 아니면 좌회전 불가
			if(curLane != laneMax &&
				Vector3.SignedAngle(transform.forward, destWaypoint.transform.position - transform.position, Vector3.up) < -10)
			{
				destWaypoint = curWaypoint.mainNext;
			}
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

	public void ChangeLaneIfPossible()
	{
		if (laneMax < 1)
			return;

		if (destWaypoint.next.Count > 1)
			return;

		if (lastLaneChangeTime + 3 > Time.time)
			return;

		bool isLaneChanged = false;

		if(curLane > 0)
		{
			if (!Physics.Raycast(transform.position + (transform.right * 0.9f) - (transform.forward * 2f), 
				transform.forward, 8f))
			{
				curLane--;
				isLaneChanged = true;
			}			
		}
		else if(curLane < laneMax)
		{
			if (!Physics.Raycast(transform.position - (transform.right * 0.9f) - (transform.forward * 2f), 
				transform.forward, 8f))
			{
				curLane++;
				isLaneChanged = true;
			}			
		}

		if(isLaneChanged)
		{
			lastLaneChangeTime = Time.time;
			finalDestPos = curWaypoint.carRoadDict[destWaypoint].laneEndPosition[curLane];
			CalcSubDestPosition();
			carAi.SetDestination(curDestPos);
		}
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
            Gizmos.DrawWireSphere(destWaypoint.transform.position, 0.3f);
            // Handles.Label(destWaypoint.transform.position, "destWP");
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(curDestPos, 0.25f);
        // Handles.Label(curDestPos, "curDest");
    }
}
