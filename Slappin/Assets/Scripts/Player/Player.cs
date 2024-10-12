using System;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour, IHpAdjustmentListener
{
    public Health thisHealth { get; set; }

    [SerializeField] private HandMovement handMovement;
    private PlayerInput PlayerInput { get; set; }
    private PlayerState currentState { get; set; }

    private void Awake()
    {
        thisHealth = GetComponent<Health>();
        PlayerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        thisHealth.Initialize();
        HpBar.I.UpdateHpBar(thisHealth);
        currentState = new StateDefault(this);
    }

    private void Update()
    {
        currentState?.Update(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        currentState?.FixedUpdate(Time.fixedDeltaTime);
    }

    private void SetState(PlayerState newState)
    {
        if (newState == null)
        {
            Debug.LogError("Tried to set player to a null state");
        }
        
        PlayerState oldState = currentState;
        currentState?.Exit(newState);
        currentState = newState;
        currentState?.Enter(oldState);
        
        Debug.Log($"Switching state from: {oldState?.state}\nTo: {newState.state}");
    }

    public void DisableInputs()
    {
        PlayerInput.enabled = false;
        handMovement.enabled = false;
    }

    public void EnableMovement()
    {
        handMovement.enabled = true;
    }

    public void EnablePlayerAttacks()
    {
        PlayerInput.enabled = true;
    }

    public void TookDamage(int damageAmount, GameObject attacker)
    {
        Debug.Log("player took damage");
        CameraShake.I.StartCameraShake(0.013f, 0.13f);
        HpBar.I.UpdateHpBar(thisHealth);
        //player needs some sort of feedback of getting hurt
        //sfx
        //visual - vfx on the screen of ouchies? (based on the attacker)

    }

    public void Healed(int healAmount, GameObject healer)
    {
        HpBar.I.UpdateHpBar(thisHealth);
    }

    public float HandleDeath(int lastAttack, GameObject killer)
    {
        
        DisableInputs();

        //TODO:: Use this for dying
        float deathAnimationTime = 0f;
        return deathAnimationTime;
    }
}