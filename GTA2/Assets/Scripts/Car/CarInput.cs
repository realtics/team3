using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarManager))]
public class CarInput : MonoBehaviour
{
    public CarManager carManager;

    float inputH, inputV;
    float joystickInputH, joystickInputV;
    public float GetInputH() { return inputH; }
    public float GetInputV() { return inputV; }

    void OnEnable()
    {
        carManager.OnDriverGetOff += ReleaseInput;
    }

    void OnDisable()
    {
        carManager.OnDriverGetOff -= ReleaseInput;
    }

    public void PlayerInput()
    {
        if (joystickInputH == 0)
        {
            inputH = Input.GetAxisRaw("Horizontal");
        }
        else
        {
            inputH = joystickInputH;
        }

        if (joystickInputV == 0)
        {
            inputV = Input.GetAxisRaw("Vertical");
        }
        else
        {
            inputV = joystickInputV;
        }

        if (inputV < 0)
            inputH *= -1;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            carManager.OnReturnKeyDownEvent();
        }
    }

    public void AiInput(float h, float v)
    {
        inputH = h;
        inputV = v;
    }

    public void InputVertical(float value)
    {
        joystickInputV = value;
    }

    public void InputHorizon(float value)
    {
        joystickInputH = value;
    }

    public void InputReturn()
    {
        carManager.OnReturnKeyDownEvent();
    }

    void ReleaseInput()
    {
        inputH = 0;
        inputV = 0;
        joystickInputH = 0;
        joystickInputV = 0;
    }
}
