
public class Enemy_Bouncer : Enemy, IObjectPool<Enemy_Bouncer>
{
    protected override void Attack()
    {
        //TODO:: Explode and deal the damage
    }

    public override void ReturnObjectToPool()
    {
        ObjectPoolManager<Enemy_Bouncer>.ReturnObject(this);
    }
}
