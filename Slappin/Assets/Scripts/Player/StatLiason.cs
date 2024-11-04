using System;
using System.Collections.Generic;
using UnityEngine;

public class StatLiason : Singleton<StatLiason>
{
    public Dictionary<Stat, float> Stats = new Dictionary<Stat, float>();

    public Dictionary<Stat, float> EnemyStats = new Dictionary<Stat, float>();
    public Dictionary<Stat, SO_Upgrade> DifficultyIncreasers = new Dictionary<Stat, SO_Upgrade>();


    private void Awake()
    {
        string resourcePath = "DifficultyScrobs";
        SO_Upgrade[] difficulties = Resources.LoadAll<SO_Upgrade>(resourcePath);

        foreach (SO_Upgrade difficultyIncreaser in difficulties)
        {
            // Debug.LogWarning(difficultyIncreaser.stat);
            EnemyStats.Add(difficultyIncreaser.stat, difficultyIncreaser.baseValue);
            DifficultyIncreasers.Add(difficultyIncreaser.stat, difficultyIncreaser);
        }
    }

    public float Get(Stat stat)
    {
        return Stats[stat];
    }

    public float GetEnemy(Stat stat)
    {
        return EnemyStats[stat];
    }

    public void UpgradeEnemyStats()
    {
        List<Stat> keys = new List<Stat>(EnemyStats.Keys);
        foreach (Stat key in keys)
        {
            EnemyStats[key] = DifficultyIncreasers[key].newValueGrowthCurve
                .ComputeGrowth(DifficultyIncreasers[key].baseValue, 
                    DifficultyManager.I.currentWave);
        }
        
        // foreach (System.Collections.Generic.KeyValuePair<Stat, float> entry in EnemyStats)
        // {
        //     EnemyStats[entry.Key]
        //         = DifficultyIncreasers[entry.Key].newValueGrowthCurve
        //             .ComputeGrowth(DifficultyIncreasers[entry.Key].baseValue, 
        //                 DifficultyManager.I.currentWave);
        // }
    }
}