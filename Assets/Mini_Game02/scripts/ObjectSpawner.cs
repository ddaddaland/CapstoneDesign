using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public Transform[] obstaclePrefab;
    public Transform car;
    public float forwardDistance = 20f; //자동차와 장애물 생성 거리 차이
    public float spawnTime = 2f;
    private float currentTime = 0f;
    private Transform[] spawnPos;
    void Start()
    {
        spawnPos = new Transform[transform.childCount];
        for(int i = 0; i < transform.childCount; i++)
        {
            spawnPos[i] = transform.GetChild(i).transform;
        }

    }
    void Update()
    {
        if(currentTime >= spawnTime)
        {
            SpawnObjects();
            currentTime = 0f;
        }
        else
        {
            currentTime += Time.deltaTime;
        }
    }
    private void LateUpdate()
    {
        Vector3 spawnManagerPos = new Vector3(0, car.position.y + forwardDistance, 0);
        transform.position = spawnManagerPos;
    }
    void SpawnObjects()
    {
        int obstacleCnt = Random.Range(1, 4);
        List<int> availableRoad = new List<int> { 0, 1, 2, 3 };
        for(int i = 0; i< obstacleCnt; i++)
        {
            int randomObstacle = Random.Range(0, obstaclePrefab.Length);
            int randomIndex = Random.Range(0, availableRoad.Count);
            int selectedRoad = availableRoad[randomIndex];
            Instantiate(obstaclePrefab[randomObstacle], spawnPos[selectedRoad].position, Quaternion.identity);
            availableRoad.RemoveAt(randomIndex);
        }
    }
}
