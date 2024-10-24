using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy_Mage : Enemy, IObjectPool<Enemy_Mage>
{
    // private Material thisMaterial;
    private MoveTowardsTransform _moveTowardsTransform;
    private Tween attackTween;

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float zProjectileSpawnOffset;
    [SerializeField] private float yProjectileSpawnOffset = .2f;

    public override void SetupObjectFirstTime()
    {
        base.SetupObjectFirstTime();
        //_moveTowardsTransform = gameObject.GetComponent<MoveTowardsTransform>();
        if (TryGetComponent(out MoveTowardsTransform walkTowardsTransform))
        {
            _moveTowardsTransform = walkTowardsTransform;
        }
        else
        {
            Debug.LogWarning("pawn didn't have a walk towards transform component");
        }
    }

    private void Start()
    {
        _moveTowardsTransform.goalAttackLine = EnemyTarget.I.mageLine;
    }

    public override void InitializeObjectFromPool()
    {
        base.InitializeObjectFromPool();

        StartCoroutine(RandomlyStopToAttack());
    }

    private void OnDisable()
    {
        attackTween.Kill();
    }

    private IEnumerator RandomlyStopToAttack()
    {
        yield return new WaitForSeconds(Random.Range(3f, 10f));
        performBehavior = Attack;
    }

    public override void SwitchToAttackMode()
    {
        //Prevent the default attack behavior
        //base.SwitchToAttackMode();
    }

    protected override void Attack()
    {
        StopAllCoroutines();
        SFXPlayer.I.Play(AudioEventsStorage.I.projectileShot);
        _moveTowardsTransform.enabled = false;
        //PlayerInfo.I.health.AdjustHp((int)-damage, gameObject);
        EnemyProjectile enemyProjectile = ObjectPoolManager<EnemyProjectile>.GetObject(projectilePrefab);
        if (enemyProjectile is not null)
        {
            enemyProjectile.transform.position = new Vector3(transform.position.x,
                transform.position.y + yProjectileSpawnOffset, 
                transform.position.z + zProjectileSpawnOffset);
            enemyProjectile.damage = -damage;
            enemyProjectile.gameObject.SetActive(true);
        }

        _enemyAnimations?.Play(EnemyAnimations.AnimationFrames.Attack01,
            () =>
            {
                isTryingToAttack = false;

                DecideNextAnimation();
                StartCoroutine(RandomlyStopToAttack());
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
        ObjectPoolManager<Enemy_Mage>.ReturnObject(this);
    }
}