using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    [SerializeField] private float timeBetweenSpawns = 2f;

    [SerializeField] private Enemy[] enemyPrefabs;
    
    [SerializeField] private GameObject spikePrefab;
    [SerializeField] private GameObject bouncerPrefab;
    [SerializeField] private GameObject pawnPrefab;

    [SerializeField] private Transform topLeftPossibleSpawn;
    [SerializeField] private Transform bottomRightPossibleSpawn;

    private float t = 0;
    
    private void Update()
    {
        t += Time.deltaTime;
        if (t >= timeBetweenSpawns)
        {
            if (ObjectPoolManager<Enemy_Pawn>.ExceedingCapacity()) return;
            Enemy_Pawn pawn = ObjectPoolManager<Enemy_Pawn>.GetObject(pawnPrefab);
            if (pawn is null) return;
            pawn.transform.position = GetRandomSpawnPosition(); 
            t = 0;
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(topLeftPossibleSpawn.position.x, bottomRightPossibleSpawn.position.x);
        float z = Random.Range(topLeftPossibleSpawn.position.z, bottomRightPossibleSpawn.position.z);

        return new Vector3(x, 0, z);
    }
}