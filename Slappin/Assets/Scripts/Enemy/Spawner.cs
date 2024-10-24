using System;
using System.Collections;
using Mono.CSharp;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    [SerializeField] private float minTimeBetweenSpawns = .25f;
    [SerializeField] private float maxTimeBetweenSpawns = .5f;
    private float timeTilNextSpawn = 2f;
    private float extraSpawnTime = 0f;


    [SerializeField] private GameObject spikePrefab;
    [SerializeField] private GameObject bouncerPrefab;
    [SerializeField] private GameObject pawnPrefab;
    [SerializeField] private GameObject turtlePrefab;
    [SerializeField] private GameObject magePrefab;
    [SerializeField] private GameObject orcPrefab;

    [SerializeField] private float mageSpawnChance = .1f;
    [SerializeField] private float orcSpawnChance = .065f;
    [SerializeField] private float spikeSpawnChance = .05f;
    [SerializeField] private float bouncerSpawnChance = .013f;
    [SerializeField] private float turtleSpawnChance = .15f;

    [SerializeField] private Transform topLeftPossibleSpawn;
    [SerializeField] private Transform bottomRightPossibleSpawn;

    [Header("DEBUG")] [SerializeField] private bool onlySpawnOne = false;

    private float t = 0;

    private void Start()
    {
        if (onlySpawnOne)
        {
            DoSpawn();
            Debug.LogError("ONLY SPAWN ONE IS ON");
        }
    }


    private void Update()
    {
        if (onlySpawnOne) return;
        t += Time.deltaTime;
        if (t >= timeTilNextSpawn)
        {
            DoSpawn();
            t = 0;
            timeTilNextSpawn = GetRandomNextSpawnTime() + extraSpawnTime;
        }
    }

    private void DoSpawn()
    {
        if (GetRandomNumberBetweenZeroAndOne() < bouncerSpawnChance * StatLiason.I.GetEnemy(Stat.Enemy_SpawnChance))
        {
            Enemy_Bouncer enemy = ObjectPoolManager<Enemy_Bouncer>.GetObject(bouncerPrefab);
            if (enemy is not null)
            {
                enemy.transform.position = GetRandomSpawnPosition();
                enemy.transform.Rotate(0, 180, 0);
                extraSpawnTime = 1f;
            }
        }
        else if (GetRandomNumberBetweenZeroAndOne() <
                 spikeSpawnChance * StatLiason.I.GetEnemy(Stat.Enemy_SpawnChance))
        {
            Enemy_Spike enemy = ObjectPoolManager<Enemy_Spike>.GetObject(spikePrefab);
            if (enemy is not null)
            {
                enemy.transform.position = GetRandomSpawnPosition();
                enemy.transform.Rotate(0, 180, 0);
                extraSpawnTime = .5f;
            }
        }
        else if (GetRandomNumberBetweenZeroAndOne() <
                 orcSpawnChance * StatLiason.I.GetEnemy(Stat.Enemy_SpawnChance))
        {
            Enemy_Pawn orcPawn = ObjectPoolManager<Enemy_Pawn>.GetObject(orcPrefab);
            if (orcPawn is not null)
            {
                orcPawn.transform.position = GetRandomSpawnPosition();
                orcPawn.transform.Rotate(0, 180, 0);
                extraSpawnTime = .5f;
            }
        }
        else if (GetRandomNumberBetweenZeroAndOne() <
                 mageSpawnChance * StatLiason.I.GetEnemy(Stat.Enemy_SpawnChance))
        {
            Enemy_Mage enemy = ObjectPoolManager<Enemy_Mage>.GetObject(magePrefab);
            if (enemy is not null)
            {
                enemy.transform.position = GetRandomSpawnPosition();
                enemy.transform.Rotate(0, 180, 0);
                extraSpawnTime = .25f;
            }
        }
        else
        {
            if (GetRandomNumberBetweenZeroAndOne() <
                1 - turtleSpawnChance * StatLiason.I.GetEnemy(Stat.Enemy_SpawnChance))
            {
                Enemy_Pawn enemy = ObjectPoolManager<Enemy_Pawn>.GetObject(pawnPrefab);
                if (enemy is not null)
                {
                    enemy.transform.position = GetRandomSpawnPosition();
                    enemy.transform.Rotate(0, 180, 0);
                    extraSpawnTime = 0f;
                }
            }
            else
            {
                Enemy_Turtle enemy = ObjectPoolManager<Enemy_Turtle>.GetObject(turtlePrefab);
                if (enemy is not null)
                {
                    enemy.transform.position = GetRandomSpawnPosition();
                    extraSpawnTime = 0f;
                }
            }
        }

    }


    private Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(topLeftPossibleSpawn.position.x, bottomRightPossibleSpawn.position.x);
        float z = Random.Range(topLeftPossibleSpawn.position.z, bottomRightPossibleSpawn.position.z);

        return new Vector3(x, .01f, z);
    }

    private float GetRandomNumberBetweenZeroAndOne()
    {
        return Random.Range(0f, 1f);
    }

    private float GetRandomNextSpawnTime()
    {
        float spawnRateMultiplier = StatLiason.I.GetEnemy(Stat.Enemy_SpawnRate);

        //TODO:: Include current wave into the equation

        float spawnTimer = Random.Range(minTimeBetweenSpawns * spawnRateMultiplier
            , maxTimeBetweenSpawns * spawnRateMultiplier);

        Debug.Log($"Spawn timer is: {spawnTimer}");

        return spawnTimer;
    }
}