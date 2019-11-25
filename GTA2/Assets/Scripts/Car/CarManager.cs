using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarController))]
[RequireComponent(typeof(CarControllAi))]
[RequireComponent(typeof(CarDamage))]
[RequireComponent(typeof(CarEffectsController))]
public class CarManager : MonoBehaviour
{
    public CarController carController;
    public CarControllAi carAi;
    public CarDamage carDamage;
    public CarEffectsController carEffects;

    public delegate void CarEventHandler();
    public static event CarEventHandler OnDamage; // 피해받음
    public static event CarEventHandler OnDestroy; // 파괴됨

    public static event CarEventHandler OnDriverGetOn; // 탑승
    public static event CarEventHandler OnDriverGetOff; // 하차

    public void OnDestroyEvent()
    {
        OnDestroy();
    }

    public void OnDamageEvent()
    {
        OnDamage();
    }
}
