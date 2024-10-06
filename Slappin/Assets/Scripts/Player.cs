using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerMovement))]
public class Player : MonoBehaviour, IDamageable
{
    public Health thisHealth { get; set; }

    private PlayerMovement PlayerMovement { get; set; }
    private PlayerInput PlayerInput { get; set; }
    private void Awake()
    {
        thisHealth = GetComponent<Health>();
        
    }

    public void AdjustHealth(int amount) { }

    public void HandleDeath()
    {
        PlayerInput.enabled = false;
        PlayerMovement.enabled = false;
    }
}
