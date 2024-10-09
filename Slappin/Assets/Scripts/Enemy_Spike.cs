using System;
using UnityEngine;

public class Enemy_Spike : Enemy, IObjectPool<Enemy_Spike>
{
    [SerializeField] public int handStabDamage = 1;

    protected override void Attack()
    {
        throw new NotImplementedException();
    }

    public override void ReturnObjectToPool()
    {
        ObjectPoolManager<Enemy_Spike>.ReturnObject(this);
    }
}
