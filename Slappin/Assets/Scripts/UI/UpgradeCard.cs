using UnityEngine;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour
{
    [SerializeField] private GameObject border;
    [SerializeField] private Image shadowImage;
    
    [SerializeField] private Color shadowDefault;
    [SerializeField] private Color shadowSelected;
    [SerializeField] private Color shadowPressed;

    [SerializeField] private RectTransform cardBodyRect;

    private Vector2 defaultPosition;
    
    private void Awake()
    {
        shadowDefault = shadowImage.color;
        defaultPosition = cardBodyRect.anchoredPosition;
    }

    public void OnDefault()
    {
        border.gameObject.SetActive(false);
        shadowImage.color = shadowDefault;
        cardBodyRect.anchoredPosition = defaultPosition;
    }

    public void OnSelected()
    {
        border.gameObject.SetActive(true);
        shadowImage.color = shadowSelected;
        cardBodyRect.anchoredPosition = defaultPosition;
    }

    public void OnPressed()
    {
        cardBodyRect.anchoredPosition = new Vector2(10f, -10f);
        border.gameObject.SetActive(true);
        shadowImage.color = shadowPressed;
    }
}