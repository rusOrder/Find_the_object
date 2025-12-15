using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIItemElement : MonoBehaviour
{
    [SerializeField] private TMP_Text textElement;
    [SerializeField] private Image imageElement;
    [SerializeField] private Button button;
    
    private CanvasGroup _canvasGroup;
    public string ItemId { get; private set; }
    public event Action OnClicked;
    
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        button.onClick.AddListener(() => OnClicked?.Invoke());
    }
    
    public void Initialize(FindableItemData item, bool useImage)
    {
        ItemId = item.id;
        
        if (useImage && item.uiSprite != null && imageElement != null)
        {
            imageElement.sprite = item.uiSprite;
            imageElement.gameObject.SetActive(true);
            if (textElement != null) textElement.gameObject.SetActive(false);
        }
        else if (textElement != null)
        {
            textElement.text = item.displayName;
            textElement.gameObject.SetActive(true);
            if (imageElement != null) imageElement.gameObject.SetActive(false);
        }
    }
    
    public void FadeOut(Action onComplete = null)
    {
        if (_canvasGroup != null)
        {
            StartCoroutine(FadeCoroutine(onComplete));
        }
        else
        {
            onComplete?.Invoke();
        }
    }
    
    private System.Collections.IEnumerator FadeCoroutine(Action onComplete)
    {
        float duration = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }
        
        onComplete?.Invoke();
    }
}