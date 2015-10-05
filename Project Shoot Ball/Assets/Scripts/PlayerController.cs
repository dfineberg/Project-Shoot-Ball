using UnityEngine;
using System.Collections;

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
    LineRenderer lineRenderer;

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
        lineRenderer = GetComponent<LineRenderer>();
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

        RotatePlayer();
        ActionCheck();
        GravityFieldCheck();
        Aim();

        if (myInput.GetAim() != Vector2.zero)
        {
            lineRenderer.enabled = true;
            RotatePlayer();
        }
        else
        {
            lineRenderer.enabled = false;
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
    void RotatePlayer()
    {
        /* the LookRotation method assumes 'forward' means forward in the object's local z-axis
            in 2D top-down perspective forward is actually forward in the y-axis, hence the aim vector being used for the second parameter rather than the first */
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, myInput.GetAim());
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
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

    void Aim()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, maxAimDistance, aimLayerMask);
        lineRenderer.SetPosition(0, transform.position);

        if (hit.collider != null)
        {
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            lineRenderer.SetPosition(1, transform.position + (transform.up * maxAimDistance));
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

    public int GetAmmoCount()
    {
        return currentShots;
    }

    public float GetChargeProportion()
    {
        return Mathf.Clamp01(currentChargeAmount / shotRechargeTime);
    }
}
