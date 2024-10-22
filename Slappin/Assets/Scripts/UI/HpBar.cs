using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : Singleton<HpBar>
{
    private Slider _hpBar;
    [SerializeField] private TextMeshProUGUI hpText;

    [SerializeField] private Color defaulthpBarColor;
    [SerializeField] private Color dangerColor;

    [SerializeField] private Image hpFillImage;

    private void Awake()
    {
        _hpBar = GetComponent<Slider>();
    }

    public void UpdateHpBar(Health health)
    {
        if (!health) return;
        int hp = Mathf.Clamp(health.hp, 0, health.hp);
        hpText.text = hp.ToString();
        _hpBar.value = (float)health.hp / health.maxHp;
        if (_hpBar.value <= .25f)
        {
            hpFillImage.color = dangerColor;
            hpText.color = dangerColor;
        }
        else
        {
            hpFillImage.color = defaulthpBarColor;
            hpText.color = defaulthpBarColor;
        }
    }
}