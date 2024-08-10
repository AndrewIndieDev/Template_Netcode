using System;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    public InventoryItem(string name, Sprite icon, int count = 1)
    {
        Name = name;
        Icon = icon;
        Count = count;
    }

    public InventoryItem(InventoryItem copyFrom)
    {
        Name = copyFrom.Name;
        Icon = copyFrom.Icon;
        MaxStack = copyFrom.MaxStack;
        Count = copyFrom.Count;
    }

    public int ID => ItemDatabase.GetIDByName(Name);
    public string Name;
    public Sprite Icon;
    public int MaxStack = 1;
    public int Count;

    public void AddCount(int amount)
    {
        Count += amount;
    }
}
