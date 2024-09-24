using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string ItemName; // Name of the item to display in Inventory
    public string Tag; // Used to access other objects to use (e.g., gun needs ammo, healing needs medkit/bandage)
    public Sprite Icon; // Icon for UI display in Inventory
    public bool IsUsable = true; // Can the item be used (e.g., Medkit, Unlock something, etc.)
    public bool CanDrop = true; // Can the item be dropped?
    public bool CanAddable = true; // Can the item be added to an existing stack?
    public int CurrentAmount; // Current amount of the item (e.g., 30 bullets)
    public int MaxStack; // Maximum stack size (e.g., max 90 bullets)
    public float OccupieSpace; // one CurrentAmmout Value for Space required in the inventory
    public bool OnTimePick; // Used to only pick one-time pickups like power-ups in a lobby like PUBG

    // Method to clone the ScriptableObject and create a deep copy
    public Item Clone()
    {
        Item newItem = ScriptableObject.CreateInstance<Item>(); // Create a new instance
        newItem.ItemName = this.ItemName;
        newItem.Tag = this.Tag;
        newItem.Icon = this.Icon;
        newItem.IsUsable = this.IsUsable;
        newItem.CanDrop = this.CanDrop;
        newItem.CanAddable = this.CanAddable;
        newItem.CurrentAmount = this.CurrentAmount;
        newItem.MaxStack = this.MaxStack;
        newItem.OccupieSpace = this.OccupieSpace;
        newItem.OnTimePick = this.OnTimePick;
        return newItem;
    }
}

