using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public Item itemData; // Reference to the ScriptableObject (assigned in the Editor)
    public Item clonedItem; // Clone of the item data for modification


    public void Pickup()
    {
        Debug.Log($"Attempting to pick up: {itemData.ItemName}, Tag: {itemData.Tag}");

        // Clone the item to avoid modifying the original ScriptableObject
        if (clonedItem == null)
        {
            clonedItem = itemData.Clone();
        }

        // Attempt to receive item data in InventoryManager
        bool wasItemAdded = InventoryManager.Instance.ReceiveItemData(clonedItem);

        // Get the amount of items picked up
        int addedAmount = InventoryManager.Instance.UpdatePickupObject(); 

        // Provide feedback to the player
        if (wasItemAdded)
        {
            // Subtract the picked amount from the clonedItem's current amount
            clonedItem.CurrentAmount -= addedAmount;

            Debug.Log($"Updated Amount after pickup: {addedAmount}, Current Amount remaining: {clonedItem.CurrentAmount}");

            // Destroy the pickup object if no items are left
            if (clonedItem.CurrentAmount <= 0)
            {
                DestroyCurrentPickup();
            }
        }
        else
        {
            Debug.LogWarning($"Failed to pick up {itemData.ItemName}. Not enough space in inventory.");
        }
    }

    public void DestroyCurrentPickup()
    {
        Debug.Log($"Picked up and removed: {itemData.ItemName}");
        Destroy(gameObject);
    }
}
