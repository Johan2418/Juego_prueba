using UnityEngine;

[CreateAssetMenu(menuName = "CozyGameplay/Items/Item Data", fileName = "ItemData")]
public class ItemData : ScriptableObject
{
    [SerializeField] private string itemId;
    [SerializeField] private string displayName;
    [TextArea(2, 4)]
    [SerializeField] private string description;

    public string ItemId => itemId;
    public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
    public string Description => description;

    protected virtual void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            itemId = name.ToLowerInvariant().Replace(" ", "_");
        }
    }
}
