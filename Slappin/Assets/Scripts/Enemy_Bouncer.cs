
public class Enemy_Bouncer : Enemy, IObjectPool<Enemy_Bouncer>
{
    public override void ReturnObjectToPool()
    {
        ObjectPoolManager<Enemy_Bouncer>.ReturnObject(this);
    }
}
