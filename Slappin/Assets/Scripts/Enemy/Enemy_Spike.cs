using UnityEngine;

public class Enemy_Spike : Enemy, IObjectPool<Enemy_Spike>
{
    [SerializeField] public int handStabDamage = 1;
    [SerializeField] public float handStabStunDuration = 1f;
    
    protected override void Attack()
    {
    }

    public override void ReturnObjectToPool()
    {
        ObjectPoolManager<Enemy_Spike>.ReturnObject(this);
    }
}
