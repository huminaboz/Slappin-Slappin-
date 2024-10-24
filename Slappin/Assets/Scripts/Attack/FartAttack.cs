using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FartAttack : MonoBehaviour
{
    public static Action<float, float> OnFart;
    [SerializeField] private float fartKnockback = 40f; 

    [SerializeField] private Image cooldownMeter;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color fartReadyColor = Color.white;
    [SerializeField] private TextMeshProUGUI fartText;
    
    
    private float fartCooldown;
    private float fartDamage;
    private float currentCooldown;

    private void OnEnable()
    {
        UpgradeData.OnPurchaseMade += UpdateFartStats;
    }

    private void OnDisable()
    {
        UpgradeData.OnPurchaseMade -= UpdateFartStats;
    }


    private void UpdateFartStats()
    {
        fartCooldown = StatLiason.I.Get(Stat.FartCooldown);
        fartDamage = StatLiason.I.Get(Stat.FartDamage);
    }

    private float GetCooldownRatio()
    {
        return currentCooldown / fartCooldown;
    }

    private void Update()
    {
        if (UIStateSwapper.I.currentUIState != UIStateSwapper.UIState.playing) return;
        
        if (currentCooldown < fartCooldown)
        {
            currentCooldown += Time.deltaTime;
            currentCooldown = Mathf.Clamp(currentCooldown, 0f, fartCooldown);
            cooldownMeter.fillAmount = GetCooldownRatio();
            cooldownMeter.color = fartReadyColor;

            if (currentCooldown >= fartCooldown)
            {
                fartText.text = "Fart";
            }
            
            return;
        }

        if (Input.GetAxis("LTrigger") > 0f)
        {
            UpdateFartStats();
            OnFart?.Invoke(fartDamage, fartKnockback);
            currentCooldown = 0f;
            cooldownMeter.color = defaultColor;
            fartText.text = "...";
            SFXPlayer.I.Play(AudioEventsStorage.I.farted);
        }
    }
}