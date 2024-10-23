using System.Collections;
using System.Collections.Generic;
using QFSW.QC;
using UnityEngine;

public class DifficultyManager : Singleton<DifficultyManager>
{
    //TODO:: Get a reference to the spawner
    [SerializeField] private Spawner _spawner;
    public int currentWave = 1;

    [SerializeField] public float maxSpawnSpeedBoostMultiplier = 5f;
    [SerializeField] public AnimationCurve spawnSpeedBoostCurve;
    
    public void SetupNextWave()
    {
        currentWave++;
        Debug.LogWarning($"Current Wave is now: {currentWave}");
        StatLiason.I.UpgradeEnemyStats();
    }

    [Command]
    private void SetWave(int wave)
    {
        currentWave = wave;
    }
    
}