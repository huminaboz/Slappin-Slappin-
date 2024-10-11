using System.Collections;
using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class SlapAttack : AttackType, IHpAdjustmentListener
{
    // [SerializeField] private Transform shadow;
    [SerializeField] private Transform slapPosition;

    [SerializeField] private Health playerHealth;

    [SerializeField] private Transform handModelTransform;

    [SerializeField] private Player player;

    //Todo:: Just make this disappear
    [SerializeField] private float OffScreenSlapYPosition = .88f; //Measured by holding it off camera
    [SerializeField] private float groundYPosition = -0.78f; //Measured by putting the hand on the ground 

    [SerializeField] private AnimationCurve startSlapCurve;

    [SerializeField] private Renderer slapRenderer;
    private Material slapMaterial;


    private void Awake()
    {
        slapMaterial = slapRenderer.material;
    }

    private void Start()
    {
        slapPosition.DOMoveY(OffScreenSlapYPosition, 0f);
        // transform.localPosition = new Vector3(transform.position.x, OffScreenSlapYPosition, transform.position.z);
    }

    private Tween downTween;

    public void DropSlap()
    {
        if (!playerHealth.isAlive)
        {
            Debug.LogWarning("Can't slap - player is dead");
            return;
        }

        DumpCollisionsLists();
        
        downTween.Kill();
        // slapCollider.enabled = true;
        //Warp to the off screen Y position and the x and z position of the shadow

        //Tween down to the ground
        downTween = slapPosition.DOMoveY(groundYPosition, attackData.attackSpeed)
            .SetEase(startSlapCurve)
            .OnComplete(() =>
            {
                //Make a big puff of smoke or something
                // slapCollider.enabled = false;
                // Debug.Break();
                HandleCollisions();
                StartCoroutine(GoBackUp(.1f));
            });
        player.DisableInputs();
    }

    //I think I'm going to have to send the hand down, collect all the collision data from multiple colliders
    //Then sort through it and decide what was allowed to happen

    //It could be a delegate that gets functions added to it and if you hit a spike, it empties the delegate and puts in the spike outcome
    // Or save up a list of stuff to hit and stuff to collect and then wait for the go ahead to do it or not

    
    public void HitSpike(GameObject thingThatGotHit)
    {
        //If hitting a spike, take damage and go back up
        downTween.Kill();
        Enemy_Spike enemySpike = thingThatGotHit.GetComponent<Enemy_Spike>();
        playerHealth.AdjustHp(-enemySpike.handStabDamage, gameObject);
        slapMaterial.color = Color.red;
        DumpCollisionsLists();
        StartCoroutine(GoBackUp(1f));
    }

    IEnumerator GoBackUp(float returnDelay)
    {
        yield return new WaitForSeconds(returnDelay * attackData.slapRecoveryMultiplier);

        
        //To be able to start moving again while on the way up
        player.EnableMovement();

        //tween back up from current position
        slapPosition.DOMoveY(OffScreenSlapYPosition, .1f * attackData.slapRecoveryMultiplier)
            .SetEase(Ease.InQuint)
            .OnComplete(() =>
            {
                slapMaterial.color = Color.white;
                player.EnableButtonInput();
            });
    }

    public void TookDamage(int damageAmount, GameObject attacker)
    {
        //Feedback on the hand that it hit a spike
    }

    public void Healed(int healAmount, GameObject healer)
    {
    }

    public float HandleDeath(int lastAttack, GameObject killer)
    {
        downTween.Kill();
        return 0;
    }
}