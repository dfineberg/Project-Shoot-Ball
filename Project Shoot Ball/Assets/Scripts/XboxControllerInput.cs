using UnityEngine;
using System.Collections;

public class XboxControllerInput : MonoBehaviour, IGetPlayerInput {

    PlayerController myPlayer;

    public static float TRIGGERS_THRESHOLD = 0.2f;
    public static float ANALOGUE_STICK_THRESHOLD = 0.3f;


    void Start()
    {
        myPlayer = GetComponent<PlayerController>();
    }

    //save the left and right stick input axes to Vector2s and give them to 'input'
    public void GetInput(ref PlayerInput input)
    {
        Vector2 leftStickInput = new Vector2(Input.GetAxis("L_XAxis_" + myPlayer.playerNo), Input.GetAxis("L_YAxis_" + myPlayer.playerNo));
        Vector2 rightStickInput = new Vector2(Input.GetAxis("R_XAxis_" + myPlayer.playerNo), Input.GetAxis("R_YAxis_" + myPlayer.playerNo));

        if(leftStickInput.SqrMagnitude() > (ANALOGUE_STICK_THRESHOLD * ANALOGUE_STICK_THRESHOLD))
        {
            input.SetMovement(leftStickInput);
        }
        else
        {
            input.SetMovement(Vector2.zero);
        }

        if(rightStickInput.SqrMagnitude() > (ANALOGUE_STICK_THRESHOLD * ANALOGUE_STICK_THRESHOLD))
        {
            input.SetAim(rightStickInput);
        }
        else
        {
            input.SetAim(Vector2.zero);
        }

        //check if the right trigger is pressed
        bool action = Input.GetAxis("TriggersR_" + myPlayer.playerNo) > TRIGGERS_THRESHOLD ? true : false;

        //if action was false and is now true, set actionDown to true
        if(!input.GetAction() && action)
        {
            input.SetActionDown(true);
        }
        else
        {
            //otherwise actionDown is false
            input.SetActionDown(false);
        }

        //check if the left trigger is pressed
        bool secondaryAction = Input.GetAxis("TriggersL_" + myPlayer.playerNo) > TRIGGERS_THRESHOLD ? true : false;

        //if secondaryAction was false and is now true, set secondaryActionDown to true
        if (!input.GetSecondaryAction() && secondaryAction)
        {
            input.SetSecondaryActionDown(true);
        }
        else
        {
            //otherwise secondaryActionDown is false
            input.SetSecondaryActionDown(false);
        }

        input.SetAction(action);
        input.SetSecondaryAction(secondaryAction);
    }
}
