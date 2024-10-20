using System.Collections;
using System.Collections.Generic;
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

        //NOTE:: Make sure that the stats put on the new enemies are only on spawn
        //That way any remaining enemies from the last wave will be at the same stats as
        //the last wave
        
        //TODO::Increase currency amount dropped
        //TODO::Increase enemy speed
        //TODO::  Increase enemy damage
        //TODO::Increase rates certain enemies spawn
        //TODO:: Increase spawn rate
    }
    
}