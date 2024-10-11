using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class SlapAttack : AttackType, IHpAdjustmentListener
{
    // [SerializeField] private Transform shadow;
    [SerializeField] private Transform slapPosition;

    [SerializeField] private Health playerHealth;

    [SerializeField] private Transform handModelTransform;

    [SerializeField] private Player player;

    //Todo:: Just make this disappear
    private const float OffScreenSlapYPosition = .88f; //Measured by holding it off camera
    private const float groundYPosition = -0.78f; //Measured by putting the hand on the ground 

    [SerializeField] private AnimationCurve startSlapCurve;
    
   [SerializeField] private Renderer slapRenderer;
    private Material slapMaterial;
    
    
    private void Awake()
    {
        slapMaterial = slapRenderer.material;
    }

    private void Start()
    {
        transform.position = new Vector3(transform.position.x, OffScreenSlapYPosition, transform.position.z);
    }

    private Tween downTween;

    public void DropSlap()
    {
        if (!playerHealth.isAlive)
        {
            Debug.LogWarning("Can't slap - player is dead");
            return;
        }
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
                StartCoroutine(GoBackUp(.1f));
            });
        player.DisableInputs();
    }

    public void HitSpike(GameObject thingThatGotHit)
    {
        //If hitting a spike, take damage and go back up
            downTween.Kill();
            Enemy_Spike enemySpike = thingThatGotHit.GetComponent<Enemy_Spike>();
            playerHealth.AdjustHp(-enemySpike.handStabDamage, gameObject);
            slapMaterial.color = Color.red;
            StartCoroutine(GoBackUp(1f));
    }
    
    // public override void HitSomething(GameObject thingThatGotHit)
    // {
    //     base.HitSomething(thingThatGotHit);
    // }

    //The collider is in another place, probably a child
    private void OnTriggerEnter(Collider other) {}


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