using System;
using UnityEngine;

public class Enemy_Spike : Enemy, IObjectPool<Enemy_Spike>
{
    [SerializeField] public int handStabDamage = 1;

    private void OnEnable()
    {
        InitializeObjectFromPool();
    }

    private void OnDisable()
    {
        ReturnObjectToPool();
    }

    public override void ReturnObjectToPool()
    {
        throw new NotImplementedException();
    }
}
