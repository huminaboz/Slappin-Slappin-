using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour, IDamageable
{
    public Health thisHealth { get; set; }

    [SerializeField] private HandMovement handMovement;
    private PlayerInput PlayerInput { get; set; }

    private void Awake()
    {
        thisHealth = GetComponent<Health>();
        PlayerInput = GetComponent<PlayerInput>();
    }

    public void AdjustHealth(int amount)
    {
    }

    public void HandleDeath()
    {
        DisableInputs();
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
}