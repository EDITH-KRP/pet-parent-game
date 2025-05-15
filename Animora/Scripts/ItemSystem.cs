using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Food,
    Toy,
    Clothing,
    Furniture,
    Decoration,
    Medicine,
    Special
}

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

[System.Serializable]
public class Item
{
    public string itemId;
    public string itemName;
    public string description;
    public ItemType itemType;
    public ItemRarity rarity = ItemRarity.Common;
    public Sprite icon;
    public GameObject prefab;
    public int price = 10;
    public bool isStackable = true;
    public int maxStackSize = 99;
    
    // Stats effects
    public float hungerEffect;
    public float moodEffect;
    public float energyEffect;
    public float cleanlinessEffect;
    public float healthEffect;
    
    // Special effects
    public bool hasSpecialEffect;
    public string specialEffectDescription;
}

[System.Serializable]
public class InventoryItem
{
    public Item item;
    public int quantity;
    
    public InventoryItem(Item item, int quantity = 1)
    {
        this.item = item;
        this.quantity = quantity;
    }
}

public class ItemSystem : MonoBehaviour
{
    public static ItemSystem Instance;
    
    [Header("Item Database")]
    public List<Item> allItems = new List<Item>();
    
    [Header("Player Inventory")]
    public List<InventoryItem> inventory = new List<InventoryItem>();
    public int maxInventorySize = 50;
    
    [Header("Equipped Items")]
    public List<Item> equippedClothing = new List<Item>();
    public List<Item> placedFurniture = new List<Item>();
    public List<Item> placedDecorations = new List<Item>();
    
    // Private variables
    private Dictionary<string, Item> itemDictionary = new Dictionary<string, Item>();
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Initialize item dictionary
        foreach (Item item in allItems)
        {
            itemDictionary[item.itemId] = item;
        }
    }
    
    // Add item to inventory
    public bool AddItem(string itemId, int quantity = 1)
    {
        if (!itemDictionary.ContainsKey(itemId))
        {
            Debug.LogWarning($"Item with ID {itemId} not found in database!");
            return false;
        }
        
        Item item = itemDictionary[itemId];
        return AddItem(item, quantity);
    }
    
    public bool AddItem(Item item, int quantity = 1)
    {
        if (item == null || quantity <= 0)
            return false;
        
        // Check if inventory is full
        if (inventory.Count >= maxInventorySize && !HasItem(item))
        {
            Debug.Log("Inventory is full!");
            return false;
        }
        
        // If item is stackable, try to add to existing stack
        if (item.isStackable)
        {
            foreach (InventoryItem inventoryItem in inventory)
            {
                if (inventoryItem.item.itemId == item.itemId)
                {
                    // Check if adding would exceed max stack size
                    if (inventoryItem.quantity + quantity <= item.maxStackSize)
                    {
                        inventoryItem.quantity += quantity;
                        return true;
                    }
                    else
                    {
                        // Add what we can to this stack
                        int canAdd = item.maxStackSize - inventoryItem.quantity;
                        inventoryItem.quantity += canAdd;
                        quantity -= canAdd;
                        
                        // If we still have items to add, continue to create a new stack
                        if (quantity <= 0)
                            return true;
                    }
                }
            }
        }
        
        // If we get here, we need to add a new inventory item
        if (inventory.Count < maxInventorySize)
        {
            inventory.Add(new InventoryItem(item, quantity));
            return true;
        }
        
        return false;
    }
    
    // Remove item from inventory
    public bool RemoveItem(string itemId, int quantity = 1)
    {
        if (!itemDictionary.ContainsKey(itemId))
        {
            Debug.LogWarning($"Item with ID {itemId} not found in database!");
            return false;
        }
        
        Item item = itemDictionary[itemId];
        return RemoveItem(item, quantity);
    }
    
    public bool RemoveItem(Item item, int quantity = 1)
    {
        if (item == null || quantity <= 0)
            return false;
        
        int remainingToRemove = quantity;
        
        // Find all stacks of this item
        for (int i = inventory.Count - 1; i >= 0; i--)
        {
            InventoryItem inventoryItem = inventory[i];
            
            if (inventoryItem.item.itemId == item.itemId)
            {
                if (inventoryItem.quantity >= remainingToRemove)
                {
                    // This stack has enough to satisfy the request
                    inventoryItem.quantity -= remainingToRemove;
                    
                    // Remove the stack if it's empty
                    if (inventoryItem.quantity <= 0)
                    {
                        inventory.RemoveAt(i);
                    }
                    
                    return true;
                }
                else
                {
                    // This stack doesn't have enough, so take what we can
                    remainingToRemove -= inventoryItem.quantity;
                    inventory.RemoveAt(i);
                    
                    // If we've removed enough, we're done
                    if (remainingToRemove <= 0)
                    {
                        return true;
                    }
                }
            }
        }
        
        // If we get here, we couldn't remove all the requested quantity
        return false;
    }
    
    // Check if player has an item
    public bool HasItem(string itemId, int quantity = 1)
    {
        if (!itemDictionary.ContainsKey(itemId))
        {
            Debug.LogWarning($"Item with ID {itemId} not found in database!");
            return false;
        }
        
        Item item = itemDictionary[itemId];
        return HasItem(item, quantity);
    }
    
    public bool HasItem(Item item, int quantity = 1)
    {
        if (item == null || quantity <= 0)
            return false;
        
        int totalQuantity = 0;
        
        foreach (InventoryItem inventoryItem in inventory)
        {
            if (inventoryItem.item.itemId == item.itemId)
            {
                totalQuantity += inventoryItem.quantity;
                
                if (totalQuantity >= quantity)
                {
                    return true;
                }
            }
        }
        
        return false;
    }
    
    // Get total quantity of an item
    public int GetItemQuantity(string itemId)
    {
        if (!itemDictionary.ContainsKey(itemId))
        {
            Debug.LogWarning($"Item with ID {itemId} not found in database!");
            return 0;
        }
        
        Item item = itemDictionary[itemId];
        return GetItemQuantity(item);
    }
    
    public int GetItemQuantity(Item item)
    {
        if (item == null)
            return 0;
        
        int totalQuantity = 0;
        
        foreach (InventoryItem inventoryItem in inventory)
        {
            if (inventoryItem.item.itemId == item.itemId)
            {
                totalQuantity += inventoryItem.quantity;
            }
        }
        
        return totalQuantity;
    }
    
    // Use an item on a pet
    public bool UseItem(Item item, Pet targetPet)
    {
        if (item == null || targetPet == null)
            return false;
        
        // Check if we have the item
        if (!HasItem(item))
            return false;
        
        // Apply item effects based on type
        switch (item.itemType)
        {
            case ItemType.Food:
                // Apply hunger and other effects
                targetPet.stats.hunger = Mathf.Min(100, targetPet.stats.hunger + item.hungerEffect);
                targetPet.stats.mood = Mathf.Min(100, targetPet.stats.mood + item.moodEffect);
                targetPet.stats.energy = Mathf.Min(100, targetPet.stats.energy + item.energyEffect);
                
                // Play eating animation
                targetPet.Feed();
                break;
                
            case ItemType.Toy:
                // Apply mood and energy effects
                targetPet.stats.mood = Mathf.Min(100, targetPet.stats.mood + item.moodEffect);
                targetPet.stats.energy = Mathf.Max(0, targetPet.stats.energy + item.energyEffect); // Toys might reduce energy
                
                // Play playing animation
                targetPet.Play();
                break;
                
            case ItemType.Medicine:
                // Apply health effects
                targetPet.stats.health = Mathf.Min(100, targetPet.stats.health + item.healthEffect);
                targetPet.stats.mood = Mathf.Min(100, targetPet.stats.mood + item.moodEffect); // Medicine might affect mood
                
                // Play healing animation
                targetPet.Heal();
                break;
                
            case ItemType.Clothing:
                // Equip clothing on pet
                EquipClothing(item, targetPet);
                return true; // Don't consume clothing items
                
            default:
                Debug.Log($"Item type {item.itemType} not implemented for direct use on pets.");
                return false;
        }
        
        // Remove one of the item from inventory
        RemoveItem(item);
        
        // Add experience to pet
        targetPet.AddExperience(5f);
        
        return true;
    }
    
    // Equip clothing on a pet
    private void EquipClothing(Item clothingItem, Pet targetPet)
    {
        if (clothingItem.itemType != ItemType.Clothing || targetPet == null)
            return;
        
        // This would be implemented with a more complex clothing system
        // For now, just log that we equipped it
        Debug.Log($"Equipped {clothingItem.itemName} on {targetPet.petName}");
        
        // Add to equipped clothing list
        if (!equippedClothing.Contains(clothingItem))
        {
            equippedClothing.Add(clothingItem);
        }
    }
    
    // Place furniture in the world
    public GameObject PlaceFurniture(Item furnitureItem, Vector3 position, Quaternion rotation)
    {
        if (furnitureItem.itemType != ItemType.Furniture)
            return null;
        
        // Check if we have the item
        if (!HasItem(furnitureItem))
            return null;
        
        // Remove one from inventory
        RemoveItem(furnitureItem);
        
        // Instantiate the furniture prefab
        GameObject furniture = Instantiate(furnitureItem.prefab, position, rotation);
        
        // Add to placed furniture list
        if (!placedFurniture.Contains(furnitureItem))
        {
            placedFurniture.Add(furnitureItem);
        }
        
        return furniture;
    }
    
    // Place decoration in the world
    public GameObject PlaceDecoration(Item decorationItem, Vector3 position, Quaternion rotation)
    {
        if (decorationItem.itemType != ItemType.Decoration)
            return null;
        
        // Check if we have the item
        if (!HasItem(decorationItem))
            return null;
        
        // Remove one from inventory
        RemoveItem(decorationItem);
        
        // Instantiate the decoration prefab
        GameObject decoration = Instantiate(decorationItem.prefab, position, rotation);
        
        // Add to placed decorations list
        if (!placedDecorations.Contains(decorationItem))
        {
            placedDecorations.Add(decorationItem);
        }
        
        return decoration;
    }
    
    // Get item by ID
    public Item GetItemById(string itemId)
    {
        if (itemDictionary.ContainsKey(itemId))
        {
            return itemDictionary[itemId];
        }
        
        return null;
    }
    
    // Get all items of a specific type
    public List<Item> GetItemsByType(ItemType type)
    {
        List<Item> items = new List<Item>();
        
        foreach (Item item in allItems)
        {
            if (item.itemType == type)
            {
                items.Add(item);
            }
        }
        
        return items;
    }
    
    // Get all items of a specific rarity
    public List<Item> GetItemsByRarity(ItemRarity rarity)
    {
        List<Item> items = new List<Item>();
        
        foreach (Item item in allItems)
        {
            if (item.rarity == rarity)
            {
                items.Add(item);
            }
        }
        
        return items;
    }
}