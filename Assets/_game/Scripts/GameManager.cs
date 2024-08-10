using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static Inventory MainInventory => Instance.mainInventory;
    [SerializeField] private Inventory mainInventory;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
        mainInventory = new Inventory(10);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            InventoryItem item = new InventoryItem(ItemDatabase.GetItemByID(0));
            item.Count = 1;
            MainInventory.AddItem(item);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            InventoryItem item = new InventoryItem(ItemDatabase.GetItemByID(1));
            item.Count = 1;
            MainInventory.AddItem(item);
        }
    }
}
