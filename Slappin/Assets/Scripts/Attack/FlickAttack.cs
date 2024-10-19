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
    
    //Flick forecast
    [SerializeField] private GameObject forecastCube;
    [SerializeField] private GameObject forecastCubePositioner;
    [SerializeField] private Renderer handShadowRenderer;
    
    private Action _currentAction;
    private SO_AttackData_Flick _flickData;
    
    //Charging
    private ObjectShake _shake;
    private AnimationCurve chargeCurve;
    private int chargeDamage;
    private float chargedDistance = 1f;
    private float maxChargeTime;
    private float _currentChargeTime = 0f;
    private readonly float chargeTickRate = .25f;
    private float _totalChargeTime = 0f;
    
    //Camera
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
        chargeFrame.SetActive(false);
        hitFrame.SetActive(false);
        AdjustForecastScale(_flickData.distanceBase);
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
        maxChargeTime = StatLiason.I.Get(Stat.FlickMaxChargeTime);
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
        
        //Increase the FoV on the camera so you can see from farther back
        // DOTween.To(() => _camera.fieldOfView, 
        //     x => _camera.fieldOfView = x, 
        //     65f, .4f).SetEase(Ease.OutQuad);

        //Setup forecast
        AdjustForecastScale(_flickData.distanceBase);
        //forecastCube.gameObject.SetActive(true);
        handShadowRenderer.enabled = false;
    }

    private void AdjustForecastScale(float zDistance)
    {
        //Only needed this when the parent object was a different scale 
        // const float defaultZScaleOfCube = 1f;
        // zDistance *= defaultZScaleOfCube / flickBulletSpawnPoint.localScale.z; 
        forecastCubePositioner.transform.localPosition =  new Vector3(
            forecastCubePositioner.transform.localPosition.x,
            forecastCubePositioner.transform.localPosition.y, 
            zDistance*.5f);
        forecastCube.transform.localScale = new Vector3(
            forecastCube.transform.localScale.x,
            forecastCube.transform.localScale.y, 
            zDistance);
    }


    private void ChargeAttack()
    {
        _shake.StartShake();

        //Increase charge
        _currentChargeTime += Time.deltaTime;
        _totalChargeTime += Time.deltaTime;
        if (_currentChargeTime > chargeTickRate && _totalChargeTime < maxChargeTime)
        {
            _currentChargeTime = 0f;
        }
        float ratio = _totalChargeTime / maxChargeTime;
        
        //Set up the forecast cube
        //As you charge, the forecast on the ground grows longer/wider relative to the charge
        chargedDistance = _flickData.distanceBase + _flickData.distanceBase 
                           * _flickData.distanceMaxMultiplier * chargeCurve.Evaluate(ratio);
        // Debug.LogWarning($"Charged Distance {chargedDistance}");
        AdjustForecastScale(chargedDistance);
        
        //Alter the color based on the charge
        Color chargeColor = Color.Lerp(_defaultTopOfHandColor, 
            Color.magenta, chargeCurve.Evaluate(ratio));
        handRenderer.material.SetColor("_ColorDim", chargeColor);
        
        if (!Input.GetButton("Fire3"))
        {
            ReleaseCharge(ratio);
            _currentAction = null;
        }
    }

    private void ReleaseCharge(float ratio)
    {
        Debug.Log($"Charge Ratio: {ratio}");
        chargeDamage = _flickData.baseDamage + (int) (_flickData.baseDamage * 
                        StatLiason.I.Get(Stat.FlickMaxChargeDamage) * chargeCurve.Evaluate(ratio));
        Debug.Log($"Max charge damage {_flickData.baseDamage} x {StatLiason.I.Get(Stat.FlickMaxChargeDamage)}x" +
                         $"\n Charged Damage: {chargeDamage}");
        //TODO:: calculate attack width
     
        //Adjust the field of width back to normal
        // DOTween.To(() => _camera.fieldOfView, 
        //     x => _camera.fieldOfView = x, 
        //     startingFoV, .25f).SetEase(Ease.OutQuad);
        
        //Do visuals
        flickParticle.Play();
        SpawnFlickBullet();
        chargeFrame.SetActive(false);
        hitFrame.SetActive(true);

        //Cleanup
        player.DisableMovement();
        _shake.StopShake();
        // forecastCube.gameObject.SetActive(false);
        AdjustForecastScale(_flickData.distanceBase);
        handShadowRenderer.enabled = true;
        RestoreDefaultAppearance();
        InitiateTravelBackUp();
    }

    private void SpawnFlickBullet()
    {
        SFXPlayer.I.Play(AudioEventsStorage.I.releasedFlick);
        
        FlickBullet flickBullet = ObjectPoolManager<FlickBullet>.GetObject(flickBulletPrefab);
        
        if (flickBullet is null) return;
        flickBullet.transform.position = flickBulletSpawnPoint.position;
        //Spawn position was getting set to the position before it gets reset - had to set it here
        flickBullet.spawnPosition = flickBulletSpawnPoint.position;
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
            float damage = GetLuckDamage(chargeDamage);
            health.AdjustHp((int)-damage, gameObject);
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
        _totalChargeTime = 0f;
        _currentChargeTime = 0f;
        chargeFrame.SetActive(false);
        hitFrame.SetActive(false);
        base.Cleanup();
    }
}