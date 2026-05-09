using UnityEngine;
using System.Collections.Generic;

public class RoadSpawner : MonoBehaviour
{
    public Transform car;
    public Transform road;
    public int roadCount = 5;
    private Transform lastRoad;
    private Queue<Transform> roadQueue = new Queue<Transform>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < roadCount; i++)
        {
            addRoad();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(car.position.y > roadQueue.Peek().transform.position.y + 25f)
        {
            RecycleRoad();
        }
    }
    void addRoad()
    {
        Vector3 spawnPos = (lastRoad == null) ? Vector3.zero : lastRoad.GetComponent<RoadInfo>().roadSpawnPos.position;
        Transform roadIns = Instantiate(road, spawnPos, Quaternion.identity);
        roadQueue.Enqueue(roadIns);
        lastRoad = roadIns;
    }
    void RecycleRoad()
    {
        Transform oldRoad = roadQueue.Dequeue();

        oldRoad.transform.position = lastRoad.GetComponent<RoadInfo>().roadSpawnPos.position;
        roadQueue.Enqueue(oldRoad);
        lastRoad = oldRoad;
    }
}
