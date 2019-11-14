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

        Vector3 dir = endWaypoint.transform.position - transform.position;
        Vector3 right = Vector3.Cross(dir.normalized, Vector3.up);

        laneStartPosition.Clear();
        laneEndPosition.Clear();
        for (int i = 0; i < numOfLane; i++)
        {
            laneStartPosition.Add(transform.position + (right * i * laneWidth));
            laneEndPosition.Add(laneStartPosition[i] + dir);
        }
    }
}
