using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; 
using Zenject;

public class GameController : MonoBehaviour
{
    [Inject] private IItemManager _itemManager;
    [Inject] private GameUI _gameUI;
	  [Inject] private IGameTimer _gameTimer;
    [Inject] private LevelConfig _levelConfig;
    [Inject] private SignalBus _signalBus;
    [Inject] private UIItemPlacer _uiItemPlacer;


  private void Awake()
    {
        _signalBus.Subscribe<GameInitializedSignal>(OnGameInitialized);
    }

    private void OnGameInitialized(GameInitializedSignal signal)
    {
        Debug.Log($"Signal received! Items: {signal.ItemCount}");
        SetupGame();
    }

    private void SetupGame()
	  {
		    Debug.Log("Setting up game...");
		
		    try
		    {
			      var currentItems = _itemManager.CurrentItems;
			      Debug.Log($"Current items count: {currentItems.Count}");

			      _gameUI.Initialize(currentItems, _levelConfig.useImagesInsteadOfText);

            if (_uiItemPlacer != null)
            {
                _uiItemPlacer.UpdateAllItemsInteractivity(currentItems);
            }

            _itemManager.OnAllItemsFound += OnAllItemsFound;
			      _gameUI.OnItemClicked += OnUIItemClicked;
            _itemManager.OnCurrentItemsUpdated += OnCurrentItemsUpdated;


            if (_levelConfig.useTimer)
			      {
				        _gameTimer.StartTimer(_levelConfig.timerDuration);
				        _gameTimer.OnTimeExpired += OnTimeExpired;
				        Debug.Log($"Timer started: {_levelConfig.timerDuration} seconds");
			      }
            Debug.Log("Game setup complete!");
		    }
		    catch (System.Exception e)
		    {
			      Debug.LogError($"Error during game setup: {e.Message}\n{e.StackTrace}");
		    }
	  }

    private void OnUIItemClicked(string itemId)
    {
        Debug.Log($"Ищите предмет: {itemId}");
        if (_uiItemPlacer != null)
        {
            _uiItemPlacer.UpdateAllItemsInteractivity(_itemManager.CurrentItems);
        }
    }
    
    private void OnAllItemsFound()
    {
        _gameTimer.StopTimer();
        _gameUI.ShowWinScreen();
    }
    
    private void OnTimeExpired()
    {
        _gameUI.ShowLoseScreen();
    }



    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Debug.Log("Esc pressed - quitting...");
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }

    private void OnCurrentItemsChanged(CurrentItemsChangedSignal signal)
    {
      Debug.Log($"Current items changed. Current: {signal.CurrentItems.Count}, Available: {signal.AvailableItems.Count}");

      if (_uiItemPlacer != null)
      {
          _uiItemPlacer.UpdateAllItemsInteractivity(signal.CurrentItems);
      }
    }
    private void OnCurrentItemsUpdated(List<FindableItemData> currentItems)
    {
        Debug.Log($"Current items updated: {currentItems.Count} items");
        _gameUI.RefreshItems(currentItems, _levelConfig.useImagesInsteadOfText);

        if (_uiItemPlacer != null)
        {
            _uiItemPlacer.UpdateAllItemsInteractivity(currentItems);
        }
    }
    private void OnDestroy()
    {
        _signalBus.TryUnsubscribe<GameInitializedSignal>(OnGameInitialized);
        _signalBus.TryUnsubscribe<CurrentItemsChangedSignal>(OnCurrentItemsChanged);
        if (_itemManager != null)
        {
             _itemManager.OnAllItemsFound -= OnAllItemsFound;
             _itemManager.OnCurrentItemsUpdated -= OnCurrentItemsUpdated;
         }
        
        if (_gameTimer != null)
        {
            _gameTimer.OnTimeExpired -= OnTimeExpired;
        }
    }
}