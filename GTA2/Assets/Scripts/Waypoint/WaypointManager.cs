using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public static WaypointManager instance;

    public GameObject[] allWaypointsForCar;
    public GameObject[] allWaypointsForHuman;

    public enum WaypointType
    {
        car, human
    }

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("여기로 오면 안됨!");
            Destroy(gameObject);
        }

        allWaypointsForCar = GameObject.FindGameObjectsWithTag("waypoint");
        allWaypointsForHuman = GameObject.FindGameObjectsWithTag("waypointForHuman");
    }

    // excludeViewport 를 true로 하면 화면 안에 있는 waypoint는 무시함.
    public GameObject FindClosestWaypoint(WaypointType type, Vector3 origin, bool excludeViewport = false)
    {
        GameObject closest = null;
        float closestDist = Mathf.Infinity;

        GameObject[] allWaypoints;

        if(type == WaypointType.car)
        {
            allWaypoints = allWaypointsForCar;
        }
        else
        {
            allWaypoints = allWaypointsForHuman;
        }

        foreach (var wp in allWaypoints)
        {
            if(excludeViewport)
            {
                Vector3 pos = Camera.main.WorldToViewportPoint(wp.transform.position);
                float offset = 0.5f;
                if (pos.x >= 0 - offset && pos.x <= 1 + offset && pos.y >= 0 - offset && pos.y <= 1 + offset)
                    continue;
            }

            var dist = (wp.transform.position - origin).magnitude;
            if (dist < closestDist)
            {
                closest = wp;
                closestDist = dist;
            }
        }

        if(closest != null)
        {
            return closest;
        }
        else
        {
            Debug.LogWarning("조건을 만족하는 웨이포인트가 없음");
            return null;
        }
    }
}
