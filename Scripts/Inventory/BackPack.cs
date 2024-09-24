// // Backpack.cs
// using System.Collections.Generic;
// using UnityEngine;

// public class Backpack : MonoBehaviour
// {
//     public int maxCapacity = 20;  // Maximum capacity of items in the backpack
//     public List<Item> items = new List<Item>(); // List of items currently in the backpack

//     // Event to update the UI when the backpack changes
//     public delegate void OnBackpackChanged();
//     public OnBackpackChanged onBackpackChangedCallback;

//     // Method to add an item to the backpack
//     public bool AddItem(Item item)
//     {
//         if (items.Count >= maxCapacity)
//         {
//             Debug.Log("Backpack is full!");
//             return false;
//         }

//         items.Add(item);
//         Debug.Log($"{item.itemName} added to the backpack.");
//         onBackpackChangedCallback?.Invoke(); // Update the UI
//         return true;
//     }

//     // Method to remove an item from the backpack
//     public void RemoveItem(Item item)
//     {
//         if (items.Contains(item))
//         {
//             items.Remove(item);
//             Debug.Log($"{item.itemName} removed from the backpack.");
//             onBackpackChangedCallback?.Invoke(); // Update the UI
//         }
//     }

//     // Method to use an item
//     public void UseItem(Item item)
//     {
//         if (items.Contains(item))
//         {
//             item.Use(); // Call the item's use method
//             if (item.itemType != Item.ItemType.Weapon) // For consumables
//             {
//                 RemoveItem(item);
//             }
//         }
//     }
// }
