using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;
    
    [Header("Item Collections")]
    public List<Item> foodItems = new List<Item>();
    public List<Item> toyItems = new List<Item>();
    public List<Item> clothingItems = new List<Item>();
    public List<Item> furnitureItems = new List<Item>();
    public List<Item> decorationItems = new List<Item>();
    public List<Item> medicineItems = new List<Item>();
    public List<Item> specialItems = new List<Item>();
    
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
        
        // Initialize item database
        InitializeItems();
    }
    
    private void Start()
    {
        // Register all items with the ItemSystem
        RegisterItemsWithSystem();
    }
    
    private void InitializeItems()
    {
        // Create food items
        CreateFoodItems();
        
        // Create toy items
        CreateToyItems();
        
        // Create clothing items
        CreateClothingItems();
        
        // Create furniture items
        CreateFurnitureItems();
        
        // Create decoration items
        CreateDecorationItems();
        
        // Create medicine items
        CreateMedicineItems();
        
        // Create special items
        CreateSpecialItems();
    }
    
    private void RegisterItemsWithSystem()
    {
        if (ItemSystem.Instance == null)
            return;
            
        // Register all items with the ItemSystem
        RegisterItemCollection(foodItems);
        RegisterItemCollection(toyItems);
        RegisterItemCollection(clothingItems);
        RegisterItemCollection(furnitureItems);
        RegisterItemCollection(decorationItems);
        RegisterItemCollection(medicineItems);
        RegisterItemCollection(specialItems);
    }
    
    private void RegisterItemCollection(List<Item> items)
    {
        foreach (Item item in items)
        {
            if (!ItemSystem.Instance.allItems.Contains(item))
            {
                ItemSystem.Instance.allItems.Add(item);
            }
        }
    }
    
    private void CreateFoodItems()
    {
        // Basic Food
        foodItems.Add(new Item
        {
            itemId = "food_kibble",
            itemName = "Basic Kibble",
            description = "Standard pet food. Satisfies hunger but not very exciting.",
            itemType = ItemType.Food,
            rarity = ItemRarity.Common,
            price = 10,
            hungerEffect = 20f,
            moodEffect = 5f,
            energyEffect = 10f
        });
        
        foodItems.Add(new Item
        {
            itemId = "food_premium_kibble",
            itemName = "Premium Kibble",
            description = "Higher quality pet food with better nutrition.",
            itemType = ItemType.Food,
            rarity = ItemRarity.Uncommon,
            price = 25,
            hungerEffect = 30f,
            moodEffect = 10f,
            energyEffect = 15f
        });
        
        foodItems.Add(new Item
        {
            itemId = "food_gourmet_meal",
            itemName = "Gourmet Pet Meal",
            description = "Delicious gourmet meal that pets absolutely love.",
            itemType = ItemType.Food,
            rarity = ItemRarity.Rare,
            price = 50,
            hungerEffect = 50f,
            moodEffect = 20f,
            energyEffect = 25f
        });
        
        // Treats
        foodItems.Add(new Item
        {
            itemId = "food_basic_treat",
            itemName = "Basic Treat",
            description = "A simple treat that pets enjoy.",
            itemType = ItemType.Food,
            rarity = ItemRarity.Common,
            price = 5,
            hungerEffect = 5f,
            moodEffect = 10f,
            energyEffect = 5f
        });
        
        foodItems.Add(new Item
        {
            itemId = "food_luxury_treat",
            itemName = "Luxury Treat",
            description = "A fancy treat that pets absolutely adore.",
            itemType = ItemType.Food,
            rarity = ItemRarity.Uncommon,
            price = 15,
            hungerEffect = 10f,
            moodEffect = 20f,
            energyEffect = 10f
        });
        
        // Special Foods
        foodItems.Add(new Item
        {
            itemId = "food_energy_boost",
            itemName = "Energy Boost Snack",
            description = "A special snack that provides a significant energy boost.",
            itemType = ItemType.Food,
            rarity = ItemRarity.Rare,
            price = 40,
            hungerEffect = 15f,
            moodEffect = 10f,
            energyEffect = 40f
        });
        
        foodItems.Add(new Item
        {
            itemId = "food_mood_enhancer",
            itemName = "Mood Enhancing Treat",
            description = "A special treat that significantly improves mood.",
            itemType = ItemType.Food,
            rarity = ItemRarity.Rare,
            price = 40,
            hungerEffect = 10f,
            moodEffect = 40f,
            energyEffect = 10f
        });
        
        foodItems.Add(new Item
        {
            itemId = "food_golden_apple",
            itemName = "Golden Apple",
            description = "A rare magical fruit that improves all stats.",
            itemType = ItemType.Food,
            rarity = ItemRarity.Epic,
            price = 100,
            hungerEffect = 30f,
            moodEffect = 30f,
            energyEffect = 30f,
            healthEffect = 30f,
            cleanlinessEffect = 10f
        });
        
        // Pet-specific foods
        foodItems.Add(new Item
        {
            itemId = "food_dragon_chili",
            itemName = "Dragon's Chili",
            description = "Extremely spicy food that dragons love.",
            itemType = ItemType.Food,
            rarity = ItemRarity.Uncommon,
            price = 35,
            hungerEffect = 40f,
            moodEffect = 15f,
            energyEffect = 20f
        });
        
        foodItems.Add(new Item
        {
            itemId = "food_unicorn_berries",
            itemName = "Rainbow Berries",
            description = "Colorful berries that unicorns adore.",
            itemType = ItemType.Food,
            rarity = ItemRarity.Uncommon,
            price = 35,
            hungerEffect = 35f,
            moodEffect = 25f,
            energyEffect = 15f
        });
        
        foodItems.Add(new Item
        {
            itemId = "food_robot_oil",
            itemName = "Premium Robot Oil",
            description = "High-grade lubricant for robot pets.",
            itemType = ItemType.Food,
            rarity = ItemRarity.Uncommon,
            price = 30,
            hungerEffect = 30f,
            moodEffect = 10f,
            energyEffect = 30f
        });
    }
    
    private void CreateToyItems()
    {
        // Basic Toys
        toyItems.Add(new Item
        {
            itemId = "toy_ball",
            itemName = "Bouncy Ball",
            description = "A simple ball that pets love to play with.",
            itemType = ItemType.Toy,
            rarity = ItemRarity.Common,
            price = 15,
            moodEffect = 15f,
            energyEffect = -10f
        });
        
        toyItems.Add(new Item
        {
            itemId = "toy_plush",
            itemName = "Plush Toy",
            description = "A soft plush toy that pets can cuddle with.",
            itemType = ItemType.Toy,
            rarity = ItemRarity.Common,
            price = 20,
            moodEffect = 20f,
            energyEffect = -5f
        });
        
        // Interactive Toys
        toyItems.Add(new Item
        {
            itemId = "toy_puzzle",
            itemName = "Puzzle Toy",
            description = "An interactive puzzle that stimulates pets mentally.",
            itemType = ItemType.Toy,
            rarity = ItemRarity.Uncommon,
            price = 35,
            moodEffect = 25f,
            energyEffect = -15f
        });
        
        toyItems.Add(new Item
        {
            itemId = "toy_automatic",
            itemName = "Automatic Toy",
            description = "A battery-powered toy that moves on its own.",
            itemType = ItemType.Toy,
            rarity = ItemRarity.Uncommon,
            price = 45,
            moodEffect = 30f,
            energyEffect = -20f
        });
        
        // Premium Toys
        toyItems.Add(new Item
        {
            itemId = "toy_premium_ball",
            itemName = "Premium Play Ball",
            description = "A high-quality ball with unpredictable bounces.",
            itemType = ItemType.Toy,
            rarity = ItemRarity.Rare,
            price = 60,
            moodEffect = 35f,
            energyEffect = -25f
        });
        
        toyItems.Add(new Item
        {
            itemId = "toy_flying_disc",
            itemName = "Flying Disc",
            description = "A disc that glides through the air for pets to catch.",
            itemType = ItemType.Toy,
            rarity = ItemRarity.Rare,
            price = 50,
            moodEffect = 40f,
            energyEffect = -30f
        });
        
        // Special Toys
        toyItems.Add(new Item
        {
            itemId = "toy_magical_orb",
            itemName = "Magical Orb",
            description = "A mysterious orb that changes colors and patterns.",
            itemType = ItemType.Toy,
            rarity = ItemRarity.Epic,
            price = 120,
            moodEffect = 50f,
            energyEffect = -20f,
            hasSpecialEffect = true,
            specialEffectDescription = "Has a chance to grant a small amount of experience when played with."
        });
        
        // Pet-specific toys
        toyItems.Add(new Item
        {
            itemId = "toy_dragon_fireproof",
            itemName = "Fireproof Chew Toy",
            description = "A special toy that can withstand dragon fire.",
            itemType = ItemType.Toy,
            rarity = ItemRarity.Uncommon,
            price = 55,
            moodEffect = 45f,
            energyEffect = -25f
        });
        
        toyItems.Add(new Item
        {
            itemId = "toy_unicorn_rainbow",
            itemName = "Rainbow Prism",
            description = "A crystal that creates beautiful rainbow patterns.",
            itemType = ItemType.Toy,
            rarity = ItemRarity.Uncommon,
            price = 55,
            moodEffect = 45f,
            energyEffect = -15f
        });
        
        toyItems.Add(new Item
        {
            itemId = "toy_robot_puzzle",
            itemName = "Circuit Puzzle",
            description = "An electronic puzzle perfect for robot pets.",
            itemType = ItemType.Toy,
            rarity = ItemRarity.Uncommon,
            price = 50,
            moodEffect = 40f,
            energyEffect = -10f
        });
    }
    
    private void CreateClothingItems()
    {
        // Basic Clothing
        clothingItems.Add(new Item
        {
            itemId = "clothing_basic_collar",
            itemName = "Basic Collar",
            description = "A simple collar for pets.",
            itemType = ItemType.Clothing,
            rarity = ItemRarity.Common,
            price = 25,
            moodEffect = 5f
        });
        
        clothingItems.Add(new Item
        {
            itemId = "clothing_bandana",
            itemName = "Stylish Bandana",
            description = "A fashionable bandana for pets to wear.",
            itemType = ItemType.Clothing,
            rarity = ItemRarity.Common,
            price = 30,
            moodEffect = 8f
        });
        
        // Seasonal Clothing
        clothingItems.Add(new Item
        {
            itemId = "clothing_winter_sweater",
            itemName = "Winter Sweater",
            description = "A warm sweater for cold weather.",
            itemType = ItemType.Clothing,
            rarity = ItemRarity.Uncommon,
            price = 50,
            moodEffect = 10f,
            healthEffect = 5f
        });
        
        clothingItems.Add(new Item
        {
            itemId = "clothing_summer_hat",
            itemName = "Summer Hat",
            description = "A cool hat for hot summer days.",
            itemType = ItemType.Clothing,
            rarity = ItemRarity.Uncommon,
            price = 45,
            moodEffect = 10f,
            healthEffect = 5f
        });
        
        // Fancy Clothing
        clothingItems.Add(new Item
        {
            itemId = "clothing_fancy_outfit",
            itemName = "Fancy Outfit",
            description = "An elegant outfit for special occasions.",
            itemType = ItemType.Clothing,
            rarity = ItemRarity.Rare,
            price = 80,
            moodEffect = 15f
        });
        
        clothingItems.Add(new Item
        {
            itemId = "clothing_costume",
            itemName = "Cute Costume",
            description = "An adorable costume for pets.",
            itemType = ItemType.Clothing,
            rarity = ItemRarity.Rare,
            price = 75,
            moodEffect = 20f
        });
        
        // Special Clothing
        clothingItems.Add(new Item
        {
            itemId = "clothing_royal_crown",
            itemName = "Royal Crown",
            description = "A majestic crown fit for pet royalty.",
            itemType = ItemType.Clothing,
            rarity = ItemRarity.Epic,
            price = 150,
            moodEffect = 25f,
            hasSpecialEffect = true,
            specialEffectDescription = "Increases the rate at which your pet gains experience."
        });
        
        // Pet-specific clothing
        clothingItems.Add(new Item
        {
            itemId = "clothing_dragon_armor",
            itemName = "Dragon Scale Armor",
            description = "Protective armor made from dragon scales.",
            itemType = ItemType.Clothing,
            rarity = ItemRarity.Rare,
            price = 100,
            moodEffect = 15f,
            healthEffect = 10f
        });
        
        clothingItems.Add(new Item
        {
            itemId = "clothing_unicorn_horn",
            itemName = "Decorative Horn Cover",
            description = "A beautiful ornament for unicorn horns.",
            itemType = ItemType.Clothing,
            rarity = ItemRarity.Rare,
            price = 90,
            moodEffect = 20f
        });
        
        clothingItems.Add(new Item
        {
            itemId = "clothing_robot_upgrade",
            itemName = "Decorative Shell Upgrade",
            description = "A stylish upgrade for robot pet exteriors.",
            itemType = ItemType.Clothing,
            rarity = ItemRarity.Rare,
            price = 95,
            moodEffect = 15f,
            healthEffect = 5f
        });
    }
    
    private void CreateFurnitureItems()
    {
        // Basic Furniture
        furnitureItems.Add(new Item
        {
            itemId = "furniture_basic_bed",
            itemName = "Basic Pet Bed",
            description = "A simple, comfortable bed for pets.",
            itemType = ItemType.Furniture,
            rarity = ItemRarity.Common,
            price = 50,
            energyEffect = 10f,
            moodEffect = 5f
        });
        
        furnitureItems.Add(new Item
        {
            itemId = "furniture_food_bowl",
            itemName = "Food Bowl",
            description = "A standard bowl for pet food.",
            itemType = ItemType.Furniture,
            rarity = ItemRarity.Common,
            price = 30,
            hungerEffect = 5f
        });
        
        // Comfort Furniture
        furnitureItems.Add(new Item
        {
            itemId = "furniture_luxury_bed",
            itemName = "Luxury Pet Bed",
            description = "A premium, extra comfortable bed for pets.",
            itemType = ItemType.Furniture,
            rarity = ItemRarity.Uncommon,
            price = 100,
            energyEffect = 20f,
            moodEffect = 10f
        });
        
        furnitureItems.Add(new Item
        {
            itemId = "furniture_pet_sofa",
            itemName = "Pet Sofa",
            description = "A small sofa designed for pets to lounge on.",
            itemType = ItemType.Furniture,
            rarity = ItemRarity.Uncommon,
            price = 120,
            energyEffect = 15f,
            moodEffect = 15f
        });
        
        // Play Furniture
        furnitureItems.Add(new Item
        {
            itemId = "furniture_climbing_tree",
            itemName = "Climbing Tree",
            description = "A structure for pets to climb and play on.",
            itemType = ItemType.Furniture,
            rarity = ItemRarity.Rare,
            price = 150,
            moodEffect = 25f,
            energyEffect = -15f
        });
        
        furnitureItems.Add(new Item
        {
            itemId = "furniture_playground",
            itemName = "Pet Playground",
            description = "A small playground with various activities for pets.",
            itemType = ItemType.Furniture,
            rarity = ItemRarity.Rare,
            price = 200,
            moodEffect = 30f,
            energyEffect = -20f
        });
        
        // Special Furniture
        furnitureItems.Add(new Item
        {
            itemId = "furniture_magical_bed",
            itemName = "Enchanted Pet Bed",
            description = "A magical bed that helps pets recover faster while sleeping.",
            itemType = ItemType.Furniture,
            rarity = ItemRarity.Epic,
            price = 300,
            energyEffect = 30f,
            moodEffect = 20f,
            healthEffect = 10f,
            hasSpecialEffect = true,
            specialEffectDescription = "Pets recover energy twice as fast while sleeping on this bed."
        });
        
        // Pet-specific furniture
        furnitureItems.Add(new Item
        {
            itemId = "furniture_dragon_nest",
            itemName = "Dragon's Nest",
            description = "A heat-resistant nest perfect for dragons.",
            itemType = ItemType.Furniture,
            rarity = ItemRarity.Rare,
            price = 250,
            energyEffect = 25f,
            moodEffect = 20f
        });
        
        furnitureItems.Add(new Item
        {
            itemId = "furniture_unicorn_stable",
            itemName = "Magical Stable",
            description = "A beautiful stable with magical decorations for unicorns.",
            itemType = ItemType.Furniture,
            rarity = ItemRarity.Rare,
            price = 250,
            energyEffect = 25f,
            moodEffect = 20f
        });
        
        furnitureItems.Add(new Item
        {
            itemId = "furniture_robot_dock",
            itemName = "Charging Dock",
            description = "A specialized dock where robot pets can recharge.",
            itemType = ItemType.Furniture,
            rarity = ItemRarity.Rare,
            price = 220,
            energyEffect = 30f,
            moodEffect = 15f
        });
    }
    
    private void CreateDecorationItems()
    {
        // Basic Decorations
        decorationItems.Add(new Item
        {
            itemId = "decor_small_plant",
            itemName = "Small Plant",
            description = "A small decorative plant for your pet's home.",
            itemType = ItemType.Decoration,
            rarity = ItemRarity.Common,
            price = 25,
            moodEffect = 3f
        });
        
        decorationItems.Add(new Item
        {
            itemId = "decor_wall_art",
            itemName = "Pet Wall Art",
            description = "Cute pet-themed wall art.",
            itemType = ItemType.Decoration,
            rarity = ItemRarity.Common,
            price = 30,
            moodEffect = 5f
        });
        
        // Themed Decorations
        decorationItems.Add(new Item
        {
            itemId = "decor_beach_theme",
            itemName = "Beach Theme Set",
            description = "A set of beach-themed decorations.",
            itemType = ItemType.Decoration,
            rarity = ItemRarity.Uncommon,
            price = 60,
            moodEffect = 8f
        });
        
        decorationItems.Add(new Item
        {
            itemId = "decor_forest_theme",
            itemName = "Forest Theme Set",
            description = "A set of forest-themed decorations.",
            itemType = ItemType.Decoration,
            rarity = ItemRarity.Uncommon,
            price = 60,
            moodEffect = 8f
        });
        
        // Premium Decorations
        decorationItems.Add(new Item
        {
            itemId = "decor_aquarium",
            itemName = "Decorative Aquarium",
            description = "A beautiful aquarium with colorful fish.",
            itemType = ItemType.Decoration,
            rarity = ItemRarity.Rare,
            price = 120,
            moodEffect = 15f
        });
        
        decorationItems.Add(new Item
        {
            itemId = "decor_fountain",
            itemName = "Indoor Fountain",
            description = "A soothing water fountain for indoor use.",
            itemType = ItemType.Decoration,
            rarity = ItemRarity.Rare,
            price = 150,
            moodEffect = 18f
        });
        
        // Special Decorations
        decorationItems.Add(new Item
        {
            itemId = "decor_magical_crystal",
            itemName = "Magical Crystal",
            description = "A mysterious crystal that emits a calming glow.",
            itemType = ItemType.Decoration,
            rarity = ItemRarity.Epic,
            price = 250,
            moodEffect = 25f,
            hasSpecialEffect = true,
            specialEffectDescription = "Slowly restores mood for all pets in the home."
        });
        
        // Seasonal Decorations
        decorationItems.Add(new Item
        {
            itemId = "decor_winter_set",
            itemName = "Winter Holiday Set",
            description = "Festive decorations for the winter holidays.",
            itemType = ItemType.Decoration,
            rarity = ItemRarity.Uncommon,
            price = 80,
            moodEffect = 12f
        });
        
        decorationItems.Add(new Item
        {
            itemId = "decor_halloween_set",
            itemName = "Halloween Set",
            description = "Spooky decorations for Halloween.",
            itemType = ItemType.Decoration,
            rarity = ItemRarity.Uncommon,
            price = 80,
            moodEffect = 12f
        });
    }
    
    private void CreateMedicineItems()
    {
        // Basic Medicines
        medicineItems.Add(new Item
        {
            itemId = "medicine_basic",
            itemName = "Basic Medicine",
            description = "A simple medicine that helps with minor health issues.",
            itemType = ItemType.Medicine,
            rarity = ItemRarity.Common,
            price = 30,
            healthEffect = 20f,
            moodEffect = -5f
        });
        
        medicineItems.Add(new Item
        {
            itemId = "medicine_vitamins",
            itemName = "Pet Vitamins",
            description = "Nutritional supplements to keep pets healthy.",
            itemType = ItemType.Medicine,
            rarity = ItemRarity.Common,
            price = 25,
            healthEffect = 10f,
            moodEffect = 5f
        });
        
        // Specialized Medicines
        medicineItems.Add(new Item
        {
            itemId = "medicine_energy_booster",
            itemName = "Energy Booster",
            description = "A medicine that quickly restores energy.",
            itemType = ItemType.Medicine,
            rarity = ItemRarity.Uncommon,
            price = 45,
            energyEffect = 40f,
            healthEffect = 5f
        });
        
        medicineItems.Add(new Item
        {
            itemId = "medicine_mood_enhancer",
            itemName = "Mood Enhancer",
            description = "A medicine that improves mood.",
            itemType = ItemType.Medicine,
            rarity = ItemRarity.Uncommon,
            price = 45,
            moodEffect = 40f,
            healthEffect = 5f
        });
        
        // Advanced Medicines
        medicineItems.Add(new Item
        {
            itemId = "medicine_advanced",
            itemName = "Advanced Medicine",
            description = "A powerful medicine for treating serious health issues.",
            itemType = ItemType.Medicine,
            rarity = ItemRarity.Rare,
            price = 80,
            healthEffect = 50f,
            moodEffect = -10f
        });
        
        medicineItems.Add(new Item
        {
            itemId = "medicine_wellness_tonic",
            itemName = "Wellness Tonic",
            description = "A comprehensive tonic that improves overall well-being.",
            itemType = ItemType.Medicine,
            rarity = ItemRarity.Rare,
            price = 100,
            healthEffect = 30f,
            moodEffect = 20f,
            energyEffect = 20f
        });
        
        // Special Medicines
        medicineItems.Add(new Item
        {
            itemId = "medicine_miracle_cure",
            itemName = "Miracle Cure",
            description = "A legendary medicine that can cure any ailment.",
            itemType = ItemType.Medicine,
            rarity = ItemRarity.Epic,
            price = 200,
            healthEffect = 100f,
            moodEffect = 30f,
            energyEffect = 30f,
            hasSpecialEffect = true,
            specialEffectDescription = "Instantly cures any status ailments affecting the pet."
        });
        
        // Pet-specific medicines
        medicineItems.Add(new Item
        {
            itemId = "medicine_dragon_scales",
            itemName = "Dragon Scale Powder",
            description = "A special medicine made from dragon scales.",
            itemType = ItemType.Medicine,
            rarity = ItemRarity.Rare,
            price = 120,
            healthEffect = 60f,
            moodEffect = 10f
        });
        
        medicineItems.Add(new Item
        {
            itemId = "medicine_unicorn_tears",
            itemName = "Unicorn Tear Elixir",
            description = "A magical elixir with healing properties.",
            itemType = ItemType.Medicine,
            rarity = ItemRarity.Rare,
            price = 130,
            healthEffect = 70f,
            moodEffect = 15f
        });
        
        medicineItems.Add(new Item
        {
            itemId = "medicine_robot_repair",
            itemName = "Nano Repair Kit",
            description = "Advanced nanobots that repair robot pets.",
            itemType = ItemType.Medicine,
            rarity = ItemRarity.Rare,
            price = 110,
            healthEffect = 65f,
            energyEffect = 20f
        });
    }
    
    private void CreateSpecialItems()
    {
        // Quest Items
        specialItems.Add(new Item
        {
            itemId = "special_forest_key",
            itemName = "Enchanted Forest Key",
            description = "A magical key that unlocks the Enchanted Forest.",
            itemType = ItemType.Special,
            rarity = ItemRarity.Epic,
            price = 0, // Not for sale
            isStackable = false
        });
        
        specialItems.Add(new Item
        {
            itemId = "special_sky_key",
            itemName = "Sky Island Key",
            description = "A magical key that grants access to the Sky Islands.",
            itemType = ItemType.Special,
            rarity = ItemRarity.Epic,
            price = 0, // Not for sale
            isStackable = false
        });
        
        specialItems.Add(new Item
        {
            itemId = "special_ocean_key",
            itemName = "Ocean Realm Key",
            description = "A magical key that opens the way to the Underwater Realm.",
            itemType = ItemType.Special,
            rarity = ItemRarity.Epic,
            price = 0, // Not for sale
            isStackable = false
        });
        
        // Rare Collectibles
        specialItems.Add(new Item
        {
            itemId = "special_rare_gem",
            itemName = "Rare Gem",
            description = "A beautiful gem that can be sold for a high price.",
            itemType = ItemType.Special,
            rarity = ItemRarity.Rare,
            price = 500,
            isStackable = true
        });
        
        specialItems.Add(new Item
        {
            itemId = "special_ancient_artifact",
            itemName = "Ancient Artifact",
            description = "A mysterious artifact from an ancient civilization.",
            itemType = ItemType.Special,
            rarity = ItemRarity.Epic,
            price = 1000,
            isStackable = true
        });
        
        // Special Use Items
        specialItems.Add(new Item
        {
            itemId = "special_pet_whistle",
            itemName = "Pet Whistle",
            description = "A special whistle that instantly calls your pet to your side.",
            itemType = ItemType.Special,
            rarity = ItemRarity.Uncommon,
            price = 150,
            isStackable = true,
            maxStackSize = 10,
            hasSpecialEffect = true,
            specialEffectDescription = "Instantly teleports your pet to your location."
        });
        
        specialItems.Add(new Item
        {
            itemId = "special_experience_boost",
            itemName = "Experience Boost",
            description = "A special item that temporarily increases experience gain.",
            itemType = ItemType.Special,
            rarity = ItemRarity.Rare,
            price = 200,
            isStackable = true,
            maxStackSize = 5,
            hasSpecialEffect = true,
            specialEffectDescription = "Doubles experience gain for 1 hour."
        });
        
        specialItems.Add(new Item
        {
            itemId = "special_stat_reset",
            itemName = "Stat Reset Potion",
            description = "A magical potion that allows you to redistribute your pet's stats.",
            itemType = ItemType.Special,
            rarity = ItemRarity.Epic,
            price = 300,
            isStackable = true,
            maxStackSize = 3,
            hasSpecialEffect = true,
            specialEffectDescription = "Allows you to reset and redistribute your pet's stat points."
        });
        
        // Legendary Items
        specialItems.Add(new Item
        {
            itemId = "special_legendary_egg",
            itemName = "Legendary Pet Egg",
            description = "A mysterious egg that contains a legendary pet.",
            itemType = ItemType.Special,
            rarity = ItemRarity.Legendary,
            price = 5000,
            isStackable = false,
            hasSpecialEffect = true,
            specialEffectDescription = "Contains a rare legendary pet that can be hatched."
        });
    }
}