using System;
using DG.Tweening;
using UnityEngine;

public class Enemy_Turtle : Enemy, IObjectPool<Enemy_Turtle>
{
    // private Material thisMaterial;
    [SerializeField] private AnimationCurve attackCurve;
    [SerializeField] public int handStabDamage = 1;

    private MoveTowardsTransform _moveTowardsTransform;
    private Tween attackTween;

    private Collider _collider;

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

        _collider = GetComponent<Collider>();
    }

    public override void InitializeObjectFromPool()
    {
        base.InitializeObjectFromPool();
        _collider.enabled = true;
    }

    private void OnDisable()
    {
        attackTween.Kill();
    }

    protected override void Attack()
    {
        _moveTowardsTransform.enabled = false;
        _enemyAnimations?.Play(EnemyAnimations.AnimationFrames.Attack01,
            () =>
            {
                SFXPlayer.I.Play(AudioEventsStorage.I.enemyAttacked);
                PlayerInfo.I.health.AdjustHp((int)-damage, gameObject);
                isTryingToAttack = false;
                _moveTowardsTransform.BackUp();
                _moveTowardsTransform.enabled = true;
                DecideNextAnimation();
            });

        performBehavior = null;
    }

    public override float HandleDeath(int lastAttack, GameObject killer)
    {
        _collider.enabled = false;
        // thisMaterial.color = Color.red;
        return base.HandleDeath(lastAttack, killer);
    }

    public override void ReturnObjectToPool()
    {
        ObjectPoolManager<Enemy_Turtle>.ReturnObject(this);
    }
}