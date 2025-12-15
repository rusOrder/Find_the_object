using UnityEngine;

[System.Serializable]
public class FindableItemData
{
    public string id;
    public string displayName;
    public Sprite uiSprite;
    public Sprite sceneSprite;
    public bool isEnabled = true;

    public Vector2 uiSize = new Vector2(100, 100);
    public Color tintColor = Color.white;
}