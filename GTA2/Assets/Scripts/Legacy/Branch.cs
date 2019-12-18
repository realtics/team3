using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Branch : MonoBehaviour
{
    Vector3 oldPosition;

    [Range(1, 3)]
    public int numOfLane = 2;
    public float laneWidth = 0.9f;
    public float centerWidth = 0.0f;
    public List<Vector3> lanePosition = new List<Vector3>();

    public WaypointForCar parantWaypoint;
    public Branch myPair;

    public TrafficLight trafficLight;

    private void OnDrawGizmos()
    {
        foreach (var lp in lanePosition)
        {
            Gizmos.DrawSphere(lp, 0.1f);
            for (int i = 0; i < numOfLane * 2; i++)
            {
                // myPair.lanePosition의 인덱스 계산법 수정필요.
                Gizmos.DrawLine(lanePosition[i], myPair.lanePosition[numOfLane - 1 - i]);
            }
        }
    }

    void Start()
    {
        oldPosition = transform.position;
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
        Vector3 dir = (myPair.transform.position - transform.position).normalized;
        Vector3 right = Vector3.Cross(dir.normalized, Vector3.up);

        lanePosition.Clear();
        for (int i = 0; i < numOfLane * 2; i++)
        {
            float offset = (-laneWidth * numOfLane) + (laneWidth / 2) + (i * laneWidth);

            if (i < numOfLane)
            {
                offset -= centerWidth / 2;
            }
            else
            {
                offset += centerWidth / 2;
            }

            lanePosition.Add(transform.position + (right * offset));
        }
    }
}
