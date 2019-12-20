using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WaypointForCar : Waypoint
{
    [HideInInspector]
    public Dictionary<WaypointForCar, CarRoad> carRoadDict = new Dictionary<WaypointForCar, CarRoad>();
    public List<Waypoint> prev = new List<Waypoint>();
    public List<Waypoint> next = new List<Waypoint>();
	public WaypointForCar mainNext;

    public bool canChangeLane;

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        foreach (var n in next)
        {
            Gizmos.DrawLine(transform.position, n.transform.position);
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
    }

    protected override void Awake()
    {
        base.Awake();

        CarRoad[] carRoads = GetComponentsInChildren<CarRoad>();
        foreach (var road in carRoads)
        {
            carRoadDict.Add(road.endWaypoint, road);
        }
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnObjectMoved()
    {
        base.OnObjectMoved();

		if (prev != null)
			transform.forward = transform.position - prev[0].transform.position;

		foreach (var road in carRoadDict)
        {
            road.Value.transform.position = transform.position;
			road.Value.CalcLanePos();
        }

        foreach (WaypointForCar wp in prev)
        {
            if(wp.carRoadDict.ContainsKey(this))
                wp.carRoadDict[this].CalcLanePos();
        }
    }

    public void AddBranch(WaypointForCar targetWaypoint)
    {
        RemoveBranch(targetWaypoint);

        GameObject go = new GameObject("CarRoad", typeof(CarRoad));
        go.transform.SetParent(transform);
        CarRoad carRoad = go.GetComponent<CarRoad>();
        carRoad.Init(this, targetWaypoint);

		if (prev != null)
			carRoad.numOfLane = (prev[0] as WaypointForCar).carRoadDict[this].numOfLane;

        carRoadDict.Add(targetWaypoint, carRoad);
        next.Add(targetWaypoint);
        targetWaypoint.prev.Add(this);
    }

    public void RemoveBranch(WaypointForCar targetWaypoint)
    {
        if (carRoadDict.ContainsKey(targetWaypoint) && carRoadDict[targetWaypoint].gameObject != null)
            DestroyImmediate(carRoadDict[targetWaypoint].gameObject);

        carRoadDict.Remove(targetWaypoint);
        next.Remove(targetWaypoint);
    }
}
