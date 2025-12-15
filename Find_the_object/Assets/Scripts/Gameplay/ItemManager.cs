using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class GameInitializedSignal
{
    public int ItemCount { get; }

    public GameInitializedSignal(int itemCount)
    {
        ItemCount = itemCount;
    }
}

public class CurrentItemsChangedSignal
{
    public List<FindableItemData> CurrentItems { get; }
    public List<FindableItemData> AvailableItems { get; }
    public CurrentItemsChangedSignal(List<FindableItemData> currentItems, List<FindableItemData> availableItems)
    {
        CurrentItems = currentItems;
        AvailableItems = availableItems;
    }
}

public interface IItemManager
{
    List<FindableItemData> CurrentItems { get; }
    void ItemFound(string itemId);
    event Action OnAllItemsFound;

    bool IsItemActive(string itemId);
    event Action<List<FindableItemData>> OnCurrentItemsUpdated;
}

public class ItemManager : IItemManager, IInitializable
{
    private readonly SignalBus _signalBus;

    private readonly LevelConfig _levelConfig;
    private List<FindableItemData> _availableItems;
    private List<FindableItemData> _currentItems = new List<FindableItemData>();
    
    public List<FindableItemData> CurrentItems => _currentItems;
    public event Action OnAllItemsFound;
    public event Action OnCurrentItemsChanged;
    public event Action<List<FindableItemData>> OnCurrentItemsUpdated;

    [Inject]
    public ItemManager(SignalBus signalBus, LevelConfig levelConfig)
    {
        _signalBus = signalBus;
        _levelConfig = levelConfig;
    }
    
    public void Initialize()
    {
        _availableItems = _levelConfig.GetEnabledItems();
        RefreshCurrentItems();
        _signalBus.Fire(new GameInitializedSignal(_currentItems.Count));
    }
    public bool IsItemActive(string itemId)
    {
      return _currentItems.Any(item => item.id == itemId);
    }

    private void RefreshCurrentItems()
    {
        _currentItems.Clear();
        
        int itemsToAdd = Mathf.Min(_levelConfig.maxConcurrentItems, _availableItems.Count);
        for (int i = 0; i < itemsToAdd; i++)
        {
            _currentItems.Add(_availableItems[i]);
        }
    }
    
    public void ItemFound(string itemId)
    {
        Debug.Log($"Item found: {itemId}");

        var foundItem = _availableItems.FirstOrDefault(item => item.id == itemId);
        if (foundItem != null)
        {
            _availableItems.Remove(foundItem);
            _currentItems.RemoveAll(item => item.id == itemId);
            if (_availableItems.Count > 0 && _currentItems.Count < _levelConfig.maxConcurrentItems)
            {
                foreach (var item in _availableItems)
                {
                    if (!_currentItems.Any(i => i.id == item.id))
                    {
                        _currentItems.Add(item);
                        break;
                    }
                }
            }
            
            Debug.Log($"Items remaining: {_availableItems.Count}, Current shown: {_currentItems.Count}");

            _signalBus.Fire(new CurrentItemsChangedSignal(new List<FindableItemData>(_currentItems),new List<FindableItemData>(_availableItems)));
            OnCurrentItemsUpdated?.Invoke(new List<FindableItemData>(_currentItems));

            if (_availableItems.Count == 0 && _currentItems.Count == 0)
            {
                Debug.Log("All items found!");
                OnAllItemsFound?.Invoke();
            }
        }
        else
        {
            Debug.LogWarning($"Item {itemId} not found in available items");
        }
    }
}