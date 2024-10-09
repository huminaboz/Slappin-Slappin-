using UnityEngine;
using UnityEngine.ProBuilder;

public class Enemy_Pawn : Enemy, IObjectPool<Enemy_Pawn>
{
    // private Material thisMaterial;

    
    public override void SetupObjectFirstTime()
    {
        base.SetupObjectFirstTime();
        // thisMaterial = GetComponent<Renderer>().material;
        transform.position += Vector3.up * 0.07410529f;
    }

    public override void InitializeObjectFromPool()
    {
        base.InitializeObjectFromPool();
        
    }

    protected override void Attack()
    {
        //TODO:: Do some sort of wiggle (representing the attack)
        //TODO:: When wiggle finishes, damage the player
        //(needs easy access to player health)
        //player needs some sort of feedback of getting hurt
            //sfx
            //visual - vfx on the screen of ouchies?
            //Screen shake?
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
