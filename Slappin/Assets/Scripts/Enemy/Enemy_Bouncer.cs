
public class Enemy_Bouncer : Enemy, IObjectPool<Enemy_Bouncer>
{
    protected override void Attack()
    {
        if (!thisHealth) return;
        //TODO:: Explode and deal the damage
        SFXPlayer.I.Play(AudioEventsStorage.I.bouncerExploded);
        PlayerInfo.I.health.AdjustHp((int)-damage, gameObject);
        thisHealth.AdjustHp(-thisHealth.maxHp, gameObject);
    }

    public override void ReturnObjectToPool()
    {
        ObjectPoolManager<Enemy_Bouncer>.ReturnObject(this);
    }
}
