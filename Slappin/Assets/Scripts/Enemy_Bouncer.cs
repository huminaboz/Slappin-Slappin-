
public class Enemy_Bouncer : Enemy, IObjectPool<Enemy_Bouncer>
{
    protected override void Attack()
    {
    }

    public override void ReturnObjectToPool()
    {
        ObjectPoolManager<Enemy_Bouncer>.ReturnObject(this);
    }
}
