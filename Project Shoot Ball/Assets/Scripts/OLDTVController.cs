using UnityEngine;
using System.Collections;

public class OLDTVController : MonoBehaviour {

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
