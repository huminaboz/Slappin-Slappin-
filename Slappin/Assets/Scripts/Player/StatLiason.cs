using System.Collections.Generic;

public class StatLiason : Singleton<StatLiason>
{
    public Dictionary<Stat, float> Stats = new Dictionary<Stat, float>();
}
