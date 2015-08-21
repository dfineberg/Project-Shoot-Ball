using UnityEngine;
using System.Collections;

public class PlayerInput
{
    Vector2 movementInput = Vector2.zero;
    Vector2 aimInput = Vector2.zero;

    bool actionDown = false;
    bool action = false;

    public void SetMovement(Vector2 newInput)
    {
        movementInput = newInput;
    }

    public Vector2 GetMovement()
    {
        return movementInput;
    }

    public void SetAim(Vector2 newInput)
    {
        aimInput = newInput;
    }

    public Vector2 GetAim()
    {
        return aimInput;
    }

    public void SetAction(bool newAction)
    {
        action = newAction;
    }

    public bool GetAction()
    {
        return action;
    }

    public void SetActionDown(bool newActionDown)
    {
        actionDown = newActionDown;
    }

    public bool GetActionDown()
    {
        return actionDown;
    }
}



public interface IGetPlayerInput
{
    void GetInput(ref PlayerInput input);
}



public interface IExplodable
{
    void Explode();
}