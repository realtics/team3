﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    public enum SignalColor
    {
        SC_Green, SC_Red
    }
    public SignalColor signalColor = SignalColor.SC_Red;

    public Light pointLight;
    public BoxCollider boxColliderForCar;
    public BoxCollider boxColliderForPed1;
    public BoxCollider boxColliderForPed2;

    public void ToggleSignal()
    {
        if(signalColor == SignalColor.SC_Green)
        {
            signalColor = SignalColor.SC_Red;
            pointLight.color = Color.red;

            boxColliderForCar.enabled = true;
            boxColliderForPed1.enabled = false;
            boxColliderForPed2.enabled = false;
        }
        else
        {
            signalColor = SignalColor.SC_Green;
            pointLight.color = Color.green;

            boxColliderForCar.enabled = false;
            boxColliderForPed1.enabled = true;
            boxColliderForPed2.enabled = true;
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