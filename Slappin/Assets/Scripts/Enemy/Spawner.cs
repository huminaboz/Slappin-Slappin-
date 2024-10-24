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

    private void SpawnEnemy<T>(GameObject prefab, float postSpawnDelay) where T : MonoBehaviour, IObjectPool<T>
    {
        T enemy = ObjectPoolManager<T>.GetObject(prefab);
        if (enemy is not null)
        {
            enemy.transform.position = GetRandomSpawnPosition();
            enemy.transform.Rotate(0, 180, 0);
            extraSpawnTime = postSpawnDelay;
        }
    }

    private void DoSpawn()
    {
        if (GetRandomNumberBetweenZeroAndOne() < bouncerSpawnChance * StatLiason.I.GetEnemy(Stat.Enemy_SpawnChance)
            && DifficultyManager.I.currentWave >= 12)
        {
            SpawnEnemy<Enemy_Bouncer>(bouncerPrefab, 2f);
        }
        else if (GetRandomNumberBetweenZeroAndOne() <
                 spikeSpawnChance * StatLiason.I.GetEnemy(Stat.Enemy_SpawnChance)
                 && DifficultyManager.I.currentWave >= 10)
        {
            SpawnEnemy<Enemy_Spike>(spikePrefab, 0f);
        }
        else if (GetRandomNumberBetweenZeroAndOne() <
                 orcSpawnChance * StatLiason.I.GetEnemy(Stat.Enemy_SpawnChance)
                 && DifficultyManager.I.currentWave >= 8)
        {
            SpawnEnemy<Enemy_Pawn>(orcPrefab, 1.5f);
        }
        else if (GetRandomNumberBetweenZeroAndOne() <
                 mageSpawnChance * StatLiason.I.GetEnemy(Stat.Enemy_SpawnChance)
                 && DifficultyManager.I.currentWave >= 6)
        {
            SpawnEnemy<Enemy_Mage>(magePrefab, .25f);
        }
        else
        {
            if (GetRandomNumberBetweenZeroAndOne() <
                1 - turtleSpawnChance * StatLiason.I.GetEnemy(Stat.Enemy_SpawnChance)
                || DifficultyManager.I.currentWave < 3)
            {
                if (IsDivisibleBy(4, DifficultyManager.I.currentWave) && DifficultyManager.I.currentWave > 3)
                {
                    DoSpawn();
                }
                else
                {
                    SpawnEnemy<Enemy_Pawn>(pawnPrefab, -.25f);
                }
            }
            else
            {
                if (IsDivisibleBy(3, DifficultyManager.I.currentWave) && DifficultyManager.I.currentWave > 3)
                {
                    DoSpawn();
                }
                else
                {
                    SpawnEnemy<Enemy_Turtle>(turtlePrefab, 0f);
                }
            }
        }
    }

    bool IsDivisibleBy(int count, int number)
    {
        int sumOfDigits = 0;

        // Calculate the sum of the digits
        while (number != 0)
        {
            sumOfDigits += number % 10;
            number /= 10;
        }

        // Check if the sum is divisible by 3
        return sumOfDigits % count == 0;
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