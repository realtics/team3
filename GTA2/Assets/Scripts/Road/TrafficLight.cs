using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    public enum SignalColor
    {
        SC_Green, SC_Red
    }
    public SignalColor signalColor = SignalColor.SC_Red;

    public Light light;

    public void ToggleSignal()
    {
        if(signalColor == SignalColor.SC_Green)
        {
            signalColor = SignalColor.SC_Red;
            light.color = Color.red;
        }
        else
        {
            signalColor = SignalColor.SC_Green;
            light.color = Color.green;
        }
    }

    void OnDrawGizmos()
    {
        if(signalColor == SignalColor.SC_Green)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawSphere(transform.position, 0.3f);
    }
}
