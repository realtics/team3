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
    public event CarEventHandler OnDamage; // 피해받음
    public event CarEventHandler OnDestroy; // 파괴됨

    public event CarEventHandler OnDriverGetOn; // 탑승
    public event CarEventHandler OnDriverGetOff; // 하차

    public void OnDestroyEvent()
    {
        OnDestroy();
    }

    public void OnDamageEvent()
    {
        OnDamage();
    }
}
