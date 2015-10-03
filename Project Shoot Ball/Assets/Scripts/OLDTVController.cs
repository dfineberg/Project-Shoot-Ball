using UnityEngine;
using System.Collections;

[System.Serializable]
public class TVEvent
{
    public float aberration;
    public float noise;
    public float staticMagnitude;
    public float time;
}

public class OLDTVController : MonoBehaviour {

    public TVEvent explosionEvent;
    public float staticScrollSpeed;

    Animator animator;
    int explosionHash;

    void Start()
    {
        animator = GetComponent<Animator>();
        explosionHash = Animator.StringToHash("Explosion");

        Grenade.e_explode += Explode;
    }

    void OnDisable()
    {
        Grenade.e_explode -= Explode;
    }

    void Explode()
    {
        animator.SetTrigger(explosionHash);
    }
}
