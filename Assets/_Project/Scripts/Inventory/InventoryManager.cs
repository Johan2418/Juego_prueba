using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private List<InventoryItem> startingItems = new List<InventoryItem>();
    [SerializeField] private bool logChanges = true;

    public static InventoryManager Instance { get; private set; }
    public IReadOnlyList<InventoryItem> Items => items;

    public event Action<ItemData, int> ItemAdded;
    public event Action<ItemData, int> ItemRemoved;

    private readonly List<InventoryItem> items = new List<InventoryItem>();
    private readonly Dictionary<string, InventoryItem> itemsById = new Dictionary<string, InventoryItem>(StringComparer.OrdinalIgnoreCase);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        RebuildInventoryCache();
    }

    public bool AddItem(ItemData itemData, int amount = 1)
    {
        if (!IsValidItem(itemData) || amount <= 0)
        {
            return false;
        }

        if (!itemsById.TryGetValue(itemData.ItemId, out InventoryItem entry))
        {
            entry = new InventoryItem(itemData, 0);
            itemsById[itemData.ItemId] = entry;
            items.Add(entry);
        }

        entry.Add(amount);

        if (logChanges)
        {
            Debug.Log($"Inventory +{amount} {itemData.DisplayName} (Total: {entry.Quantity})");
        }

        QuestManager.Instance?.SubmitEvent(QuestEvent.ForItemCollected(itemData.ItemId, amount));
        ItemAdded?.Invoke(itemData, amount);
        return true;
    }

    public bool RemoveItem(string itemId, int amount = 1)
    {
        if (string.IsNullOrWhiteSpace(itemId) || amount <= 0 || !itemsById.TryGetValue(itemId, out InventoryItem entry))
        {
            return false;
        }

        if (entry.Quantity < amount)
        {
            return false;
        }

        entry.Remove(amount);
        ItemRemoved?.Invoke(entry.ItemData, amount);

        if (entry.Quantity <= 0)
        {
            itemsById.Remove(itemId);
            items.Remove(entry);
        }

        return true;
    }

    public int GetQuantity(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId) || !itemsById.TryGetValue(itemId, out InventoryItem entry))
        {
            return 0;
        }

        return entry.Quantity;
    }

    private void RebuildInventoryCache()
    {
        items.Clear();
        itemsById.Clear();

        for (int i = 0; i < startingItems.Count; i++)
        {
            InventoryItem startItem = startingItems[i];
            if (startItem == null || !IsValidItem(startItem.ItemData) || startItem.Quantity <= 0)
            {
                continue;
            }

            InventoryItem runtimeEntry = new InventoryItem(startItem.ItemData, startItem.Quantity);
            items.Add(runtimeEntry);
            itemsById[startItem.ItemData.ItemId] = runtimeEntry;
        }
    }

    private static bool IsValidItem(ItemData itemData)
    {
        return itemData != null && !string.IsNullOrWhiteSpace(itemData.ItemId);
    }
}
