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

    public delegate void CarDamageHandler(bool sourceIsPlayer);
    public event CarDamageHandler OnDamage;
    public event CarDamageHandler OnDestroy;

    public delegate void CarPassengerHandler(People people, int idx);
    public event CarPassengerHandler OnDriverGetOn;
    public event CarPassengerHandler OnDriverGetOff;

    public enum CarState
    {
        idle, controlledByPlayer, controlledByAi, destroied
    }
    public CarState carState = CarState.controlledByAi;

    void OnEnable()
    {
        if(carState == CarState.destroied)
            carState = CarState.controlledByAi;

        StopAllCoroutines();
        StartCoroutine(DisableIfOutOfCamera());
    }

    public void OnReturnKeyDownEvent()
    {
        OnReturnKeyDown();
    }

    public void OnDestroyEvent(bool isDamagedByPlayer)
    {
        if (carState == CarState.destroied)
            return;

        OnDestroy(isDamagedByPlayer);
        carState = CarState.destroied;
    }

    public void OnDamageEvent(bool sourceIsPlayer)
    {
        if (carState == CarState.destroied)
            return;

        OnDamage(sourceIsPlayer);
    }

    public void OnDriverGetOnEvent(People p, int idx)
    {
        OnDriverGetOn?.Invoke(p, idx);

        if(idx == 0)
        {
            if (p == GameManager.Instance.player)
            {
                carState = CarState.controlledByPlayer;
            }
            else
            {
                carState = CarState.controlledByAi;
            }
        }
    }

    public void OnDriverGetOffEvent(People p, int idx)
    {
        OnDriverGetOff?.Invoke(p, idx);

        if(idx == 0)
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

    IEnumerator DisableIfOutOfCamera()
    {
        while (true)
        {
            Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
            float offset = 3f;
            if (pos.x < 0 - offset ||
                pos.x > 1 + offset ||
                pos.y < 0 - offset ||
                pos.y > 1 + offset)
                gameObject.SetActive(false);

            yield return new WaitForSeconds(1.0f);
        }
    }
}
