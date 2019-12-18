using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointForHuman : Waypoint
{
    public List<WaypointForHuman> neighbor = new List<WaypointForHuman>();

    protected override void OnDrawGizmos()
    {
		Gizmos.color = gizmoColor;
		Gizmos.DrawWireSphere(transform.position, transform.localScale.x / 2);

        foreach (var n in neighbor)
        {
            Gizmos.DrawLine(transform.position, n.transform.position);
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

		foreach (var n in neighbor)
		{
			Vector3 dir = n.transform.position - transform.position;
			Vector3 right = Vector3.Cross(dir.normalized, Vector3.up);

			Gizmos.DrawLine(transform.position + (right * transform.localScale.x / 2), 
				n.transform.position + (right * n.transform.localScale.x / 2));

			Gizmos.DrawLine(transform.position + (right * transform.localScale.x / -2),
				n.transform.position + (right * n.transform.localScale.x / -2));
		}
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
