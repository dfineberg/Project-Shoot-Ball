using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public int playerNo;
    public float moveSpeed;
    public float accelleration;
    public float turnSpeed;
    public float shotPower;
    public GameObject projectile;
    public GameObject projectileShotPos;

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
            Shoot();
        }
    }

    void Shoot()
    {
        if(projectile != null)
        {
            if(projectileShotPos != null)
            {
                GameObject newProj = (GameObject)Instantiate(projectile, projectileShotPos.transform.TransformPoint(Vector3.zero), Quaternion.identity);
                newProj.GetComponent<Rigidbody2D>().AddForce(transform.TransformDirection(Vector3.up) * shotPower, ForceMode2D.Impulse);
            }
            else
            {
                Debug.Log("projectileShotPos is null");
            }
        }
        else
        {
            Debug.Log("Projectile is null");
        }
    }
}
