using System;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private int quantity;

    public ItemData ItemData => itemData;
    public int Quantity => quantity;

    public InventoryItem(ItemData data, int amount)
    {
        itemData = data;
        quantity = Mathf.Max(0, amount);
    }

    public void Add(int amount)
    {
        quantity = Mathf.Max(0, quantity + amount);
    }

    public void Remove(int amount)
    {
        quantity = Mathf.Max(0, quantity - Mathf.Abs(amount));
    }
}
