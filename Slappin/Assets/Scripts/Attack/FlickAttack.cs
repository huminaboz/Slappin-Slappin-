using System;
using DG.Tweening;
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
    [SerializeField] private Transform flickBulletSpawnPoint;
    [SerializeField] private GameObject flickBulletPrefab;
    
    private AnimationCurve chargeCurve;
    private ObjectShake _shake;
    private Action _currentAction;
    private int chargeDamage;
    private SO_AttackData_Flick _flickData;
    private float chargedDistance = 1f;
    private float maxChargeTime;
    private Camera _camera;
    private float startingFoV;

    public override void Initialize()
    {
        base.Initialize();
        _flickData = (SO_AttackData_Flick) attackData;
        _shake = GetComponent<ObjectShake>();
        _camera = cameraTransform.gameObject.GetComponent<Camera>();
        startingFoV = _camera.fieldOfView;
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
        chargedDistance = _flickData.distanceBase;
        chargeDamage = _flickData.baseDamage;
        chargeCurve = _flickData.chargeCurve;
        maxChargeTime = _flickData.maxChargeTime;
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
        //TODO:: increase the FoV on the camera
        
        DOTween.To(() => _camera.fieldOfView, 
            x => _camera.fieldOfView = x, 
            65f, .25f);

    }

    private float currentChargeTime = 0f;
    private float chargeTickRate = .25f;
    private float totalChargeTime = 0f;
    private void ChargeAttack()
    {
        _shake.StartShake();

        //Increase charge
        currentChargeTime += Time.deltaTime;
        totalChargeTime += Time.deltaTime;
        if (currentChargeTime > chargeTickRate && totalChargeTime < maxChargeTime)
        {
            currentChargeTime = 0f;
        }
        
        //Alter the color based on the charge
        float ratio = totalChargeTime / maxChargeTime;
        Color chargeColor = Color.Lerp(_defaultTopOfHandColor, 
            Color.magenta, chargeCurve.Evaluate(ratio));
        handRenderer.material.SetColor("_ColorDim", chargeColor);
        
        //TODO:: As you charge, the forecast on the ground grows longer/wider relative to the attack

        if (!Input.GetButton("Fire3"))
        {
            ReleaseCharge(ratio);
            _currentAction = null;
        }
    }

    private void ReleaseCharge(float ratio)
    {
        chargeDamage = (int) (_flickData.baseDamage * 
                              _flickData.chargeMaxDamageMultiplier * chargeCurve.Evaluate(ratio));
        chargeDamage +=_flickData.baseDamage;
        
        chargedDistance =  chargedDistance 
                           * _flickData.distanceMaxMultiplier * chargeCurve.Evaluate(ratio);
        chargedDistance += _flickData.distanceBase;
        
        //TODO:: calculate width
     
        DOTween.To(() => _camera.fieldOfView, 
            x => _camera.fieldOfView = x, 
            startingFoV, .25f);
        
        flickParticle.Play();
        player.DisableMovement();
        _shake.StopShake();
        chargeFrame.SetActive(false);
        hitFrame.SetActive(true);

        SpawnFlickBullet();

        InitiateTravelBackUp();
        RestoreDefaultAppearance();
    }

    private void SpawnFlickBullet()
    {
        SFXPlayer.I.Play(AudioEventsStorage.I.releasedFlick);
        
        FlickBullet flickBullet = ObjectPoolManager<FlickBullet>.GetObject(flickBulletPrefab);
        
        if (flickBullet is null) return;
        flickBullet.transform.position = flickBulletSpawnPoint.position;
        flickBullet.spawnPosition = flickBullet.transform.position;
        flickBullet.flickAttack = this;
        flickBullet.maxTravelDistance = chargedDistance;
        flickBullet.gameObject.SetActive(true);
    }

    public override void HitSomething(GameObject thingThatGotHit)
    {
        //HURT IT!
        if (thingThatGotHit.GetComponent<Health>() != null)
        {
            Health health = thingThatGotHit.GetComponent<Health>();
            int damage = GetBonusDamage(chargeDamage);
            health.AdjustHp(-damage, gameObject);
            if (-damage < 0)
            {
                SFXPlayer.I.Play(attackData.playSFXOnHit);
            }
        }
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
        totalChargeTime = 0f;
        currentChargeTime = 0f;
        base.Cleanup();
    }
}