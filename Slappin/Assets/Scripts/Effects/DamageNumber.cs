using UnityEngine;
using DG.Tweening;
using TMPro;

public class DamageNumber : MonoBehaviour, IObjectPool<DamageNumber>
{
    [SerializeField] private float yStartingOffset = 0.2f;
    [SerializeField] private float distanceToMoveUp = 1f;
    [SerializeField] private float moveUpTime = 1f;
    
    private TextMeshPro textMesh;
    private RectTransform rectTransform;
    
    public void SetupObjectFirstTime()
    {
        gameObject.SetActive(false);
        textMesh = GetComponent<TextMeshPro>();
        rectTransform = GetComponent<RectTransform>();
    }
    
    public void InitializeObjectFromPool()
    {
        textMesh.text = string.Empty;
        gameObject.SetActive(true);
    }
    
    
    public void Spawn(int damage, Vector3 startPosition)
    {
        textMesh.text = Mathf.Abs(damage).ToString();
        startPosition += Vector3.up * yStartingOffset;
        transform.position = startPosition;
        
        transform.DOMoveY(transform.position.y + distanceToMoveUp, moveUpTime*2f)
            .SetDelay(moveUpTime*.25f)
            .SetEase(Ease.OutCubic);

        Color currentColor = textMesh.color;

        DOTween.To(() => textMesh.color.a, x =>
            {
                Color newColor = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, x);
                textMesh.color = newColor;
            },
            currentColor.a, moveUpTime*2f)
            .SetDelay(moveUpTime*.25f)
            .SetEase(Ease.Linear)
            .OnComplete(() => { ReturnObjectToPool(); });
    }


    public void ReturnObjectToPool()
    {
        ObjectPoolManager<DamageNumber>.ReturnObject(this);
    }
}