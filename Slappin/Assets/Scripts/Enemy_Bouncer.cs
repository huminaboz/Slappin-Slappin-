
public class Enemy_Bouncer : Enemy, IObjectPool<Enemy_Bouncer>
{
    public override void ReturnObjectToPool()
    {
        throw new System.NotImplementedException();
    }
}
