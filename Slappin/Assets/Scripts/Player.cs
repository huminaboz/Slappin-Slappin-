using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour, IHpAdjustmentListener
{
    public Health thisHealth { get; set; }

    [SerializeField] private HandMovement handMovement;
    private PlayerInput PlayerInput { get; set; }

    private void Awake()
    {
        thisHealth = GetComponent<Health>();
        PlayerInput = GetComponent<PlayerInput>();
    }

    public void DisableInputs()
    {
        PlayerInput.enabled = false;
        handMovement.enabled = false;
    }

    public void EnableInputs()
    {
        PlayerInput.enabled = true;
        handMovement.enabled = true;
    }

    public void TookDamage(int damageAmount, GameObject attacker)
    {
    }

    public void Healed(int healAmount, GameObject healer)
    {
    }

    public float HandleDeath(int lastAttack, GameObject killer)
    {
        
        DisableInputs();

        //TODO:: Use this for dying
        float deathAnimationTime = 0f;
        return deathAnimationTime;
    }
}