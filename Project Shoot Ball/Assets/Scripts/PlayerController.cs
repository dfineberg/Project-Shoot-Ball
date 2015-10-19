using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;

public class PlayerController : MonoBehaviour, IHaveAmmo {

    public int playerNo;
    public float moveSpeed;
    public float accelleration;
    public float turnSpeed;
    public float shotPower;
    public float throwPower;
    public float gravityFieldPower;
    [Range(0f, 1f)]
    public float gravityFieldWalkSpeed;
    public int maxShots;
    public float shotRechargeTime;
    public GameObject projectile;
    public Transform projectileShotPos;
    public LayerMask aimLayerMask;
    public float maxAimDistance;
    public Texture aimTexture;
    public float aimWidth = 50f;
    public float aimTextureScale = 2f;
    [HideInInspector]
    public bool hasBall = false;

    public bool gravityFieldActive { get; private set; }

    int currentShots;
    float currentChargeAmount;

    IGetPlayerInput myInputComponent;
    PlayerInput myInput;
    new Rigidbody2D rigidbody;
    CircleCollider2D gravityFieldTrigger;
    GameObject ball;
    Collider2D ballCol;
    Rigidbody2D ballRigidbody;
    ParticleSystem gravityParticles;
    VectorLine aimLine;

    public delegate void PlayerEvent(PlayerController player);
    public static event PlayerEvent e_catchBall;


    void Start()
    {
        myInputComponent = GetComponent(typeof(IGetPlayerInput)) as IGetPlayerInput;
        rigidbody = GetComponent<Rigidbody2D>();
        myInput = new PlayerInput();
        ball = GameObject.FindGameObjectWithTag("Ball");
        ballCol = ball.GetComponent<Collider2D>();
        ballRigidbody = ball.GetComponent<Rigidbody2D>();
        aimLine = new VectorLine("aimLine", new List<Vector3>(3), aimTexture, aimWidth, LineType.Continuous);
        aimLine.textureScale = aimTextureScale;
        gravityParticles = GetComponent<ParticleSystem>();
        gravityParticles.Stop();
        currentShots = maxShots;

        foreach(CircleCollider2D col in GetComponents<CircleCollider2D>())
        {
            if (col.isTrigger)
            {
                gravityFieldTrigger = col;
                break;
            }
        }
    }

    void Update()
    {
        //update the input values stored in myInput
        myInputComponent.GetInput(ref myInput);

        ActionCheck();
        GravityFieldCheck();
        Aim();

        if (myInput.GetAim() != Vector2.zero)
        {
            aimLine.active = true;
            RotatePlayer(myInput.GetAim());
        }
        else
        {
            aimLine.active = false;

            if(myInput.GetMovement() != Vector2.zero)
            {
                RotatePlayer(myInput.GetMovement());
            }
        }
    }

    void FixedUpdate()
    {
        rigidbody.AddForce(myInput.GetMovement().normalized * accelleration);
        rigidbody.velocity = Vector2.ClampMagnitude(rigidbody.velocity, gravityFieldActive ? moveSpeed * gravityFieldWalkSpeed : moveSpeed);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject == ball && !hasBall)
        {
            CatchBall();
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if(col == ballCol && gravityFieldActive)
        {
            Vector3 relativeDir = transform.position - ball.transform.position;
            float forceProportion = 1 - (relativeDir.sqrMagnitude / (gravityFieldTrigger.radius * gravityFieldTrigger.radius));
            forceProportion = Mathf.Clamp01(forceProportion);
            ballRigidbody.AddForce(relativeDir.normalized * (gravityFieldPower * forceProportion) * Time.fixedDeltaTime);
        }
    }

    //checks to see if the player is aiming and rotates the character accordingly
    void RotatePlayer(Vector2 rotateDirection)
    {
        /* the LookRotation method assumes 'forward' means forward in the object's local z-axis
            in 2D top-down perspective forward is actually forward in the y-axis, hence the aim vector being used for the second parameter rather than the first */
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, rotateDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
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

    void GravityFieldCheck()
    {
        if (myInput.GetSecondaryAction())
        {
            if (gravityParticles.isStopped)
            {
                gravityParticles.Play();
                gravityFieldActive = true;
            }
        }
        else if (gravityParticles.isPlaying)
        {
            gravityParticles.Stop();
            gravityFieldActive = false;
        }
    }

    void Shoot()
    {
        if(currentShots > 0)
        {
            currentShots--;

            if (projectile != null)
            {
                if (projectileShotPos != null)
                {
                    GameObject newProj = (GameObject)Instantiate(projectile, projectileShotPos.TransformPoint(Vector3.zero), Quaternion.identity);
                    newProj.GetComponent<Rigidbody2D>().velocity = transform.TransformDirection(Vector3.up) * shotPower;
                    //newProj.GetComponent<Rigidbody2D>().AddForce(transform.TransformDirection(Vector3.up) * shotPower, ForceMode2D.Impulse);
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

    void Aim()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, hasBall ? 0.35f : 0.2f, transform.up, maxAimDistance, aimLayerMask);
        aimLine.points3[0] = transform.position;

        if (hit.collider != null)
        {
            aimLine.points3[1] = hit.point;
            float distanceLeft = maxAimDistance - hit.distance;
            Vector2 reflectedRay = Vector2.Reflect(transform.up, hit.normal);
            reflectedRay.Normalize();
            RaycastHit2D secondHit = Physics2D.Raycast(hit.point + (reflectedRay * 0.1f), reflectedRay, distanceLeft, aimLayerMask);

            if(secondHit.collider != null)
            {
                aimLine.points3[2] = secondHit.point;
            } else
            {
                reflectedRay *= maxAimDistance - hit.distance;
                aimLine.points3[2] = hit.point + reflectedRay;
            }
        }
        else
        {
            aimLine.points3[1] = transform.position + (transform.up * maxAimDistance);
            aimLine.points3[2] = transform.position + (transform.up * maxAimDistance);
        }

        aimLine.Draw3D();
    }

    void CatchBall()
    {
        ballCol.isTrigger = true;
        ballRigidbody.velocity = Vector2.zero;
        ballRigidbody.isKinematic = true;
        ball.transform.SetParent(transform);
        ball.transform.localPosition = Vector2.zero;
        hasBall = true;

        if(currentShots < maxShots)
        {
            StartCoroutine(AmmoRechargeRoutine());
        }

        if(e_catchBall != null)
        {
            e_catchBall(this);
        }
    }

    IEnumerator AmmoRechargeRoutine()
    {
        //while the player is holding the ball and ammo is not yet full
        while (hasBall && currentShots < maxShots)
        {
            //if the player has been charging for a full shotRechargeTime, increase the player's ammo count
            if(currentChargeAmount >= shotRechargeTime)
            {
                currentShots++;
                currentChargeAmount = 0f;
            }
            else
            {
                currentChargeAmount += Time.deltaTime;
            }

            yield return null;
        }

        //if the player no longer has the ball
        if (!hasBall)
        {
            //decrement the chargeAmount to zero
            while(!hasBall && currentChargeAmount > 0f)
            {
                currentChargeAmount -= Time.deltaTime;
                currentChargeAmount = Mathf.Max(currentChargeAmount, 0f);
                yield return null;
            }
        }
    }

    void ThrowBall()
    {
        gameObject.layer = 14;
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
        gameObject.layer = 9;
    }

    public int GetAmmoCount()
    {
        return currentShots;
    }

    public float GetChargeProportion()
    {
        return Mathf.Clamp01(currentChargeAmount / shotRechargeTime);
    }
}
