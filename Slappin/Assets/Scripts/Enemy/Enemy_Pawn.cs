using System;
using DG.Tweening;
using UnityEngine;

public class Enemy_Pawn : Enemy, IObjectPool<Enemy_Pawn>
{
    // private Material thisMaterial;
    [SerializeField] private AnimationCurve attackCurve;

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
        attackTween = transform.DORotate(new Vector3(57f, 0, 0), .5f, RotateMode.LocalAxisAdd)
            .SetEase(attackCurve)
            .OnComplete(() =>
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
        ObjectPoolManager<Enemy_Pawn>.ReturnObject(this);
    }
}