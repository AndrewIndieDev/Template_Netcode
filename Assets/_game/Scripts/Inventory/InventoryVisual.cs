using UnityEngine;

public class InventoryVisual : MonoBehaviour
{
    private void Start()
    {
        GameManager.MainInventory.OnInventoryChanged += UpdateInventory;
    }

    private void UpdateInventory()
    {
        Debug.Log("Updating inventory");
    }
}
