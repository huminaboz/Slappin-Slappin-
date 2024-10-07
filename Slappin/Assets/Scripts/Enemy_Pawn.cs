using UnityEngine;

public class Enemy_Pawn : Enemy, IObjectPool<Enemy_Pawn>
{
    // private Material thisMaterial;
    
    public override void SetupObjectFirstTime()
    {
        base.SetupObjectFirstTime();
        // thisMaterial = GetComponent<Renderer>().material;
    }

    public override void InitializeObjectFromPool()
    {
        base.InitializeObjectFromPool();
        
    }

    public override float HandleDeath(int lastAttack, GameObject killer)
    {
        // thisMaterial.color = Color.red;
        return base.HandleDeath(lastAttack, killer);
    }

    public override void ReturnObjectToPool()
    {
        ObjectPoolManager<Enemy_Pawn>.ReturnObject(this);
    }
}
