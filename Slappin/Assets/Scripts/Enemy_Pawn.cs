using DG.Tweening;
using UnityEngine;

public class Enemy_Pawn : Enemy, IObjectPool<Enemy_Pawn>
{
    // private Material thisMaterial;
    [SerializeField] private AnimationCurve attackCurve;

    [SerializeField] private int attackDamage = 1;

    private WalkTowardsTransform _walkTowardsTransform;

    public override void SetupObjectFirstTime()
    {
        base.SetupObjectFirstTime();
        _walkTowardsTransform = gameObject.GetComponent<WalkTowardsTransform>();
        if (TryGetComponent(out WalkTowardsTransform walkTowardsTransform))
        {
            _walkTowardsTransform = walkTowardsTransform;
        }
        else
        {
            Debug.LogWarning("pawn didn't have a walk towards transform component");
        }
    }

    public override void InitializeObjectFromPool()
    {
        base.InitializeObjectFromPool();
    }

    protected override void Attack()
    {
        _walkTowardsTransform.enabled = false;
        PlayerInfo.I.health.AdjustHp(-attackDamage, gameObject);
        transform.DORotate(new Vector3(57f, 0, 0), .5f, RotateMode.LocalAxisAdd)
            .SetEase(attackCurve)
            .OnComplete(() =>
            {
                _walkTowardsTransform.BackUp();
                _walkTowardsTransform.enabled = true;
            });

        performBehavior = null;
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