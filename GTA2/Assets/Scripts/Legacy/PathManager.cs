using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    //public CarCtr carCtr; // 정리필요...

    //Stack<WaypointV2> curPath;
    //WaypointV2 destWaypoint;
    //WaypointV2 prevWaypoint;
    //Vector3 curDestPos;
    //Vector3 finalDestPos;
    //int laneSize = 4;
    //public int myLane = 0;

    //// 방식2. 정리필요
    //WaypointV2 curWaypoint;
    //WaypointV2 lastWaypoint;

    //enum DestType
    //{
    //    DT_EndOfRoad, DT_StartOfRoad
    //}
    //DestType destType = DestType.DT_EndOfRoad;

    //void DebugSetNewDestination()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        Vector3 mouserPos = Input.mousePosition;
    //        mouserPos.z = 20;
    //        finalDestPos = Camera.main.ScreenToWorldPoint(mouserPos);
    //        finalDestPos.y = 0;

    //        Stop();
    //        NavigateTo(finalDestPos);
    //    }
    //}

    //public void NavigateTo(Vector3 dest)
    //{
    //    curPath = new Stack<WaypointV2>();
    //    WaypointV2 curNode = FindClosestWaypoint(transform.position);
    //    prevWaypoint = curNode;
    //    WaypointV2 endNode = FindClosestWaypoint(dest);
    //    if (curNode == null || endNode == null || curNode == endNode)
    //        return;

    //    var openList = new SortedList<float, WaypointV2>(6);
    //    var closedList = new List<WaypointV2>(6);
    //    openList.Add(0, curNode);
    //    curNode.prev = null;
    //    curNode.dist = 0f;

    //    while(openList.Count > 0)
    //    {
    //        curNode = openList.Values[0];
    //        openList.RemoveAt(0);
    //        var dist = curNode.dist;
    //        closedList.Add(curNode);
    //        if (curNode == endNode)
    //            break;

    //        foreach (var neighbor in curNode.neighbors)
    //        {
    //            if (closedList.Contains(neighbor) || openList.ContainsValue(neighbor))
    //                continue;

    //            neighbor.prev = curNode;
    //            neighbor.dist = dist + (neighbor.transform.position - curNode.transform.position).magnitude;
    //            var distToTarget = (neighbor.transform.position - endNode.transform.position).magnitude;

    //            if(!openList.ContainsKey(neighbor.dist + distToTarget))
    //                openList.Add(neighbor.dist + distToTarget, neighbor);
    //        }
    //    }

    //    if(curNode == endNode)
    //    {
    //        while (curNode.prev != null)
    //        {
    //            curPath.Push(curNode);
    //            curNode = curNode.prev;
    //        }
    //        destWaypoint = curPath.Pop();
    //        laneSize = Mathf.Min(prevWaypoint.numOfLane, destWaypoint.numOfLane) * 2;
    //        transform.position = prevWaypoint.lanePosDict[destWaypoint][myLane];
    //        curDestPos = destWaypoint.lanePosDict[prevWaypoint][laneSize - 1 - myLane];
    //        destType = DestType.DT_EndOfRoad;

    //        carCtr.SetDestination(curDestPos);
    //    }
    //    else
    //    {
    //        Stop();
    //        print("경로가 없음");
    //    }
    //}

    //public void Stop()
    //{
    //    curPath = null;
    //    carCtr.StopCar();

    //    //GameObject[] wp = GameObject.FindGameObjectsWithTag("waypoint");
    //    //finalDestPos = transform.position;
    //    //while (finalDestPos == transform.position)
    //    //{
    //    //    finalDestPos = wp[Random.Range(0, wp.Length)].transform.position;
    //    //}
        
    //    //NavigateTo(finalDestPos);
    //}

    ///*
    //void Move()
    //{
    //    if (curPath == null)
    //        return;

    //    Vector3 dist = curDestPos - transform.position;

    //    if (dist.magnitude < 0.3f)
    //    {
    //        if (curPath.Count <= 0)
    //        {
    //            Stop();

    //            return;
    //        }

    //        if (dist.magnitude < 0.05f)
    //        {
    //            if (destType == DestType.DT_EndOfRoad)
    //            {
    //                curDestPos = destWaypoint.lanePosDict[curPath.Peek()][myLane];
    //                destType = DestType.DT_StartOfRoad;
    //            }
    //            else if (destType == DestType.DT_StartOfRoad)
    //            {
    //                prevWaypoint = destWaypoint;
    //                destWaypoint = curPath.Pop();
    //                laneSize = Mathf.Min(prevWaypoint.numOfLane, destWaypoint.numOfLane) * 2;
    //                curDestPos = destWaypoint.lanePosDict[prevWaypoint][laneSize - 1 - myLane];
    //                destType = DestType.DT_EndOfRoad;
    //            }

    //            carCtr.SetDestination(curDestPos);
    //        }
    //    }
    //}
    //*/

    //void SetRandomDestWaypoint()
    //{
    //    curWaypoint = FindClosestWaypoint(transform.position);
    //    while (true)
    //    {
    //        destWaypoint = curWaypoint.neighbors[Random.Range(0, curWaypoint.neighbors.Count)];
    //        if (curWaypoint.neighbors.Count == 1 || destWaypoint != lastWaypoint)
    //            break;
    //    }
    //    lastWaypoint = curWaypoint;
    //    curDestPos = curWaypoint.lanePosDict[destWaypoint][myLane];
    //}

    //void Move()
    //{
    //    float dist = (curDestPos - transform.position).magnitude;

    //    if (dist < 0.05f)
    //    {
    //        if (destType == DestType.DT_EndOfRoad)
    //        {
    //            SetRandomDestWaypoint();

    //            destType = DestType.DT_StartOfRoad;
    //        }
    //        else if (destType == DestType.DT_StartOfRoad)
    //        {
    //            laneSize = Mathf.Min(curWaypoint.numOfLane, destWaypoint.numOfLane) * 2;
    //            curDestPos = destWaypoint.lanePosDict[curWaypoint][laneSize - 1 - myLane];

    //            destType = DestType.DT_EndOfRoad;
    //        }

    //        carCtr.SetDestination(curDestPos);
    //    }
    //}

    //void Start()
    //{
    //    SetRandomDestWaypoint();

    //    destType = DestType.DT_StartOfRoad;

    //    carCtr.SetDestination(curDestPos);
    //}

    //void Update()
    //{
    //    //DebugSetNewDestination();


    //    UnityEngine.Profiling.Profiler.BeginSample("sdfsdfsdf");
          

    //    Move();

    //    UnityEngine.Profiling.Profiler.EndSample();
    //}

    //WaypointV2 FindClosestWaypoint(Vector3 target)
    //{
    //    GameObject closest = null;
    //    float closestDist = Mathf.Infinity;

    //    foreach(var waypoint in GameObject.FindGameObjectsWithTag("waypoint"))
    //    {
    //        var dist = (waypoint.transform.position - target).magnitude;
    //        if(dist < closestDist)
    //        {
    //            closest = waypoint;
    //            closestDist = dist;
    //        }
    //    }
    //    if(closest != null)
    //    {
    //        return closest.GetComponent<WaypointV2>();
    //    }
    //    return null;
    //}

    //void OnDrawGizmosSelected()
    //{
    //    if (destWaypoint != null)
    //    {
    //        Gizmos.color = Color.white;
    //        Gizmos.DrawWireSphere(destWaypoint.transform.position, 0.5f);
    //    }

    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(curDestPos, 0.25f);
    //}

}
