using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Inventory
{
    public Action OnInventoryChanged;
    
    public List<InventoryItem> Items => items;
    [SerializeField] private List<InventoryItem> items = new();

    public int MaxSlots => maxSlots;
    private int maxSlots;

    public Inventory(int maxSlots)
    {
        this.maxSlots = maxSlots;
    }
    public Inventory(Inventory copyFrom)
    {
        maxSlots = copyFrom.MaxSlots;
        items.AddRange(copyFrom.Items);
    }

    public bool AddItem(InventoryItem item)
    {
        if (!InventoryHasSpaceFor(item, out InventoryItem foundReference))
            return false;
        if (foundReference != null)
            foundReference.AddCount(item.Count);
        else
            items.Add(item);
        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool TransferItemTo(InventoryItem item, Inventory inventory)
    {
        if (!items.Contains(item)) return false;
        if (!inventory.AddItem(item)) return false;
        items.Remove(item);
        OnInventoryChanged?.Invoke();
        return true;
    }

    private InventoryItem GetItemWithStackSpace(InventoryItem itemToAdd)
    {
        foreach (var item in items)
        {
            if (item.ID == itemToAdd.ID && item.Count < item.MaxStack) return item;
        }
        return null;
    }

    private bool InventoryHasSpaceFor(InventoryItem item, out InventoryItem foundReference)
    {
        foundReference = GetItemWithStackSpace(item);
        bool noReferenceButHasSpace = foundReference == null && items.Count < maxSlots;
        bool referenceHasSpace = foundReference != null && foundReference.Count + item.Count <= foundReference.MaxStack;
        return noReferenceButHasSpace || referenceHasSpace;
    }
}
