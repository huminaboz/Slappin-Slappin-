using System;
using DG.Tweening;
using UnityEngine;

public class Enemy_Pawn : Enemy, IObjectPool<Enemy_Pawn>
{
    // private Material thisMaterial;
    [SerializeField] private AnimationCurve attackCurve;

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
        _moveTowardsTransform.enabled = false;
        _enemyAnimations?.Play(EnemyAnimations.AnimationFrames.Attack01,
            () =>
            {
                isTryingToAttack = false;
                SFXPlayer.I.Play(AudioEventsStorage.I.enemyAttacked);
                PlayerInfo.I.health.AdjustHp((int)-damage, gameObject);
                _moveTowardsTransform.BackUp();
                _moveTowardsTransform.enabled = true;
                DecideNextAnimation();
            });
        // attackTween = transform.DORotate(new Vector3(57f, 0, 0), .5f, RotateMode.LocalAxisAdd)
        //     .SetEase(attackCurve)
        //     .OnComplete(() =>
        //     {
        //         _moveTowardsTransform.BackUp();
        //         _moveTowardsTransform.enabled = true;
        //     });

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