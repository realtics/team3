using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarInput))]
[RequireComponent(typeof(CarMovement))]
[RequireComponent(typeof(CarAi))]
[RequireComponent(typeof(CarPassengerManager))]
[RequireComponent(typeof(CarDamage))]
[RequireComponent(typeof(CarEffects))]
public class CarManager : MonoBehaviour
{
    public CarInput input;
    public CarMovement movement;
    public CarAi ai;
    public CarPassengerManager passengerManager;
    public CarDamage damage;
    public CarEffects effects;

    public delegate void CarHandler();
    public event CarHandler OnReturnKeyDown;

    public event CarHandler OnDamage;
    public event CarHandler OnDestroy;

    public delegate void CarPassengerHandler(People people);
    public event CarPassengerHandler OnDriverGetOn;
    public event CarHandler OnDriverGetOff;

    public enum CarState
    {
        idle, controlledByPlayer, controlledByAi, destroied
    }
    public CarState carState = CarState.controlledByAi;

    void OnEnable()
    {
        carState = CarState.controlledByAi;
    }

    public void OnReturnKeyDownEvent()
    {
        OnReturnKeyDown();
    }

    public void OnDestroyEvent()
    {
        if (carState == CarState.destroied)
            return;

        OnDestroy();
        carState = CarState.destroied;
    }

    public void OnDamageEvent()
    {
        if (carState == CarState.destroied)
            return;

        OnDamage();
    }

    public void OnDriverGetOnEvent(People p)
    {
        OnDriverGetOn?.Invoke(p);

        if (p == GameManager.Instance.player)
        {
            carState = CarState.controlledByPlayer;
        }
        else
        {
            carState = CarState.controlledByAi;
        }
    }

    public void OnDriverGetOffEvent()
    {
        OnDriverGetOff();

        carState = CarState.idle;
    }

    void Update()
    {
        switch (carState)
        {
            case CarState.controlledByPlayer:
                {
                    input.PlayerInput();
                }
                break;
            case CarState.controlledByAi:
                {
                    // 코루틴으로 변경할것.
                    ai.ChasePlayer();
                }
                break;
        }
    }

    void FixedUpdate()
    {
        switch (carState)
        {
            case CarState.controlledByPlayer:
                {
                    movement.FixedLoop();
                }
                break;
            case CarState.controlledByAi:
                {
                    ai.FixedLoop();
                }
                break;
        }
    }
}
