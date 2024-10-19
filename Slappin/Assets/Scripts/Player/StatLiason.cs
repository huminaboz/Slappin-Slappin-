using System.Collections.Generic;

public class StatLiason : Singleton<StatLiason>
{
    public Dictionary<Stat, float> Stats = new Dictionary<Stat, float>();

    public float Get(Stat stat)
    {
        return Stats[stat];
    }
}
