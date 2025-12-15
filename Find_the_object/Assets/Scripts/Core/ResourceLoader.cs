using UnityEngine;
using Zenject;

public class ResourceLoader : IInitializable
{
    private readonly LevelConfig _levelConfig;
    
    [Inject]
    public ResourceLoader(LevelConfig levelConfig)
    {
        _levelConfig = levelConfig;
    }
    
    public void Initialize()
    {
        LoadAllResources();
    }
    
    private void LoadAllResources()
    {
        foreach (var item in _levelConfig.allItems)
        {
            Debug.Log("#   LoadAllResources   " + item.id+"   "+ item.uiSprite.name);
      /*
            if (item.uiSprite == null)
            {
                string spritePath = $"UI/{item.id}_icon";
                var sprite = Resources.Load<Sprite>(spritePath);
                if (sprite != null)
                {
                    item.uiSprite = sprite;
                }
            }*/

            /*
            if (item.prefab == null)
            {
                string prefabPath = $"Prefabs/FindableItems/{item.id}";
                var prefab = Resources.Load<GameObject>(prefabPath);
                if (prefab != null)
                {
                    item.prefab = prefab;
                }
            }*/
        }
    }
}