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

    [SerializeField] private float mageSpawnChance = .1f;
    [SerializeField] private float spikeSpawnChance = .05f;
    [SerializeField] private float bouncerSpawnChance = .013f;

    [SerializeField] private Transform topLeftPossibleSpawn;
    [SerializeField] private Transform bottomRightPossibleSpawn;

    [SerializeField] private SO_Upgrade spawnRateMultiplier;

    private float t = 0;

    private void Update()
    {
        t += Time.deltaTime;
        if (t >= timeTilNextSpawn)
        {
            if (GetRandomNumberBetweenZeroAndOne() < bouncerSpawnChance)
            {
                Enemy_Bouncer enemy = ObjectPoolManager<Enemy_Bouncer>.GetObject(bouncerPrefab);
                if (enemy is not null)
                {
                    enemy.transform.position = GetRandomSpawnPosition();
                    enemy.transform.Rotate(0, 180, 0);
                    extraSpawnTime = 1f;
                }
            }
            else if (GetRandomNumberBetweenZeroAndOne() < spikeSpawnChance)
            {
                Enemy_Spike enemy = ObjectPoolManager<Enemy_Spike>.GetObject(spikePrefab);
                if (enemy is not null)
                {
                    enemy.transform.position = GetRandomSpawnPosition();
                    enemy.transform.Rotate(0, 180, 0);
                    extraSpawnTime = .5f;
                }
            }
            else if (GetRandomNumberBetweenZeroAndOne() < mageSpawnChance)
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
                if (GetRandomNumberBetweenZeroAndOne() < .7f)
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

            t = 0;
            timeTilNextSpawn = GetRandomNextSpawnTime() + extraSpawnTime;
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
        int wave = DifficultyManager.I.currentWave;

        float spawnTimer = Random.Range(minTimeBetweenSpawns * spawnRateMultiplier
                .newValueGrowthCurve.ComputeGrowth(spawnRateMultiplier.baseValue, wave)
            , maxTimeBetweenSpawns * spawnRateMultiplier
                .newValueGrowthCurve.ComputeGrowth(spawnRateMultiplier.baseValue, wave));

        Debug.LogWarning($"Spawn timer is: {spawnTimer}");
        
        return spawnTimer;
    }
}