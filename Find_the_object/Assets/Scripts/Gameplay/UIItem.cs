using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class UIItem : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Image itemImage;
    [SerializeField] private CanvasGroup canvasGroup;
    
    private FindableItemData _itemData;
    private RectTransform _rectTransform;
    private bool _isClickable = true;
    
    public event Action<FindableItemData> OnItemClicked;

    [Inject] private IItemManager _itemManager;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        
        if (itemImage != null)
        {
            itemImage.raycastTarget = true;
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        HandleClick();
    }
    
    private void HandleClick()
    {
        if (!_isClickable || _itemData == null) return;

        if (!_itemManager.IsItemActive(_itemData.id))
        {
          Debug.Log($"Item {_itemData.id} is not active, ignoring click");
          return;
        }

        Debug.Log($"UIItem clicked: {_itemData.id}");

        StartCoroutine(CollectAnimation());

        OnItemClicked?.Invoke(_itemData);
        _itemManager.ItemFound(_itemData.id);
    }

    public void UpdateInteractivity(bool isActive)
    {
      if (itemImage != null)
      {
        itemImage.raycastTarget = isActive;
      }

      Debug.Log($"Item {_itemData?.id} interactivity set to: {isActive}");
    }

    private System.Collections.IEnumerator CollectAnimation()
    {
        _isClickable = false;
        
        float duration = 0.5f;
        float elapsed = 0f;
        Vector2 startScale = _rectTransform.localScale;
        Vector2 startPosition = _rectTransform.anchoredPosition;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            _rectTransform.localScale = Vector2.Lerp(startScale, Vector2.zero, t);
            
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            }
            Vector2 newPosition = startPosition;
            newPosition.y += 50f * t;
            _rectTransform.anchoredPosition = newPosition;
            
            yield return null;
        }
        gameObject.SetActive(false);
    }

    public void Setup(FindableItemData itemData)
    {
        _itemData = itemData;
        if (itemImage != null && itemData.sceneSprite != null)
        {
            itemImage.sprite = itemData.sceneSprite;
            //itemImage.color = itemData.tintColor;
            itemImage.SetNativeSize();
            Debug.Log($"UIItem {itemData.id} setup with sprite: {itemData.uiSprite.name}");
        }

        if (!string.IsNullOrEmpty(itemData.displayName))
        {
            gameObject.name = $"UIItem_{itemData.displayName}";
        }
    }
}