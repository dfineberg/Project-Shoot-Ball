using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour, IExplodable {

    public delegate void EnemyEvent(Enemy enemy);
    public static event EnemyEvent e_gotBall;

	public void Explode()
    {
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        //if this enemy touches the ball, trigger the gotBall event
        if (col.gameObject.CompareTag("Ball"))
        {
            if(e_gotBall != null)
            {
                e_gotBall(this);
            }
        }
        else
        {
            //if this enemy touches a player and that player is holding the ball, trigger the gotBall event
            PlayerController player = col.gameObject.GetComponent<PlayerController>();

            if (player)
            {
                if (player.hasBall)
                {
                    if (e_gotBall != null)
                    {
                        e_gotBall(this);
                    }
                }
            }
        }
    }
}
