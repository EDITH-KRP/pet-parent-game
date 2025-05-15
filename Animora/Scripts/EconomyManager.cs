using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CurrencySource
{
    public string sourceName;
    public int baseAmount;
    public float levelMultiplier = 0.1f; // Additional percentage per player level
    public float cooldownMinutes = 0f; // 0 means no cooldown
    public System.DateTime lastAwarded;
}

[System.Serializable]
public class ItemPriceModifier
{
    public ItemType itemType;
    public float priceMultiplier = 1.0f;
}

[System.Serializable]
public class EconomyEvent
{
    public string eventName;
    public System.DateTime startDate;
    public System.DateTime endDate;
    public bool isActive = false;
    public List<ItemPriceModifier> priceModifiers = new List<ItemPriceModifier>();
    public float globalSellPriceMultiplier = 1.0f;
    public float globalBuyPriceMultiplier = 1.0f;
    public float currencyBonusMultiplier = 1.0f;
}

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance;
    
    [Header("Currency Settings")]
    public List<CurrencySource> currencySources = new List<CurrencySource>();
    public float dailyLoginBonusMultiplier = 1.5f; // Bonus for consecutive daily logins
    public int maxConsecutiveDays = 7; // Max consecutive days for bonus
    
    [Header("Item Price Settings")]
    public List<ItemPriceModifier> defaultPriceModifiers = new List<ItemPriceModifier>();
    public float defaultSellPriceRatio = 0.5f; // Items sell for 50% of buy price by default
    public float rarityPriceMultiplier = 1.5f; // Each rarity level increases price by 50%
    
    [Header("Economy Events")]
    public List<EconomyEvent> scheduledEvents = new List<EconomyEvent>();
    public EconomyEvent currentEvent;
    
    // Private variables
    private int consecutiveDays = 0;
    private System.DateTime lastLoginDate;
    
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
        
        // Initialize default price modifiers if empty
        if (defaultPriceModifiers.Count == 0)
        {
            InitializeDefaultPriceModifiers();
        }
        
        // Initialize currency sources if empty
        if (currencySources.Count == 0)
        {
            InitializeDefaultCurrencySources();
        }
    }
    
    private void Start()
    {
        // Check for daily login
        CheckDailyLogin();
        
        // Check for active events
        CheckActiveEvents();
    }
    
    private void Update()
    {
        // Periodically check for events (once per minute is enough)
        if (Time.frameCount % 3600 == 0) // Assuming 60 fps, this is once per minute
        {
            CheckActiveEvents();
        }
    }
    
    private void InitializeDefaultPriceModifiers()
    {
        // Add default price modifiers for each item type
        defaultPriceModifiers.Add(new ItemPriceModifier { itemType = ItemType.Food, priceMultiplier = 1.0f });
        defaultPriceModifiers.Add(new ItemPriceModifier { itemType = ItemType.Toy, priceMultiplier = 1.2f });
        defaultPriceModifiers.Add(new ItemPriceModifier { itemType = ItemType.Clothing, priceMultiplier = 1.5f });
        defaultPriceModifiers.Add(new ItemPriceModifier { itemType = ItemType.Furniture, priceMultiplier = 2.0f });
        defaultPriceModifiers.Add(new ItemPriceModifier { itemType = ItemType.Decoration, priceMultiplier = 1.3f });
        defaultPriceModifiers.Add(new ItemPriceModifier { itemType = ItemType.Medicine, priceMultiplier = 1.8f });
        defaultPriceModifiers.Add(new ItemPriceModifier { itemType = ItemType.Special, priceMultiplier = 3.0f });
    }
    
    private void InitializeDefaultCurrencySources()
    {
        // Add default currency sources
        currencySources.Add(new CurrencySource
        {
            sourceName = "Daily Login",
            baseAmount = 50,
            levelMultiplier = 0.1f,
            cooldownMinutes = 1440 // 24 hours
        });
        
        currencySources.Add(new CurrencySource
        {
            sourceName = "Pet Care",
            baseAmount = 10,
            levelMultiplier = 0.05f,
            cooldownMinutes = 60 // 1 hour
        });
        
        currencySources.Add(new CurrencySource
        {
            sourceName = "Location Visit",
            baseAmount = 20,
            levelMultiplier = 0.05f,
            cooldownMinutes = 240 // 4 hours
        });
        
        currencySources.Add(new CurrencySource
        {
            sourceName = "Pet Level Up",
            baseAmount = 100,
            levelMultiplier = 0.2f,
            cooldownMinutes = 0 // No cooldown
        });
    }
    
    private void CheckDailyLogin()
    {
        System.DateTime today = System.DateTime.Now.Date;
        
        // Load last login date from player data
        if (GameManager.Instance != null && GameManager.Instance.playerData != null)
        {
            lastLoginDate = GameManager.Instance.playerData.lastLoginDate;
        }
        
        // Check if this is a new day
        if (today != lastLoginDate)
        {
            // Check if this is a consecutive day
            if ((today - lastLoginDate).Days == 1)
            {
                consecutiveDays = Mathf.Min(consecutiveDays + 1, maxConsecutiveDays);
            }
            else
            {
                consecutiveDays = 1; // Reset streak
            }
            
            // Award daily login bonus
            AwardCurrencyFromSource("Daily Login");
            
            // Update last login date
            lastLoginDate = today;
            if (GameManager.Instance != null && GameManager.Instance.playerData != null)
            {
                GameManager.Instance.playerData.lastLoginDate = lastLoginDate;
                GameManager.Instance.playerData.consecutiveLoginDays = consecutiveDays;
            }
        }
    }
    
    private void CheckActiveEvents()
    {
        // Reset current event
        currentEvent = null;
        
        System.DateTime now = System.DateTime.Now;
        
        // Check all scheduled events
        foreach (EconomyEvent economyEvent in scheduledEvents)
        {
            if (now >= economyEvent.startDate && now <= economyEvent.endDate)
            {
                economyEvent.isActive = true;
                currentEvent = economyEvent;
                
                // For simplicity, we'll only use one active event at a time
                // In a real game, you might want to handle multiple overlapping events
                break;
            }
            else
            {
                economyEvent.isActive = false;
            }
        }
    }
    
    // Award currency from a specific source
    public bool AwardCurrencyFromSource(string sourceName)
    {
        CurrencySource source = currencySources.Find(s => s.sourceName == sourceName);
        
        if (source == null)
            return false;
            
        // Check cooldown
        if (source.cooldownMinutes > 0)
        {
            System.DateTime now = System.DateTime.Now;
            System.TimeSpan timeSinceLastAward = now - source.lastAwarded;
            
            if (timeSinceLastAward.TotalMinutes < source.cooldownMinutes)
            {
                return false; // Still on cooldown
            }
        }
        
        // Calculate amount based on player level
        int playerLevel = 1;
        if (GameManager.Instance != null && GameManager.Instance.playerData != null)
        {
            playerLevel = GameManager.Instance.playerData.level;
        }
        
        float levelBonus = 1f + (playerLevel - 1) * source.levelMultiplier;
        
        // Apply consecutive day bonus for daily login
        if (sourceName == "Daily Login")
        {
            float consecutiveBonus = 1f + (consecutiveDays - 1) * (dailyLoginBonusMultiplier - 1f) / (maxConsecutiveDays - 1);
            levelBonus *= consecutiveBonus;
        }
        
        // Apply event bonus if any
        if (currentEvent != null && currentEvent.isActive)
        {
            levelBonus *= currentEvent.currencyBonusMultiplier;
        }
        
        // Calculate final amount
        int amount = Mathf.RoundToInt(source.baseAmount * levelBonus);
        
        // Award currency
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddCurrency(amount);
            
            // Update last awarded time
            source.lastAwarded = System.DateTime.Now;
            
            return true;
        }
        
        return false;
    }
    
    // Calculate buy price for an item
    public int CalculateBuyPrice(Item item)
    {
        if (item == null)
            return 0;
            
        float basePrice = item.price;
        
        // Apply item type modifier
        ItemPriceModifier typeModifier = defaultPriceModifiers.Find(m => m.itemType == item.itemType);
        if (typeModifier != null)
        {
            basePrice *= typeModifier.priceMultiplier;
        }
        
        // Apply rarity modifier
        float rarityMultiplier = Mathf.Pow(rarityPriceMultiplier, (int)item.rarity);
        basePrice *= rarityMultiplier;
        
        // Apply event modifiers if any
        if (currentEvent != null && currentEvent.isActive)
        {
            // Apply global buy price modifier
            basePrice *= currentEvent.globalBuyPriceMultiplier;
            
            // Apply specific item type modifier if any
            ItemPriceModifier eventTypeModifier = currentEvent.priceModifiers.Find(m => m.itemType == item.itemType);
            if (eventTypeModifier != null)
            {
                basePrice *= eventTypeModifier.priceMultiplier;
            }
        }
        
        // Round to nearest integer
        return Mathf.Max(1, Mathf.RoundToInt(basePrice));
    }
    
    // Calculate sell price for an item
    public int CalculateSellPrice(Item item)
    {
        if (item == null)
            return 0;
            
        // Start with buy price
        float buyPrice = CalculateBuyPrice(item);
        
        // Apply sell ratio
        float sellPrice = buyPrice * defaultSellPriceRatio;
        
        // Apply event modifier if any
        if (currentEvent != null && currentEvent.isActive)
        {
            sellPrice *= currentEvent.globalSellPriceMultiplier;
        }
        
        // Round to nearest integer
        return Mathf.Max(1, Mathf.RoundToInt(sellPrice));
    }
    
    // Create a scheduled economy event
    public void CreateScheduledEvent(string eventName, System.DateTime startDate, System.DateTime endDate, float buyMultiplier, float sellMultiplier, float currencyMultiplier)
    {
        EconomyEvent newEvent = new EconomyEvent
        {
            eventName = eventName,
            startDate = startDate,
            endDate = endDate,
            globalBuyPriceMultiplier = buyMultiplier,
            globalSellPriceMultiplier = sellMultiplier,
            currencyBonusMultiplier = currencyMultiplier
        };
        
        scheduledEvents.Add(newEvent);
        
        // Check if this event should be active now
        CheckActiveEvents();
    }
    
    // Add a price modifier to an event
    public void AddEventPriceModifier(string eventName, ItemType itemType, float priceMultiplier)
    {
        EconomyEvent economyEvent = scheduledEvents.Find(e => e.eventName == eventName);
        
        if (economyEvent != null)
        {
            // Check if modifier already exists
            ItemPriceModifier existingModifier = economyEvent.priceModifiers.Find(m => m.itemType == itemType);
            
            if (existingModifier != null)
            {
                existingModifier.priceMultiplier = priceMultiplier;
            }
            else
            {
                economyEvent.priceModifiers.Add(new ItemPriceModifier
                {
                    itemType = itemType,
                    priceMultiplier = priceMultiplier
                });
            }
        }
    }
    
    // Get time until cooldown expires for a currency source
    public float GetCooldownTimeRemaining(string sourceName)
    {
        CurrencySource source = currencySources.Find(s => s.sourceName == sourceName);
        
        if (source == null || source.cooldownMinutes <= 0)
            return 0f;
            
        System.DateTime now = System.DateTime.Now;
        System.TimeSpan timeSinceLastAward = now - source.lastAwarded;
        
        float minutesRemaining = (float)(source.cooldownMinutes - timeSinceLastAward.TotalMinutes);
        return Mathf.Max(0f, minutesRemaining);
    }
    
    // Get active event info
    public string GetActiveEventInfo()
    {
        if (currentEvent == null || !currentEvent.isActive)
            return "No active events";
            
        string info = $"Event: {currentEvent.eventName}\n";
        info += $"Duration: {currentEvent.startDate.ToShortDateString()} to {currentEvent.endDate.ToShortDateString()}\n";
        
        if (currentEvent.globalBuyPriceMultiplier != 1.0f)
        {
            float percentChange = (currentEvent.globalBuyPriceMultiplier - 1.0f) * 100f;
            string changeText = percentChange < 0 ? $"{percentChange:F0}% OFF!" : $"{percentChange:F0}% increase";
            info += $"Buy prices: {changeText}\n";
        }
        
        if (currentEvent.globalSellPriceMultiplier != 1.0f)
        {
            float percentChange = (currentEvent.globalSellPriceMultiplier - 1.0f) * 100f;
            info += $"Sell prices: {percentChange:F0}% bonus\n";
        }
        
        if (currentEvent.currencyBonusMultiplier != 1.0f)
        {
            float percentChange = (currentEvent.currencyBonusMultiplier - 1.0f) * 100f;
            info += $"Currency rewards: {percentChange:F0}% bonus\n";
        }
        
        return info;
    }
}