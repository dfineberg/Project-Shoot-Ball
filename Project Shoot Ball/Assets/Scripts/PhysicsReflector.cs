using UnityEngine;
using System.Collections;

public class PhysicsReflector : MonoBehaviour {

    [Range(0f, 1f)]
    public float bounce = 0.8f;

    new Rigidbody2D rigidbody;
    Vector2 velocityCache;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        velocityCache = rigidbody.velocity;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Vector2 reflectedVelocity = Vector2.Reflect(velocityCache, col.contacts[0].normal);
        reflectedVelocity *= bounce;
        rigidbody.velocity = reflectedVelocity;
    }
}
