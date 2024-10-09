
public class Enemy_Bouncer : Enemy, IObjectPool<Enemy_Bouncer>
{
    protected override void Attack()
    {
        throw new System.NotImplementedException();
    }

    public override void ReturnObjectToPool()
    {
        ObjectPoolManager<Enemy_Bouncer>.ReturnObject(this);
    }
}
