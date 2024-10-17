using UnityEngine;
using UnityEngine.UI;

public class ButtonAppearance : MonoBehaviour
{
    [SerializeField] private Image border;

    [SerializeField] private Color borderSelected;
    [SerializeField] private Color borderPressed;

    [SerializeField] private RectTransform buttonBody;
    [SerializeField] private RectTransform buttonShadowRect;
    
    private Vector2 defaultPosition;

    private void Awake()
    {
        defaultPosition = buttonBody.anchoredPosition;
    }

    private void Start()
    {
        border.gameObject.SetActive(false);
    }

    public void OnDefault()
    {
        border.gameObject.SetActive(false);
        buttonBody.anchoredPosition = defaultPosition;
    }

    public void OnSelected()
    {
        border.gameObject.SetActive(true);
        border.color = borderSelected;
        buttonBody.anchoredPosition = defaultPosition;
    }

    public void OnPressed()
    {
        buttonBody.anchoredPosition = buttonShadowRect.anchoredPosition;
        border.gameObject.SetActive(true);
        border.color = borderPressed;
    }
}