using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LegendItem : MonoBehaviour
{
    [SerializeField] private Image colorImage;
    [SerializeField] private TextMeshProUGUI label;

    public void Setup(Color color, string text)
    {
        colorImage.color = color;
        label.text = text;
    }

}
