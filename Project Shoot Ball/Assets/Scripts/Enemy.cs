using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour, IExplodable {

    public float spawnTime;

    SpriteRenderer spriteRenderer;
    new Collider2D collider;
    PolyNavAgent polyNavAgent;
    bool spawning = true;

    public delegate void EnemyEvent(Enemy enemy);
    public static event EnemyEvent e_gotBall;

    public static void DestroyAll()
    {
        foreach(Enemy enemy in FindObjectsOfType<Enemy>())
        {
            Destroy(enemy.gameObject);
        }
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        polyNavAgent = GetComponent<PolyNavAgent>();
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        StartCoroutine(SpawnFlash());
        polyNavAgent.enabled = false;
        collider.enabled = false;
        yield return new WaitForSeconds(spawnTime);
        spawning = false;
        polyNavAgent.enabled = true;
        collider.enabled = true;
    }

    IEnumerator SpawnFlash()
    {
        while (spawning)
        {
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.enabled = !spriteRenderer.enabled;
        }

        spriteRenderer.enabled = true;
    }

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
