using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    private int lastPickedAmount; // Store the last picked-up amount

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("InventoryManager instance created.");
        }
        else
        {
            Destroy(gameObject);
            Debug.LogWarning("Another instance of InventoryManager was destroyed.");
        }
    }

    public List<Item> inventoryItems = new List<Item>(); // List to store all regular items in the inventory
    public List<Item> oneTimePickups = new List<Item>(); // List to store one-time pickups

    public float maxInventorySpace = 100; // Maximum allowed space
    public float currentSpaceUsed = 0;

    // Debugging method to print inventory status
    private void DebugInventoryStatus()
    {
        Debug.Log($"[DEBUG] Current Space Used: {currentSpaceUsed}, Max Inventory Space: {maxInventorySpace}, Total Items in Inventory: {inventoryItems.Count}");
    }

    public bool ReceiveItemData(Item item)
    {
        Debug.Log($"Attempting to receive item: {item.ItemName}, Tag: {item.Tag}, OneTimePick: {item.OnTimePick}");

        // Clone the item to avoid modifying the original ScriptableObject
        Item clonedItem = item.Clone();

        // Calculate available space
        float availableSpace = maxInventorySpace - currentSpaceUsed;

        // Calculate how many units can be picked up based on available space
        int maxPickableUnits = Mathf.FloorToInt(availableSpace / clonedItem.OccupieSpace);

        // Determine how many units to actually pick up
        int pickableUnits = Mathf.Min(clonedItem.CurrentAmount, maxPickableUnits);

        // Calculate space required for the pickable amount
        float spaceRequired = pickableUnits * clonedItem.OccupieSpace;
        Debug.Log($"[DEBUG] Calculated Space Required for {clonedItem.ItemName}: {spaceRequired} (Pickable Amount: {pickableUnits})");

        // Store the picked amount in a class-level variable for future access
        lastPickedAmount = pickableUnits;

        // If item is a one-time pickup
        if (clonedItem.OnTimePick)
        {

            if (oneTimePickups.Exists(x => x.Tag == clonedItem.Tag))
            {
                Debug.Log($"[DEBUG] One-time pickup already collected: {clonedItem.Tag}");
                return false;
            }

            else if(clonedItem.OccupieSpace > spaceRequired )
            {
                Debug.Log($"[DEBUG] Space is required for PickUP !...: {clonedItem.Tag}");
                return false;
            }
            else
            {
                oneTimePickups.Add(clonedItem);
                Debug.Log($"[DEBUG] One-time pickup added: {clonedItem.Tag}");
                return true;
            }
        }
        else
        {
            Item existingItem = inventoryItems.Find(x => x.Tag == clonedItem.Tag);

            if (existingItem != null)
            {
                // Calculate old and new space for existing items
                float oldSpace = existingItem.CurrentAmount * existingItem.OccupieSpace;
                existingItem.CurrentAmount += pickableUnits; // Add picked amount to existing
                float newSpace = existingItem.CurrentAmount * existingItem.OccupieSpace;
                float spaceDifference = newSpace - oldSpace;

                Debug.Log($"[DEBUG] Existing item '{existingItem.ItemName}': Old Space: {oldSpace}, New Space: {newSpace}, Space Difference: {spaceDifference}");

                // Check if there's enough space to add the item
                if (currentSpaceUsed + spaceDifference <= maxInventorySpace)
                {
                    currentSpaceUsed += spaceDifference;
                    Debug.Log($"[DEBUG] Item updated: {existingItem.ItemName}, New Amount: {existingItem.CurrentAmount}, Current Space Used: {currentSpaceUsed}");
                    UpdateItemSlots();
                    DebugInventoryStatus();
                    return true;
                }
                else
                {
                    Debug.LogError($"[ERROR] Not enough space in inventory! Cannot add item: {clonedItem.ItemName}. Space Required: {spaceRequired}, Current Space Used: {currentSpaceUsed}, Max Space: {maxInventorySpace}");
                    return false;
                }
            }
            else
            {
                // Adding a new item to inventory
                if (currentSpaceUsed + spaceRequired <= maxInventorySpace)
                {
                    clonedItem.CurrentAmount = pickableUnits; // Set the amount being picked up
                    inventoryItems.Add(clonedItem);
                    currentSpaceUsed += spaceRequired;
                    Debug.Log($"[DEBUG] Added new item: {clonedItem.ItemName}, Space Required: {spaceRequired}, Current Space Used: {currentSpaceUsed}");
                    UpdateItemSlots();
                    DebugInventoryStatus();
                    return true;
                }
                else
                {
                    Debug.LogError($"[ERROR] Not enough space in inventory! Cannot add item: {clonedItem.ItemName}. Space Required: {spaceRequired}, Current Space Used: {currentSpaceUsed}, Max Space: {maxInventorySpace}");
                    return false;
                }
            }
        }
    }

    // Method to get the last picked amount
    public int UpdatePickupObject()
    {
        return lastPickedAmount;
    }


    public void UpdateItemSlots()
    {
        // Update UI elements to reflect changes in inventory
        Debug.Log("[DEBUG] Inventory UI updated.");
    }



    // Function to get the current amount of a specific item from the inventory by its tag.
    public int GetCurrentAmountByTag(string tag)
    {
        // Find the item in the inventory by its tag
        Item existingItem = inventoryItems.Find(x => x.Tag == tag);

        // Debug: Log the tag and current amount for tracking
        Debug.Log($"GetCurrentAmountByTag - Tag: {tag}, Current Amount: {(existingItem != null ? existingItem.CurrentAmount.ToString() : "Item not found")}");

        // Return the current amount of the item if found; otherwise, return 0
        return existingItem != null ? existingItem.CurrentAmount : 0;
    }



    // Function to use (subtract) the current amount of a specific item by its tag and update space
    public void UseCurrentAmountByTag(string tag, int amountToUse)
    {
        // Find the item in the inventory by its tag
        Item existingItem = inventoryItems.Find(x => x.Tag == tag);

        // Debug: Log the tag and amount to use for tracking
        Debug.Log($"UseCurrentAmountByTag - Tag: {tag}, Amount To Use: {amountToUse}");

        // Check if the item exists and has enough amount to be used
        if (existingItem != null && existingItem.CurrentAmount >= amountToUse)
        {
            // Subtract the amount to be used
            existingItem.CurrentAmount -= amountToUse;

            // Debug: Log the updated current amount
            Debug.Log($"Updated Current Amount - Tag: {tag}, New Amount: {existingItem.CurrentAmount}");

            // Calculate the space freed up by the amount used (occupies space per unit of current amount)
            float spaceFreed = amountToUse * existingItem.OccupieSpace;

            // Debug: Log the space freed
            Debug.Log($"Space Freed - Tag: {tag}, Space Freed: {spaceFreed}");

            // Subtract the freed space from the current space used
            currentSpaceUsed -= spaceFreed;

            // Ensure that the current space used doesn't drop below zero
            if (currentSpaceUsed < 0) currentSpaceUsed = 0;

            // Debug: Log the updated space usage
            Debug.Log($"Updated Space Used: {currentSpaceUsed}");

            // Remove the item from the inventory if the current amount is zero or less
            if (existingItem.CurrentAmount <= 0)
            {
                inventoryItems.Remove(existingItem);
                // Debug: Log the removal of the item
                Debug.Log($"Item Removed - Tag: {tag}");
            }

            // Optionally update all connected UI elements or gameplay objects if necessary
            // TODO : UpdateCurrentAmountForAllObjects(tag, existingItem.CurrentAmount);
        }
        else
        {
            // Debug: Log if there is not enough amount or item is not found
            Debug.LogWarning($"Item not found or not enough amount - Tag: {tag}");
        }
    }










}


