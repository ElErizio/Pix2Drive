using UnityEngine;

public class AlexInputSystem : MonoBehaviour
{
    public float horizontal;
    public float vertical;
    public bool breaking;

    void Update()
    {
        // Steering Input
        SetHorizontalAxis(Input.GetAxis("Horizontal"));

        // Acceleration Input
        SetVerticalAxis(Input.GetAxis("Vertical"));

        // Breaking Input
        SetBreakInput(Input.GetKey(KeyCode.Space));
    }

    public float GetHorizontalAxis()
    {
        return horizontal;
    }
    public void SetHorizontalAxis(float axis)
    {
        horizontal = axis;
    }
    public float GetVerticalAxis()
    {
        return vertical;
    }
    public void SetVerticalAxis(float axis)
    {
        vertical = axis;
    }

    public bool GetBreakInput()
    {
        return breaking;
    }

    public void SetBreakInput(bool newBool)
    {
        breaking = newBool;
    }
}
