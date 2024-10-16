using System;
using DG.Tweening;
using UnityEngine;

public class Enemy_Turtle : Enemy, IObjectPool<Enemy_Turtle>
{
    // private Material thisMaterial;
    [SerializeField] private AnimationCurve attackCurve;
    [SerializeField] public int handStabDamage = 1;
    [SerializeField] private int attackDamage = 1;

    private MoveTowardsTransform _moveTowardsTransform;
    private Tween attackTween;

    public override void SetupObjectFirstTime()
    {
        base.SetupObjectFirstTime();
        _moveTowardsTransform = gameObject.GetComponent<MoveTowardsTransform>();
        if (TryGetComponent(out MoveTowardsTransform walkTowardsTransform))
        {
            _moveTowardsTransform = walkTowardsTransform;
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

    private void OnDisable()
    {
        attackTween.Kill();
    }

    protected override void Attack()
    {
        SFXPlayer.I.Play(AudioEventsStorage.I.enemyAttacked);
        _moveTowardsTransform.enabled = false;
        PlayerInfo.I.health.AdjustHp(-attackDamage, gameObject);
        _enemyAnimations?.Play(EnemyAnimations.AnimationFrames.Attack01,
            () =>
            {
                _moveTowardsTransform.BackUp();
                _moveTowardsTransform.enabled = true;
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
        ObjectPoolManager<Enemy_Turtle>.ReturnObject(this);
    }
}