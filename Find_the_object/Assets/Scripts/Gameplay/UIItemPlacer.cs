using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class UIItemPlacer : MonoBehaviour, IInitializable
{
    [System.Serializable]
    public class ItemPosition
    {
        public string itemId;
        public Vector2 anchoredPosition;
        public Vector2 size = new Vector2(100, 100);
        public float rotation = 0f;
    }
    
    [SerializeField] private RectTransform itemsContainer;
    [SerializeField] private GameObject uiItemPrefab;
    [SerializeField] private List<ItemPosition> itemPositions = new List<ItemPosition>();

    [Inject] private LevelConfig _levelConfig;
    [Inject] private DiContainer _container;
    [Inject] private IItemManager _itemManager;

    private Dictionary<string, UIItem> _spawnedItems = new Dictionary<string, UIItem>();

    public void Initialize()
    {
        itemsContainer = transform.parent.GetComponent<RectTransform>();
        SpawnAllItems();
        StartCoroutine(DelayedInitialization());
    }
    private System.Collections.IEnumerator DelayedInitialization()
    {
      yield return null;

      if (_itemManager != null)
      {
          var currentItems = _itemManager.CurrentItems;
          UpdateAllItemsInteractivity(currentItems);
      }
    }
    private void SpawnAllItems()
    {
        var enabledItems = _levelConfig.GetEnabledItems();
        
        if (enabledItems == null || enabledItems.Count == 0)
        {
            Debug.LogWarning("No enabled items found in LevelConfig");
            return;
        }
        
        Debug.Log($"Spawning {enabledItems.Count} items");
        
        foreach (var itemData in enabledItems)
        {
            var positionData = itemPositions.Find(p => p.itemId == itemData.id);
            if (positionData != null)
            {
                SpawnItem(itemData, positionData);
            }
        }
    }
    
    private void SpawnItem(FindableItemData itemData, ItemPosition position)
    {
        if (uiItemPrefab == null)
        {
            Debug.LogError("UIItem prefab is not assigned!");
            return;
        }
        
        var instance = _container.InstantiatePrefabForComponent<UIItem>(uiItemPrefab, itemsContainer);
        
        instance.Setup(itemData);


        var rectTransform = instance.GetComponent<RectTransform>();

        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = position.anchoredPosition;
        }

        instance.OnItemClicked += OnItemClicked;
        _spawnedItems[itemData.id] = instance;

        Debug.Log($"Spawned item: {itemData.id} at position: {position.anchoredPosition}");
    }

    private void OnItemClicked(FindableItemData itemData)
    {
        Debug.Log($"Item clicked in scene: {itemData.id}");
        if (_spawnedItems.TryGetValue(itemData.id, out var item))
        {
            _spawnedItems.Remove(itemData.id);
        }
    }
    
    public void RemoveItem(string itemId)
    {
        if (_spawnedItems.TryGetValue(itemId, out var item))
        {
            _spawnedItems.Remove(itemId);
        }
    }

    public void UpdateAllItemsInteractivity(List<FindableItemData> activeItems)
    {
      if (activeItems == null) return;

      var activeItemIds = new HashSet<string>(activeItems.Select(item => item.id));

        foreach (var kvp in _spawnedItems)
        {
            bool isActive = activeItemIds.Contains(kvp.Key);
            kvp.Value.UpdateInteractivity(isActive);

            Debug.Log($"Item {kvp.Key} interactivity: {isActive}");
        }
    }
}