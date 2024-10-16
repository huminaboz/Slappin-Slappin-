using UnityEngine;

public class Enemy_Spike : Enemy, IObjectPool<Enemy_Spike>
{
    [SerializeField] public int handStabDamage = 2;
    [SerializeField] public float handStabStunDuration = 1f;

    private Collider _collider;

    public override void SetupObjectFirstTime()
    {
        base.SetupObjectFirstTime();
        _collider = GetComponent<Collider>();
    }

    public override void InitializeObjectFromPool()
    {
        base.InitializeObjectFromPool();
        _collider.enabled = true;
    }

    public override float HandleDeath(int lastAttack, GameObject killer)
    {
        _collider.enabled = false;
        return base.HandleDeath(lastAttack, killer);
    }

    protected override void Attack()
    {
    }

    public override void ReturnObjectToPool()
    {
        ObjectPoolManager<Enemy_Spike>.ReturnObject(this);
    }
}
