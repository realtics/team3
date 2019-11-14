using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HumanPathManager : MonoBehaviour
{
    public Citizen humanCtr;

    WaypointForHuman curWaypoint;
    WaypointForHuman destWaypoint;
    WaypointForHuman lastWaypoint;

    Vector3 curDestPos;

    private void Awake()
    {
        humanCtr = GetComponent<Citizen>();
    }
    void Start()
    {
        SetRandomDestWaypoint();
        humanCtr.SetDestination(curDestPos);
    }

    void Update()
    {
        SetNewDestination();
    }

    public void Stop()
    {
        humanCtr.Stop();
    }

    void SetRandomDestWaypoint()
    {
        curWaypoint = FindClosestWaypoint(transform.position);
        while (true)
        {
            destWaypoint = curWaypoint.neighbor[Random.Range(0, curWaypoint.neighbor.Count)] as WaypointForHuman;
            if (curWaypoint.neighbor.Count == 1 || destWaypoint != lastWaypoint)
                break;
        }
        lastWaypoint = curWaypoint;

        curDestPos = destWaypoint.transform.position;
    }

    void SetNewDestination()
    {
        float dist = (curDestPos - transform.position).magnitude;

        if (dist < 0.05f)
        {
            SetRandomDestWaypoint();

            humanCtr.SetDestination(curDestPos);
        }
    }

    WaypointForHuman FindClosestWaypoint(Vector3 target)
    {
        GameObject closest = null;
        float closestDist = Mathf.Infinity;

        foreach (var waypoint in GameObject.FindGameObjectsWithTag("waypointForHuman"))
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
            return closest.GetComponent<WaypointForHuman>();
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
