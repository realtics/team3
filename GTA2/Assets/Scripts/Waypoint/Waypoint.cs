using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Waypoint : MonoBehaviour
{
    public Color gizmoColor = Color.yellow;
    Vector3 oldPos;

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        
    }

    protected virtual void Awake()
    { 

    }

    protected virtual void Update()
    {
        if (transform.position == oldPos)
            return;

        oldPos = transform.position;
        OnObjectMoved();
    }

    protected virtual void OnObjectMoved()
    {
        
    }
}
