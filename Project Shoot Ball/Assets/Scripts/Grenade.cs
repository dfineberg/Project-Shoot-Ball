using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour {

    public float fuseTime;
    public float blastRadius;
    public float blastTime;

    bool exploded = false;



    void Start()
    {
        StartCoroutine(FuseCountdown());
    }

    IEnumerator FuseCountdown()
    {
        yield return new WaitForSeconds(fuseTime);

        if (!exploded)
        {
            Explode();
        }
    }

    void Explode()
    {
        exploded = true;

        //////////////////////////
        //EXPLOSION CODE GOES HERE
        //////////////////////////
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        //if the grenade collides with something explodable, start exploding
        IExplodable explodable = col.gameObject.GetComponent(typeof(IExplodable)) as IExplodable;

        if(explodable != null)
        {
            Explode();
        }
    }

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
