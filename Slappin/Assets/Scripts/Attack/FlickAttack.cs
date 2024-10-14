using System;
using QFSW.QC;
using UnityEngine;

public class FlickAttack : AttackType
{
    //State is switched to flick when you first press the flick button
    
    //If you let go of the button while it's still heading down,
    //it will do the attack and go back up
    
    //If you hold it down, it will charge until you let it go
    
    //Doing the attack creates a collider from the finger and forward
    //you can move until the attack is performing
    //distance of the collider is based on charge
    //and also on flick distance multiplier

    [Header("Flick Specific Stuff")]
        //For now, while we're switching between models for frames, two models
    [SerializeField] public GameObject chargeFrame;
    [SerializeField] public GameObject hitFrame;
    [SerializeField] private ParticleSystem flickParticle;
    
    private ObjectShake _shake;
    private Action _currentAction;

    public override void Initialize()
    {
        base.Initialize();
        _shake = GetComponent<ObjectShake>();
    }

    private void Start()
    {
        //For now, while we're switching between models for frames, two models
        chargeFrame.SetActive(true);
        hitFrame.SetActive(false);
    }
    
    private void Update()
    {
        _currentAction?.Invoke();
    }

    public override void InitiateAttack()
    {
        base.InitiateAttack();
        
        InitiateTravelToGround();
        player.EnableMovement();
    }

    protected override void InitiateTravelToGround()
    {
        base.InitiateTravelToGround();
        chargeFrame.SetActive(true);
        hitFrame.SetActive(false);
    }

    protected override void DoWhenReachingGround()
    {
        base.DoWhenReachingGround();
        _currentAction = ChargeAttack;
        //TODO::Make the forecast appear in front of the finger,
        //starting with a small circle in the center of the normal forecast
        
    }

    private void ChargeAttack()
    {
        _shake.StartShake();
        
        //TODO:: increase charge
        
        //TODO:: As you charge, the forecast on the ground grows longer/wider relative to the attack

        if (!Input.GetButton("Fire3"))
        {
            ReleaseCharge();
            _currentAction = null;
        }
    }

    private void ReleaseCharge()
    {
        flickParticle.Play();
        player.DisableMovement();
        _shake.StopShake();
        chargeFrame.SetActive(false);
        hitFrame.SetActive(true);
        InitiateTravelBackUp();
    }

    protected override void InitiateTravelBackUp()
    {
        //Stop for a bit to see the hand
        const float handRestDuration = .15f;
        StartCoroutine(BozUtilities.DoAfterDelay(handRestDuration, () =>
        {
            player.EnableMovement();
            base.InitiateTravelBackUp();
        }));
    }

    protected override void Cleanup()
    {
        _currentAction = null;
        base.Cleanup();
    }
}
