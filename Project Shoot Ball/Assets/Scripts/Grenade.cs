using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour {

    public float fuseTime;
    public float blastRadius;
    public float blastTime;

    bool exploded = false;
    new Collider2D collider;
    new Rigidbody2D rigidbody;

    public delegate void GrenadeEvent();
    public static event GrenadeEvent e_explode;



    void Start()
    {
        collider = GetComponent<Collider2D>();
        collider.isTrigger = false;
        rigidbody = GetComponent<Rigidbody2D>();
        StartCoroutine(FuseCountdown());
    }

    //countdown initiated on instantiation
    IEnumerator FuseCountdown()
    {
        yield return new WaitForSeconds(fuseTime);

        if (!exploded)
        {
            Explode();
        }
    }

    //initiates the explosion
    void Explode()
    {
        exploded = true;
        collider.isTrigger = true;
        rigidbody.velocity = Vector2.zero;
        StartCoroutine(ExplodeRoutine());

        if (e_explode != null)
        {
            e_explode();
        }
    }

    //coroutine for the explosion animation
    IEnumerator ExplodeRoutine()
    {
        Vector3 startScale = transform.localScale;
        float timer = 0f;

        while(timer <= (blastTime * 0.5f))
        {
            transform.localScale = Vector3.Lerp(startScale, startScale * blastRadius, timer / (blastTime * 0.5f));
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0f;

        while(timer <= (blastTime * 0.5f))
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, timer / (blastTime * 0.5f));
            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    //collider starts off as solid
    void OnCollisionEnter2D(Collision2D col)
    {
        //if the grenade collides with something explodable, start exploding
        IExplodable explodable = col.gameObject.GetComponent(typeof(IExplodable)) as IExplodable;

        if(explodable != null)
        {
            Explode();
            explodable.Explode();
        }
    }

    //collider is switched to a trigger upon explosion
    void OnTriggerEnter2D(Collider2D col)
    {
        //if any IExplodables are caught in the blast radius, call their Explode methods
        IExplodable explodable = col.GetComponent(typeof(IExplodable)) as IExplodable;

        if(explodable != null)
        {
            explodable.Explode();
        }
    }
}
