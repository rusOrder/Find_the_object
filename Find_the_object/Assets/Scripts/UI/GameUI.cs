using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("Основные элементы")]
    [SerializeField] private RectTransform itemsPanel;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;
    
    [Header("Префабы")]
    [SerializeField] private GameObject textItemPrefab;
    [SerializeField] private GameObject imageItemPrefab;
    
    [Inject] private IGameTimer _gameTimer;
    [Inject] private IItemManager _itemManager;

    private List<UIItemElement> _currentUIElements = new List<UIItemElement>();
    public event Action<string> OnItemClicked;
    
    public void Initialize(List<FindableItemData> items, bool useImages)
    {
        Debug.Log($"Initializing UI with {items.Count} items, useImages: {useImages}");
        
        ClearItems();
        
        foreach (var item in items)
        {
            AddItemToUI(item, useImages);
        }
        
        if (timerText != null)
        {
            UpdateTimerDisplay();
        }
    }
    
    private void AddItemToUI(FindableItemData item, bool useImage)
    {
        GameObject prefab = useImage ? imageItemPrefab : textItemPrefab;
        if (prefab == null)
        {
            Debug.LogError("UI item prefab is null!");
            return;
        }
        
        var instance = Instantiate(prefab, itemsPanel);
        var uiElement = instance.GetComponent<UIItemElement>();
        
        if (uiElement != null)
        {
            uiElement.Initialize(item, useImage);
            uiElement.OnClicked += () => 
            {
                Debug.Log($"UI element clicked: {item.id}");
                OnItemClicked?.Invoke(item.id);
            };
            _currentUIElements.Add(uiElement);
        }
    }
    public void RefreshItems(List<FindableItemData> currentItems, bool useImages)
    {
        Debug.Log($"Refreshing UI with {currentItems.Count} items");

        ClearItems();

        foreach (var item in currentItems)
        {
            AddItemToUI(item, useImages);
        }
    }
    public void RemoveItem(string itemId)
    {
        var element = _currentUIElements.Find(e => e.ItemId == itemId);
        if (element != null)
        {
            element.FadeOut(() =>
            {
                _currentUIElements.Remove(element);
                Destroy(element.gameObject);
            });
        }
    }
    
    public void AddNewItem(FindableItemData item, bool useImage)
    {
        AddItemToUI(item, useImage);
    }
    
    public void ShowWinScreen()
    {
        if (winScreen != null)
        {
            winScreen.SetActive(true);
            Debug.Log("Win screen shown");
        }
    }
    
    public void ShowLoseScreen()
    {
        if (loseScreen != null)
        {
            loseScreen.SetActive(true);
            Debug.Log("Lose screen shown");
        }
    }
    
    private void ClearItems()
    {
        foreach (var element in _currentUIElements)
        {
            Destroy(element.gameObject);
        }
        _currentUIElements.Clear();
    }
    
    private void Update()
    {
        if (_gameTimer != null && _gameTimer.IsRunning && timerText != null)
        {
            UpdateTimerDisplay();
        }
    }
    
    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(_gameTimer.RemainingTime / 60);
        int seconds = Mathf.FloorToInt(_gameTimer.RemainingTime % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}