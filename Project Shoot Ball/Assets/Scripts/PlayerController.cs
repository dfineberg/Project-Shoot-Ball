using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public int playerNo;
    public float moveSpeed;
    public float accelleration;
    public float turnSpeed;

    IGetPlayerInput myInputComponent;
    PlayerInput myInput;
    new Rigidbody2D rigidbody;


    void Start()
    {
        myInputComponent = GetComponent(typeof(IGetPlayerInput)) as IGetPlayerInput;
        rigidbody = GetComponent<Rigidbody2D>();
        myInput = new PlayerInput();
    }

    void Update()
    {
        //update the input values stored in myInput
        myInputComponent.GetInput(ref myInput);

        RotatePlayer();
        ActionCheck();
    }

    void FixedUpdate()
    {
        rigidbody.AddForce(myInput.GetMovement().normalized * accelleration);
        rigidbody.velocity = Vector2.ClampMagnitude(rigidbody.velocity, moveSpeed);
    }

    //checks to see if the player is aiming and rotates the character accordingly
    void RotatePlayer()
    {
        if (myInput.GetAim() != Vector2.zero)
        {
            /* the LookRotation method assumes 'forward' means forward in the object's local z-axis
            in 2D top-down perspective forward is actually forward in the y-axis, hence the aim vector being used for the second parameter rather than the first */
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, myInput.GetAim());
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    //checks if the player is performing a primary action (shooting)
    void ActionCheck()
    {
        if (myInput.GetActionDown())
        {
            Debug.Log("perform action");
        }
    }
}
