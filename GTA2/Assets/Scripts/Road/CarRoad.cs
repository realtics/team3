using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CarRoad : MonoBehaviour
{
    Vector3 oldPosition;

    [Range(1, 3)]
    public int numOfLane = 2;
    public float laneWidth = 0.9f;
    public List<Vector3> laneStartPosition = new List<Vector3>();
    public List<Vector3> laneEndPosition = new List<Vector3>();

    public WaypointForCar parentWaypoint;
    public WaypointForCar endWaypoint;

    public void OnDrawGizmos()
    {
        for (int i = 0; i < numOfLane; i++)
        {
            Gizmos.DrawSphere(laneStartPosition[i], 0.1f);
            Gizmos.DrawSphere(laneEndPosition[i], 0.1f);

            if (i == 0)
            {
                DrawArrow.ForGizmo(laneStartPosition[i], laneEndPosition[i] - laneStartPosition[i], Color.yellow, 0.5f);
            }
            else
            {
                DrawArrow.ForGizmo(laneStartPosition[i], laneEndPosition[i] - laneStartPosition[i], Color.white, 0.5f);
            }            
        }
    }

    public void Init(WaypointForCar parent, WaypointForCar end)
    {
        oldPosition = transform.position;

        parentWaypoint = parent;
        endWaypoint = end;

        CalcLanePos();
    }

    void Update()
    {
        if(transform.position != oldPosition)
        {
            oldPosition = transform.position;
            OnObjectMoved();
        }
    }

    void OnObjectMoved()
    {
        CalcLanePos();
    }

    public void CalcLanePos()
    {
        transform.position = transform.parent.position;

		if (parentWaypoint.prev.Count > 0)
		{
			//Vector3 averageForward = (parentWaypoint.prev[0].transform.forward + endWaypoint.transform.forward).normalized;

			Vector3 avgDir = (endWaypoint.transform.position - parentWaypoint.prev[0].transform.position).normalized;
			Vector3 avgLeft = Vector3.Cross(avgDir, Vector3.up);

			parentWaypoint.transform.right = avgLeft * -1;
		}
		else
		{
			print("asfd");
			parentWaypoint.transform.LookAt(endWaypoint.transform.position, Vector3.up);
		}

		//parentWaypoint.transform.LookAt(endWaypoint.transform.position, Vector3.up);
		//endWaypoint.transform.rotation = parentWaypoint.transform.rotation;

		Vector3 startLeft = parentWaypoint.transform.right * -1;
		Vector3 endLeft = endWaypoint.transform.right * -1;

        laneStartPosition.Clear();
        laneEndPosition.Clear();

        for (int i = 0; i < numOfLane; i++)
        {
			laneStartPosition.Add(parentWaypoint.transform.position + (startLeft * i * laneWidth));
			laneEndPosition.Add(endWaypoint.transform.position + (endLeft * i * laneWidth));
        }
    }
}
