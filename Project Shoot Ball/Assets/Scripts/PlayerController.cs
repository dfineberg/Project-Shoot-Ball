﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public int playerNo;
    public float moveSpeed;
    public float accelleration;
    public float turnSpeed;
    public float shotPower;
    public float throwPower;
    public GameObject projectile;
    public Transform projectileShotPos;
    [HideInInspector]
    public bool hasBall = false;

    IGetPlayerInput myInputComponent;
    PlayerInput myInput;
    new Rigidbody2D rigidbody;
    GameObject ball;
    Collider2D ballCol;
    Rigidbody2D ballRigidbody;


    void Start()
    {
        myInputComponent = GetComponent(typeof(IGetPlayerInput)) as IGetPlayerInput;
        rigidbody = GetComponent<Rigidbody2D>();
        myInput = new PlayerInput();
        ball = GameObject.FindGameObjectWithTag("Ball");
        ballCol = ball.GetComponent<Collider2D>();
        ballRigidbody = ball.GetComponent<Rigidbody2D>();
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

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject == ball && !hasBall)
        {
            CatchBall();
        }
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

    //checks if the player is performing a primary action (shooting/throwing)
    void ActionCheck()
    {
        if (myInput.GetActionDown())
        {
            if (!hasBall)
            {
                Shoot();
            }
            else
            {
                ThrowBall();
            }
        }
    }

    void Shoot()
    {
        if(projectile != null)
        {
            if(projectileShotPos != null)
            {
                GameObject newProj = (GameObject)Instantiate(projectile, projectileShotPos.TransformPoint(Vector3.zero), Quaternion.identity);
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

    void CatchBall()
    {
        ballCol.isTrigger = true;
        ballRigidbody.velocity = Vector2.zero;
        ballRigidbody.isKinematic = true;
        ball.transform.SetParent(transform);
        ball.transform.localPosition = projectileShotPos.localPosition;
        hasBall = true;
    }

    void ThrowBall()
    {
        ballCol.isTrigger = false;
        ball.transform.SetParent(null);
        ballRigidbody.isKinematic = false;
        ballRigidbody.AddForce(transform.TransformDirection(Vector3.up).normalized * throwPower, ForceMode2D.Impulse);
        StartCoroutine(ThrowRoutine());
    }

    IEnumerator ThrowRoutine()
    {
        yield return new WaitForSeconds(0.1f);
        hasBall = false;
    }
}
