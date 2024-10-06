using Unity.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour, IDamageable
{
    public Health thisHealth { get; set; }

    [SerializeField] private HandMovement handMovement;
    private PlayerInput PlayerInput { get; set; }

    [ReadOnly] public bool isAlive = true;

    private void Awake()
    {
        thisHealth = GetComponent<Health>();
        PlayerInput = GetComponent<PlayerInput>();
    }

    public void AdjustHealth(int amount)
    {
        thisHealth.AdjustHp(amount, this);
    }

    public void HandleDeath()
    {
        Debug.Log($"{gameObject.name} is handling death.");
        DisableInputs();
        isAlive = false;
    }

    public void DisableInputs()
    {
        PlayerInput.enabled = false;
        handMovement.enabled = false;
    }

    public void EnableInputs()
    {
        if (!isAlive) return; 
        PlayerInput.enabled = true;
        handMovement.enabled = true;
    }
}