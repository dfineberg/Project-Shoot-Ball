using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

    PlayerController carrier;
    Rigidbody2D rigidbody;
    Collider2D collider;
    static Ball instance;


    void Start()
    {
        instance = this;
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
    }

    public static PlayerController GetCarrier()
    {
        return instance.carrier;
    }
}
