using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointForHuman : Waypoint
{
    public List<WaypointForHuman> neighbor = new List<WaypointForHuman>();

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        foreach (var n in neighbor)
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
        gizmoColor = Color.cyan;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnObjectMoved()
    {
        base.OnObjectMoved();
    }
}
