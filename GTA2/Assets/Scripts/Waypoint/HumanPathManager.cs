using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

public class HumanPathManager : MonoBehaviour
{
    public NPC humanCtr;

    WaypointForHuman curWaypoint;
    WaypointForHuman destWaypoint;
    WaypointForHuman lastWaypoint;

    Vector3 curDestPos;

    private void Awake()
    {
        humanCtr = GetComponent<NPC>();
    }
    void Start()
    {
        Init();
    }

    void OnEnable()
    {
        Init();
    }

    void Init()
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
        GameObject go = WaypointManager.instance.FindClosestWaypoint(WaypointManager.WaypointType.human, transform.position);
        curWaypoint = go.GetComponent<WaypointForHuman>();

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
        Vector3 dir = (curDestPos - transform.position);
        dir.y = 0;
        float dist = dir.magnitude;

        if (dist < 0.05f)
        {
            SetRandomDestWaypoint();

            humanCtr.SetDestination(curDestPos);
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
