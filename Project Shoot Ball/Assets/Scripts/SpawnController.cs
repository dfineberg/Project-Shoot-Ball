using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnController : MonoBehaviour {

    public GameObject mainEnemy;
    public int startClusterSize;
    public int sizeIncrement;
    public float timeBetweenClusters;
    public float incrementTime;

    static SpawnController instance;
    static bool spawning = false;
    public static bool Spawning
    {
        get { return spawning; }
    }
    public static int currentClusterSize { get; private set; }
    static float startTime;

    List<List<Vector3>> spawnPositions;

    void Start()
    {
        instance = this;
        spawnPositions = new List<List<Vector3>>();

        for(int i = 0; i < 5; i++)
        {
            spawnPositions.Add(new List<Vector3>());

            foreach(GameObject thisTile in GameObject.FindGameObjectsWithTag("Spawn" + i))
            {
                spawnPositions[i].Add(thisTile.transform.position);
            }

            Debug.Log("Spawn cluster " + i + " contains " + spawnPositions[i].Count + " positions");
        }
    }

    public static void StartSpawning()
    {
        spawning = true;
        currentClusterSize = instance.startClusterSize;
        startTime = Time.time;
        instance.StartCoroutine(instance.SpawnRoutine());
    }

    public static void StopSpawning()
    {
        spawning = false;
    }

    IEnumerator SpawnRoutine()
    {
        StartCoroutine(ClusterIncrementRoutine());

        while (spawning)
        {
            Spawn(currentClusterSize);
            yield return new WaitForSeconds(timeBetweenClusters);
        }
    }

    IEnumerator ClusterIncrementRoutine()
    {
        while (spawning)
        {
            yield return new WaitForSeconds(incrementTime);
            currentClusterSize += sizeIncrement;
        }
    }

    void Spawn(int clusterSize)
    {
        //make a list of all the spawn clusters that have at least one position in them
        List<int> possiblePositions = new List<int>();

        for(int i = 0; i < spawnPositions.Count; i++)
        {
            if(spawnPositions[i].Count > 0)
            {
                possiblePositions.Add(i);
            }
        }

        //pick a random cluster from the possible positions and spawn enemies in that one
        Spawn(clusterSize, possiblePositions[Random.Range(0, possiblePositions.Count - 1)]);
    }

    void Spawn(int clusterSize, int positionNo)
    {
        if(positionNo < spawnPositions.Count)
        {
            for (int i = 0; i < clusterSize && i < spawnPositions[positionNo].Count; i++)
            {
                Instantiate(mainEnemy, spawnPositions[positionNo][i], Quaternion.identity);
            }
        }
        else
        {
            Debug.Log("invalid spawn position no.");
        }
    }
}
