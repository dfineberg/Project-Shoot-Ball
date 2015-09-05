using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PolyNavAgent))]
public class BallFollow : MonoBehaviour {

    PolyNavAgent agent;
    GameObject ball;
    public static bool chasing = true;

    void Start()
    {
        ball = GameObject.FindGameObjectWithTag("Ball");
        agent = GetComponent<PolyNavAgent>();
    }

    void Update()
    {
        if (chasing)
        {
            agent.SetDestination(ball.transform.position);
        }
    }
}
