using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : Singleton<HpBar>
{
    private Slider _hpBar;
    [SerializeField] private TextMeshProUGUI hpText;

    private void Awake()
    {
        _hpBar = GetComponent<Slider>();
    }

    public void UpdateHpBar(Health health)
    {
        if (!health) return;
        hpText.text = health.hp.ToString();
        _hpBar.value = (float)health.hp / health.maxHp;
    }
}