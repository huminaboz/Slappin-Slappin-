using System;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Player : MonoBehaviour, IHpAdjustmentListener
{
    public Health thisHealth { get; set; }

    [SerializeField] public HandMovement handMovement;
    private PlayerState CurrentState { get; set; }

    public AttackType CurrentAttackType { get; set; }
    [SerializeField] public SlapAttack slapAttack;
    [SerializeField] public FlickAttack flickAttack;
    [SerializeField] public SquishAttack squishAttack;
    [SerializeField] public WildCardAttack wildCardAttack;

    [Header("Positioning Stuff")]
    [SerializeField] private Transform handShadowTransform;
    private Transform handPositioner;


    private void Awake()
    {
        thisHealth = GetComponent<Health>();
        slapAttack.Initialize();
        flickAttack.Initialize();
        squishAttack?.Initialize();
        wildCardAttack?.Initialize();
    }

    private void Start()
    {
        thisHealth.Initialize();
        HpBar.I.UpdateHpBar(thisHealth);
        SetState(new StateDefault(this));
    }

    private void Update()
    {
        CurrentState?.Update(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        CurrentState?.FixedUpdate(Time.fixedDeltaTime);

        CurrentAttackType?.SetParentPosition();
    }

    public void SetState(PlayerState newState)
    {
        if (newState == null)
        {
            Debug.LogError("Tried to set player to a null state");
        }
        
        PlayerState oldState = CurrentState;
        CurrentState?.Exit(newState);
        CurrentState = newState;
        CurrentState?.Enter(oldState);
        
        Debug.Log($"Switching state from: <color=yellow>{oldState?.state}</color>" +
                  $" To: <color=green>{newState.state}</color>");
    }

    public void DisableMovement()
    {
        handMovement.enabled = false;
    }

    public void EnableMovement()
    {
        handMovement.enabled = true;
    }

    public void TookDamage(int damageAmount, GameObject attacker)
    {
        
        // Debug.Log("player took damage");
        SFXPlayer.I.Play(AudioEventsStorage.I.playerTookDamage);
        //TODO:: Set the values of the camera shake based on the attacker
        CameraShake.I.StartCameraShake(0.013f, 0.13f);
        HpBar.I.UpdateHpBar(thisHealth);
        //visual - vfx on the screen of ouchies? (based on the attacker-no scope for this boy)
    }

    public void Healed(int healAmount, GameObject healer)
    {
        HpBar.I.UpdateHpBar(thisHealth);
    }

    public float HandleDeath(int lastAttack, GameObject killer)
    {
        DisableMovement();
        SFXPlayer.I.Play(AudioEventsStorage.I.playerDied);

        //TODO:: Use this for dying
        float deathAnimationTime = 0f;
        return deathAnimationTime;
    }
}