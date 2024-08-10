using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;
    private void Awake()
    {
        Instance = this;
    }

    public static List<InventoryItem> Items => Instance.items;
    [SerializeField] private List<InventoryItem> items = new();

    public static InventoryItem GetItemByName(string name)
    {
        foreach (var item in Items)
        {
            if (item.Name == name)
                return item;
        }
        return null;
    }

    public static InventoryItem GetItemByID(int id)
    {
        return Items[id];
    }

    public static int GetIDByName(string name)
    {
        foreach (var item in Items)
        {
            if (item.Name == name)
                return Items.IndexOf(item);
        }
        return -1;
    }
}
