using System.Collections;
using DG.Tweening;
using UnityEngine;

public class SlapAttack : AttackType, IHpAdjustmentListener, IPickerUpper
{
    [SerializeField] private Transform shadow;
    [SerializeField] private GameObject slap;

    [SerializeField] private Health playerHealth;

    [SerializeField] private SO_AttackData attackData;

    [SerializeField] private Player player;

    private const float OffScreenSlapYPosition = .88f;
    private float groundYPosition;

    [SerializeField] private AnimationCurve startSlapCurve;
    
    private Collider slapCollider;
    private Material slapMaterial;

    private void Awake()
    {
        slapCollider = GetComponent<Collider>();
        slapMaterial = GetComponent<Renderer>().material;
    }

    private void Start()
    {
        slap.transform.position = new Vector3(shadow.position.x, OffScreenSlapYPosition, shadow.position.z);
        groundYPosition = slap.transform.localScale.y;
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
        downTween = slap.transform.DOMoveY(groundYPosition, attackData.attackSpeed)
            .SetEase(startSlapCurve)
            .OnComplete(() => 
            {
                //Make a big puff of smoke or something
                // slapCollider.enabled = false;
                StartCoroutine(GoBackUp(.1f));
            });
        player.DisableInputs();
    }

    private void OnTriggerEnter(Collider other)
    {
        //I think it makes more since to just let whatever the hand hits to handle what it does when it gets slapped

        // Debug.Log(other);
        
        //If hitting a spike, take damage and go back up
        if (other.GetComponent(typeof(Enemy_Spike)) != null) //Could handle this on the spike
        {
            downTween.Kill();
            Enemy_Spike enemySpike = other.GetComponent<Enemy_Spike>();
            playerHealth.AdjustHp(-enemySpike.handStabDamage, gameObject);
            slapMaterial.color = Color.red;
            StartCoroutine(GoBackUp(1f));
            return;
        }

        //If hitting an enemy, do damage to it
        if (other.GetComponent(typeof(Health)) != null)
        {
            Health health = other.GetComponent<Health>();
            health.AdjustHp(-attackData.baseDamage, gameObject);
        }
    }


    IEnumerator GoBackUp(float returnDelay)
    {
        yield return new WaitForSeconds(returnDelay * attackData.slapRecoveryMultiplier);

        //To be able to start moving again while on the way up
        player.EnableMovement();
        
        //tween back up from current position
        slap.transform.DOMoveY(OffScreenSlapYPosition, .1f * attackData.slapRecoveryMultiplier)
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