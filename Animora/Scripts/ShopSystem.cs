using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopCategory
{
    public string categoryName;
    public Sprite categoryIcon;
    public List<string> itemIds = new List<string>();
}

[System.Serializable]
public class ShopDiscount
{
    public string discountName;
    public float discountPercentage;
    public List<string> appliedItemIds = new List<string>();
    public List<ItemType> appliedItemTypes = new List<ItemType>();
    public System.DateTime startDate;
    public System.DateTime endDate;
    public bool isActive = true;
}

public class ShopSystem : MonoBehaviour
{
    public static ShopSystem Instance;
    
    [Header("Shop Configuration")]
    public List<ShopCategory> shopCategories = new List<ShopCategory>();
    public List<ShopDiscount> activeDiscounts = new List<ShopDiscount>();
    
    [Header("Featured Items")]
    public List<string> featuredItemIds = new List<string>();
    public float featuredItemDiscount = 0.1f; // 10% off
    
    [Header("Daily Deals")]
    public int dailyDealCount = 3;
    public float dailyDealDiscount = 0.25f; // 25% off
    private List<string> dailyDealItemIds = new List<string>();
    private System.DateTime lastDailyDealUpdate;
    
    // Private variables
    private Dictionary<string, ShopCategory> categoryDictionary = new Dictionary<string, ShopCategory>();
    
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
        
        // Initialize category dictionary
        foreach (ShopCategory category in shopCategories)
        {
            categoryDictionary[category.categoryName] = category;
        }
    }
    
    private void Start()
    {
        // Initialize daily deals
        UpdateDailyDeals();
    }
    
    private void Update()
    {
        // Check if we need to update daily deals
        if (System.DateTime.Now.Date != lastDailyDealUpdate.Date)
        {
            UpdateDailyDeals();
        }
        
        // Update discount validity
        UpdateDiscounts();
    }
    
    private void UpdateDailyDeals()
    {
        // Clear previous daily deals
        dailyDealItemIds.Clear();
        
        // Get all available items
        List<string> availableItemIds = new List<string>();
        foreach (ShopCategory category in shopCategories)
        {
            availableItemIds.AddRange(category.itemIds);
        }
        
        // Remove featured items from available items
        foreach (string featuredItemId in featuredItemIds)
        {
            availableItemIds.Remove(featuredItemId);
        }
        
        // Randomly select daily deals
        for (int i = 0; i < dailyDealCount && availableItemIds.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, availableItemIds.Count);
            string selectedItemId = availableItemIds[randomIndex];
            
            dailyDealItemIds.Add(selectedItemId);
            availableItemIds.RemoveAt(randomIndex);
        }
        
        // Update timestamp
        lastDailyDealUpdate = System.DateTime.Now;
    }
    
    private void UpdateDiscounts()
    {
        // Check each discount for validity
        for (int i = activeDiscounts.Count - 1; i >= 0; i--)
        {
            ShopDiscount discount = activeDiscounts[i];
            
            // Check if discount period is over
            if (System.DateTime.Now > discount.endDate)
            {
                discount.isActive = false;
            }
            
            // Check if discount period has started
            if (System.DateTime.Now < discount.startDate)
            {
                discount.isActive = false;
            }
            else
            {
                discount.isActive = true;
            }
        }
    }
    
    // Get all items in a category
    public List<Item> GetCategoryItems(string categoryName)
    {
        List<Item> items = new List<Item>();
        
        if (categoryDictionary.ContainsKey(categoryName))
        {
            ShopCategory category = categoryDictionary[categoryName];
            
            foreach (string itemId in category.itemIds)
            {
                Item item = ItemSystem.Instance.GetItemById(itemId);
                if (item != null)
                {
                    items.Add(item);
                }
            }
        }
        
        return items;
    }
    
    // Get all featured items
    public List<Item> GetFeaturedItems()
    {
        List<Item> items = new List<Item>();
        
        foreach (string itemId in featuredItemIds)
        {
            Item item = ItemSystem.Instance.GetItemById(itemId);
            if (item != null)
            {
                items.Add(item);
            }
        }
        
        return items;
    }
    
    // Get all daily deal items
    public List<Item> GetDailyDealItems()
    {
        List<Item> items = new List<Item>();
        
        foreach (string itemId in dailyDealItemIds)
        {
            Item item = ItemSystem.Instance.GetItemById(itemId);
            if (item != null)
            {
                items.Add(item);
            }
        }
        
        return items;
    }
    
    // Calculate the final price of an item after discounts
    public int GetItemPrice(Item item)
    {
        if (item == null)
            return 0;
        
        float price = item.price;
        float discountMultiplier = 1f;
        
        // Check if item is featured
        if (featuredItemIds.Contains(item.itemId))
        {
            discountMultiplier -= featuredItemDiscount;
        }
        
        // Check if item is a daily deal
        if (dailyDealItemIds.Contains(item.itemId))
        {
            discountMultiplier -= dailyDealDiscount;
        }
        
        // Check for additional discounts
        foreach (ShopDiscount discount in activeDiscounts)
        {
            if (discount.isActive)
            {
                // Check if discount applies to this item
                if (discount.appliedItemIds.Contains(item.itemId) || 
                    discount.appliedItemTypes.Contains(item.itemType))
                {
                    discountMultiplier -= discount.discountPercentage;
                }
            }
        }
        
        // Ensure discount doesn't go below 0.1 (90% off)
        discountMultiplier = Mathf.Max(0.1f, discountMultiplier);
        
        // Calculate final price
        price *= discountMultiplier;
        
        return Mathf.Max(1, Mathf.RoundToInt(price));
    }
    
    // Buy an item
    public bool BuyItem(Item item, int quantity = 1)
    {
        if (item == null || quantity <= 0)
            return false;
        
        // Calculate total cost
        int itemPrice = GetItemPrice(item);
        int totalCost = itemPrice * quantity;
        
        // Check if player has enough currency
        if (GameManager.Instance.SpendCurrency(totalCost))
        {
            // Add item to inventory
            bool added = ItemSystem.Instance.AddItem(item, quantity);
            
            if (!added)
            {
                // Refund if inventory is full
                GameManager.Instance.AddCurrency(totalCost);
                return false;
            }
            
            return true;
        }
        
        return false;
    }
    
    // Sell an item
    public bool SellItem(Item item, int quantity = 1)
    {
        if (item == null || quantity <= 0)
            return false;
        
        // Check if player has the item
        if (ItemSystem.Instance.HasItem(item, quantity))
        {
            // Calculate sell price (half of buy price)
            int sellPrice = Mathf.Max(1, GetItemPrice(item) / 2);
            int totalValue = sellPrice * quantity;
            
            // Remove item from inventory
            bool removed = ItemSystem.Instance.RemoveItem(item, quantity);
            
            if (removed)
            {
                // Add currency
                GameManager.Instance.AddCurrency(totalValue);
                return true;
            }
        }
        
        return false;
    }
    
    // Add a new category
    public void AddCategory(string categoryName, Sprite icon)
    {
        if (!categoryDictionary.ContainsKey(categoryName))
        {
            ShopCategory newCategory = new ShopCategory
            {
                categoryName = categoryName,
                categoryIcon = icon
            };
            
            shopCategories.Add(newCategory);
            categoryDictionary[categoryName] = newCategory;
        }
    }
    
    // Add an item to a category
    public void AddItemToCategory(string itemId, string categoryName)
    {
        if (categoryDictionary.ContainsKey(categoryName))
        {
            ShopCategory category = categoryDictionary[categoryName];
            
            if (!category.itemIds.Contains(itemId))
            {
                category.itemIds.Add(itemId);
            }
        }
    }
    
    // Add a new discount
    public void AddDiscount(string name, float percentage, List<string> itemIds, List<ItemType> itemTypes, System.DateTime endDate)
    {
        ShopDiscount newDiscount = new ShopDiscount
        {
            discountName = name,
            discountPercentage = percentage,
            appliedItemIds = new List<string>(itemIds),
            appliedItemTypes = new List<ItemType>(itemTypes),
            startDate = System.DateTime.Now,
            endDate = endDate,
            isActive = true
        };
        
        activeDiscounts.Add(newDiscount);
    }
    
    // Set featured items
    public void SetFeaturedItems(List<string> itemIds)
    {
        featuredItemIds = new List<string>(itemIds);
    }
}