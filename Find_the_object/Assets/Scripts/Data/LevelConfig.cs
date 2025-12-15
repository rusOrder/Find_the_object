using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "HiddenTest/LevelConfig")]
public class LevelConfig : ScriptableObject
{
    [Header("Настройки уровня")]
    public List<FindableItemData> allItems = new List<FindableItemData>();
    public bool useImagesInsteadOfText = false;
    public bool useTimer = true;
    public float timerDuration = 120f;
    public int maxConcurrentItems = 3;

    [Header("Порядок предметов")]
    public List<string> itemOrder = new List<string>();

    public List<FindableItemData> GetEnabledItems()
    {
        var enabledItems = new List<FindableItemData>();
        
        if (itemOrder.Count > 0)
        {
            foreach (var itemId in itemOrder)
            {
                var item = allItems.Find(i => i.id == itemId && i.isEnabled);
                if (item != null) enabledItems.Add(item);
            }
        }
        else
        {
            enabledItems = allItems.FindAll(i => i.isEnabled);
        }

        return enabledItems;
    }
}