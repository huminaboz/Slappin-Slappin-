using UnityEngine;

[RequireComponent(typeof(Health))]
public class Player : MonoBehaviour, IHpAdjustmentListener
{
    public Health thisHealth { get; set; }

    [SerializeField] private HandMovement handMovement;
    private PlayerState CurrentState { get; set; }

    public AttackType CurrentAttackType { get; set; }
    [SerializeField] public SlapAttack slapAttack;

    private void Awake()
    {
        thisHealth = GetComponent<Health>();
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
        
        Debug.Log($"Switching state from: <color=red>{oldState?.state}</color>" +
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
        Debug.Log("player took damage");
        float stunTime = 0f; //Not everything will stun you??
        if (attacker.GetComponent<Enemy_Spike>() is not null)
        {
            Enemy_Spike spike = attacker.GetComponent<Enemy_Spike>();
            stunTime = spike.handStabStunDuration;
        }
        
        SetState(new StateDamagedState(this));
        
        //TODO:: Set the values of the camera shake based on the attacker
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
        DisableMovement();

        //TODO:: Use this for dying
        float deathAnimationTime = 0f;
        return deathAnimationTime;
    }
}