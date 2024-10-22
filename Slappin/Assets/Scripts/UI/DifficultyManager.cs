using System.Collections;
using System.Collections.Generic;
using QFSW.QC;
using UnityEngine;

public class DifficultyManager : Singleton<DifficultyManager>
{
    //TODO:: Get a reference to the spawner
    [SerializeField] private Spawner _spawner;
    public int currentWave = 1;
    
    
    public void SetupNextWave()
    {
        currentWave++;
        Debug.LogWarning($"Current Wave is now: {currentWave}");
        StatLiason.I.UpgradeEnemyStats();

        //NOTE:: Make sure that the stats put on the new enemies are only on spawn
        //That way any remaining enemies from the last wave will be at the same stats as
        //the last wave
        
        
        //SPAWNING
        //TODO::Turn on spawning for certain enemies
    }

    [Command]
    private void SetWave(int wave)
    {
        currentWave = wave;
    }
    
}