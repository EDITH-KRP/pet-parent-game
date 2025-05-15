using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDatabase : MonoBehaviour
{
    public static QuestDatabase Instance;
    
    [Header("Quest Collections")]
    public List<Quest> tutorialQuests = new List<Quest>();
    public List<Quest> petCareQuests = new List<Quest>();
    public List<Quest> explorationQuests = new List<Quest>();
    public List<Quest> collectionQuests = new List<Quest>();
    public List<Quest> socialQuests = new List<Quest>();
    public List<Quest> specialQuests = new List<Quest>();
    public List<Quest> dailyQuests = new List<Quest>();
    public List<Quest> weeklyQuests = new List<Quest>();
    
    [Header("Achievement Collections")]
    public List<Achievement> generalAchievements = new List<Achievement>();
    public List<Achievement> petAchievements = new List<Achievement>();
    public List<Achievement> explorationAchievements = new List<Achievement>();
    public List<Achievement> collectionAchievements = new List<Achievement>();
    public List<Achievement> socialAchievements = new List<Achievement>();
    
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
        
        // Initialize quest database
        InitializeQuests();
    }
    
    private void Start()
    {
        // Register all quests with the QuestSystem
        RegisterQuestsWithSystem();
    }
    
    private void InitializeQuests()
    {
        // Create tutorial quests
        CreateTutorialQuests();
        
        // Create pet care quests
        CreatePetCareQuests();
        
        // Create exploration quests
        CreateExplorationQuests();
        
        // Create collection quests
        CreateCollectionQuests();
        
        // Create social quests
        CreateSocialQuests();
        
        // Create special quests
        CreateSpecialQuests();
        
        // Create daily quests
        CreateDailyQuests();
        
        // Create weekly quests
        CreateWeeklyQuests();
        
        // Create achievements
        CreateAchievements();
    }
    
    private void RegisterQuestsWithSystem()
    {
        if (QuestSystem.Instance == null)
            return;
            
        // Register all quests with the QuestSystem
        RegisterQuestCollection(tutorialQuests);
        RegisterQuestCollection(petCareQuests);
        RegisterQuestCollection(explorationQuests);
        RegisterQuestCollection(collectionQuests);
        RegisterQuestCollection(socialQuests);
        RegisterQuestCollection(specialQuests);
        RegisterQuestCollection(dailyQuests);
        RegisterQuestCollection(weeklyQuests);
        
        // Register all achievements
        RegisterAchievementCollection(generalAchievements);
        RegisterAchievementCollection(petAchievements);
        RegisterAchievementCollection(explorationAchievements);
        RegisterAchievementCollection(collectionAchievements);
        RegisterAchievementCollection(socialAchievements);
    }
    
    private void RegisterQuestCollection(List<Quest> quests)
    {
        foreach (Quest quest in quests)
        {
            if (!QuestSystem.Instance.allQuests.Contains(quest))
            {
                QuestSystem.Instance.allQuests.Add(quest);
            }
        }
    }
    
    private void RegisterAchievementCollection(List<Achievement> achievements)
    {
        foreach (Achievement achievement in achievements)
        {
            if (!QuestSystem.Instance.achievements.Contains(achievement))
            {
                QuestSystem.Instance.achievements.Add(achievement);
            }
        }
    }
    
    private void CreateTutorialQuests()
    {
        // Welcome to Animora
        Quest welcomeQuest = new Quest
        {
            questId = "tutorial_welcome",
            title = "Welcome to Animora",
            description = "Learn the basics of pet care in Animora.",
            questType = QuestType.PetCare,
            status = QuestStatus.NotStarted,
            isHidden = false,
            isRepeatable = false,
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    objectiveId = "feed_pet",
                    description = "Feed your pet",
                    requiredAmount = 1
                },
                new QuestObjective
                {
                    objectiveId = "play_with_pet",
                    description = "Play with your pet",
                    requiredAmount = 1
                }
            },
            reward = new QuestReward
            {
                currency = 50,
                experience = 20,
                itemIds = new List<string> { "food_basic_treat" },
                itemQuantities = new List<int> { 3 }
            }
        };
        
        tutorialQuests.Add(welcomeQuest);
        
        // Home Sweet Home
        Quest homeQuest = new Quest
        {
            questId = "tutorial_home",
            title = "Home Sweet Home",
            description = "Make your pet feel at home.",
            questType = QuestType.PetCare,
            status = QuestStatus.NotStarted,
            isHidden = false,
            isRepeatable = false,
            prerequisiteQuestIds = new List<string> { "tutorial_welcome" },
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    objectiveId = "clean_pet",
                    description = "Clean your pet",
                    requiredAmount = 1
                },
                new QuestObjective
                {
                    objectiveId = "pet_sleep",
                    description = "Let your pet sleep",
                    requiredAmount = 1
                }
            },
            reward = new QuestReward
            {
                currency = 75,
                experience = 30,
                itemIds = new List<string> { "furniture_basic_bed" },
                itemQuantities = new List<int> { 1 }
            }
        };
        
        tutorialQuests.Add(homeQuest);
        
        // Exploring Animora
        Quest exploreQuest = new Quest
        {
            questId = "tutorial_explore",
            title = "Exploring Animora",
            description = "Visit different locations in Animora.",
            questType = QuestType.Exploration,
            status = QuestStatus.NotStarted,
            isHidden = false,
            isRepeatable = false,
            prerequisiteQuestIds = new List<string> { "tutorial_home" },
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    objectiveId = "visit_park",
                    description = "Visit the Pet Park",
                    requiredAmount = 1
                },
                new QuestObjective
                {
                    objectiveId = "visit_cafe",
                    description = "Visit the Pet Café",
                    requiredAmount = 1
                }
            },
            reward = new QuestReward
            {
                currency = 100,
                experience = 50,
                itemIds = new List<string> { "toy_ball" },
                itemQuantities = new List<int> { 1 }
            }
        };
        
        tutorialQuests.Add(exploreQuest);
        
        // Shopping Spree
        Quest shoppingQuest = new Quest
        {
            questId = "tutorial_shopping",
            title = "Shopping Spree",
            description = "Learn how to shop for your pet.",
            questType = QuestType.PetCare,
            status = QuestStatus.NotStarted,
            isHidden = false,
            isRepeatable = false,
            prerequisiteQuestIds = new List<string> { "tutorial_explore" },
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    objectiveId = "buy_food",
                    description = "Buy food for your pet",
                    requiredAmount = 1
                },
                new QuestObjective
                {
                    objectiveId = "buy_toy",
                    description = "Buy a toy for your pet",
                    requiredAmount = 1
                }
            },
            reward = new QuestReward
            {
                currency = 150,
                experience = 75,
                itemIds = new List<string> { "clothing_basic_collar" },
                itemQuantities = new List<int> { 1 }
            }
        };
        
        tutorialQuests.Add(shoppingQuest);
    }
    
    private void CreatePetCareQuests()
    {
        // Gourmet Pet
        Quest gourmetQuest = new Quest
        {
            questId = "petcare_gourmet",
            title = "Gourmet Pet",
            description = "Feed your pet a variety of foods.",
            questType = QuestType.PetCare,
            status = QuestStatus.NotStarted,
            isHidden = false,
            isRepeatable = false,
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    objectiveId = "feed_different_foods",
                    description = "Feed your pet different types of food",
                    requiredAmount = 5
                }
            },
            reward = new QuestReward
            {
                currency = 100,
                experience = 50,
                itemIds = new List<string> { "food_gourmet_meal" },
                itemQuantities = new List<int> { 1 }
            }
        };
        
        petCareQuests.Add(gourmetQuest);
        
        // Playtime Fun
        Quest playtimeQuest = new Quest
        {
            questId = "petcare_playtime",
            title = "Playtime Fun",
            description = "Keep your pet happy with plenty of play.",
            questType = QuestType.PetCare,
            status = QuestStatus.NotStarted,
            isHidden = false,
            isRepeatable = false,
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    objectiveId = "play_with_pet",
                    description = "Play with your pet",
                    requiredAmount = 10
                },
                new QuestObjective
                {
                    objectiveId = "max_mood",
                    description = "Get your pet's mood to maximum",
                    requiredAmount = 1
                }
            },
            reward = new QuestReward
            {
                currency = 150,
                experience = 75,
                itemIds = new List<string> { "toy_premium_ball" },
                itemQuantities = new List<int> { 1 }
            }
        };
        
        petCareQuests.Add(playtimeQuest);
        
        // Squeaky Clean
        Quest cleanQuest = new Quest
        {
            questId = "petcare_clean",
            title = "Squeaky Clean",
            description = "Keep your pet clean and healthy.",
            questType = QuestType.PetCare,
            status = QuestStatus.NotStarted,
            isHidden = false,
            isRepeatable = false,
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    objectiveId = "clean_pet",
                    description = "Clean your pet",
                    requiredAmount = 5
                },
                new QuestObjective
                {
                    objectiveId = "max_cleanliness",
                    description = "Get your pet's cleanliness to maximum",
                    requiredAmount = 1
                }
            },
            reward = new QuestReward
            {
                currency = 125,
                experience = 60,
                itemIds = new List<string> { "medicine_vitamins" },
                itemQuantities = new List<int> { 3 }
            }
        };
        
        petCareQuests.Add(cleanQuest);
        
        // Health Matters
        Quest healthQuest = new Quest
        {
            questId = "petcare_health",
            title = "Health Matters",
            description = "Ensure your pet stays in good health.",
            questType = QuestType.PetCare,
            status = QuestStatus.NotStarted,
            isHidden = false,
            isRepeatable = false,
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    objectiveId = "heal_pet",
                    description = "Heal your pet",
                    requiredAmount = 3
                },
                new QuestObjective
                {
                    objectiveId = "max_health",
                    description = "Get your pet's health to maximum",
                    requiredAmount = 1
                }
            },
            reward = new QuestReward
            {
                currency = 175,
                experience = 85,
                itemIds = new List<string> { "medicine_wellness_tonic" },
                itemQuantities = new List<int> { 1 }
            }
        };
        
        petCareQuests.Add(healthQuest);
        
        // Level Up
        Quest levelQuest = new Quest
        {
            questId = "petcare_level",
            title = "Level Up",
            description = "Help your pet grow and develop.",
            questType = QuestType.PetCare,
            status = QuestStatus.NotStarted,
            isHidden = false,
            isRepeatable = false,
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    objectiveId = "pet_level",
                    description = "Reach level 5 with your pet",
                    requiredAmount = 1
                }
            },
            reward = new QuestReward
            {
                currency = 300,
                experience = 150,
                itemIds = new List<string> { "special_experience_boost" },
                itemQuantities = new List<int> { 1 }
            }
        };
        
        petCareQuests.Add(levelQuest);
        
        // Perfect Balance
        Quest balanceQuest = new Quest
        {
            questId = "petcare_balance",
            title = "Perfect Balance",
            description = "Maintain all of your pet's needs at high levels.",
            questType = QuestType.PetCare,
            status = QuestStatus.NotStarted,
            isHidden = false,
            isRepeatable = false,
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    objectiveId = "all_stats_high",
                    description = "Have all pet stats above 80 simultaneously",
                    requiredAmount = 1
                }
            },
            reward = new QuestReward
            {
                currency = 500,
                experience = 250,
                itemIds = new List<string> { "food_golden_apple" },
                itemQuantities = new List<int> { 1 }
            }
        };
        
        petCareQuests.Add(balanceQuest);
    }
    
    private void CreateExplorationQuests()
    {
        // Park Explorer
        Quest parkQuest = new Quest
        {
            questId = "explore_park",
            title = "Park Explorer",
            description = "Explore all areas of the Pet Park.",
            questType = QuestType.Exploration,
            status = QuestStatus.NotStarted,
            isHidden = false,
            isRepeatable = false,
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    objectiveId = "visit_park_areas",
                    description = "Visit different areas in the Pet Park",
                    requiredAmount = 5
                },
                new QuestObjective
                {
                    objectiveId = "play_in_park",
                    description = "Play with your pet in the park",
                    requiredAmount = 3
                }
            },
            reward = new QuestReward
            {
                currency = 150,
                experience = 75,
                itemIds = new List<string> { "toy_flying_disc" },
                itemQuantities = new List<int> { 1 }
            }
        };
        
        explorationQuests.Add(parkQuest);
        
        // Café Connoisseur
        Quest cafeQuest = new Quest
        {
            questId = "explore_cafe",
            title = "Café Connoisseur",
            description = "Become a regular at the Pet Café.",
            questType = QuestType.Exploration,
            status = QuestStatus.NotStarted,
            isHidden = false,
            isRepeatable = false,
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    objectiveId = "visit_cafe",
                    description = "Visit the Pet Café",
                    requiredAmount = 5
                },
                new QuestObjective
                {
                    objectiveId = "feed_at_cafe",
                    description = "Feed your pet at the café",
                    requiredAmount = 3
                }
            },
            reward = new QuestReward
            {
                currency = 175,
                experience = 85,
                itemIds = new List<string> { "food_luxury_treat" },
                itemQuantities = new List<int> { 5 }
            }
        };
        
        explorationQuests.Add(cafeQuest);
        
        // Spa Day
        Quest spaQuest = new Quest
        {
            questId = "explore_spa",
            title = "Spa Day",
            description = "Treat your pet to a day at the Spa & Clinic.",
            questType = QuestType.Exploration,
            status = QuestStatus.NotStarted,
            isHidden = false,
            isRepeatable = false,
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    objectiveId = "visit_spa",
                    description = "Visit the Pet Spa & Clinic",
                    requiredAmount = 3
                },
                new QuestObjective
                {
                    objectiveId = "clean_at_spa",
                    description = "Clean your pet at the spa",
                    requiredAmount = 2
                },
                new QuestObjective
                {
                    objectiveId = "heal_at_clinic",
                    description = "Heal your pet at the clinic",
                    requiredAmount = 1
                }
            },
            reward = new QuestReward
            {
                currency = 200,
                experience = 100,
                itemIds = new List<string> { "medicine_advanced" },
                itemQuantities = new List<int> { 1 }
            }
        };
        
        explorationQuests.Add(spaQuest);
        
        // Key to the Forest
        Quest forestKeyQuest = new Quest
        {
            questId = "quest_forest_key",
            title = "Key to the Forest",
            description = "Find the key to unlock the Enchanted Forest.",
            questType = QuestType.Exploration,
            status = QuestStatus.NotStarted,
            isHidden = false,
            isRepeatable = false,
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    objectiveId = "find_forest_clues",
                    description = "Find clues about the forest key",
                    requiredAmount = 3
                },
                new QuestObjective
                {
                    objectiveId = "defeat_guardian",
                    description = "Defeat the forest guardian",
                    requiredAmount = 1
                }
            },
            reward = new QuestReward
            {
                currency = 300,
                experience = 150,
                itemIds = new List<string> { "special_forest_key" },
                itemQuantities = new List<int> { 1 },
                unlockedLocationId = "enchanted_forest"
            }
        };
        
        explorationQuests.Add(forestKeyQuest);
        
        // Forest Mysteries
        Quest forestQuest = new Quest
        {
            questId = "explore_forest",
            title = "Forest Mysteries",
            description = "Uncover the secrets of the Enchanted Forest.",
            questType = QuestType.Exploration,
            status = QuestStatus.NotStarted,
            isHidden = true, // Hidden until Enchanted Forest is unlocked
            isRepeatable = false,
            prerequisiteQuestIds = new List<string> { "quest_forest_key" },
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    objectiveId = "explore_forest_areas",
                    description = "Explore different areas of the Enchanted Forest",
                    requiredAmount = 5
                },
                new QuestObjective
                {
                    objectiveId = "find_forest_treasures",
                    description = "Find hidden treasures in the forest",
                    requiredAmount = 3
                }
            },
            reward = new QuestReward
            {
                currency = 400,
                experience = 200,
                itemIds = new List<string> { "special_rare_gem" },
                itemQuantities = new List<int> { 1 }
            }
        };
        
        explorationQuests.Add(forestQuest);
        
        // Key to the Sky
        Quest skyKeyQuest = new Quest
        {
            questId = "quest_sky_key",
            title = "Key to the Sky",
            description = "Find the key to access the Sky Islands.",
            questType = QuestType.Exploration,
            status = QuestStatus.NotStarted,
            isHidden = true, // Hidden until player reaches level 10
            isRepeatable = false,
            prerequisiteQuestIds = new List<string> { "explore_forest" },
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    objectiveId = "find_sky_clues",
                    description = "Find clues about the sky key",
                    requiredAmount = 3
                },
                new QuestObjective
                {
                    objectiveId = "craft_sky_key",
                    description = "Craft the sky key",
                    requiredAmount = 1
                }
            },
            reward = new QuestReward
            {
                currency = 500,
                experience = 250,
                itemIds = new List<string> { "special_sky_key" },
                itemQuantities = new List<int> { 1 },
                unlockedLocationId = "sky_islands"
            }
        };
        
        explorationQuests.Add(skyKeyQuest);
    }
    
    private void CreateCollectionQuests()
    {
        // Toy Collector
        Quest toyQuest = new Quest
        {
            questId = "collect_toys",
            title = "Toy Collector",
            description = "Collect a variety of toys for your pet.",
            questType = QuestType.Collection,
            status = QuestStatus.NotStarted,
            isHidden = false,
            isRepeatable = false,
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    objectiveId = "collect_toys",
                    description = "Collect different toys",
                    requiredAmount = 5
                }
            },
            reward = new QuestReward
            {
                currency = 200,
                experience = 100,
                itemIds = new List<string> { "toy_magical_orb" },
                itemQuantities = new List<int> { 1 }
            }
        };
        
        collectionQuests.Add(toyQuest);
        
        // Fashion Forward
        Quest fashionQuest = new Quest
        {
            questId = "collect_clothing",
            title = "Fashion Forward",
            description = "Collect a variety of clothing items for your pet.",
            questType = QuestType.Collection,
            status = QuestStatus.NotStarted,
            isHidden = false,
            isRepeatable = false,
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    objectiveId = "collect_clothing",
                    description = "Collect different clothing items",
                    requiredAmount = 5
                }
            },
            reward = new QuestReward
            {
                currency = 250,
                experience = 125,
                itemIds = new List<string> { "clothing_royal_crown" },
                itemQuantities = new List<int> { 1 }
            }
        };
        
        collectionQuests.Add(fashionQuest);
        
        // Home Decorator
        Quest decoratorQuest = new Quest
        {
            questId = "collect_furniture",
            title = "Home Decorator",
            description = "Collect and place furniture in your pet's home.",
            questType = QuestType.Collection,
            status = QuestStatus.NotStarted,
            isHidden = false,
            isRepeatable = false,
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    objectiveId = "collect_furniture",
                    description = "Collect different furniture items",
                    requiredAmount = 5
                },
                new QuestObjective
                {
                    objectiveId = "place_furniture",
                    description = "Place furniture in your pet's home",
                    requiredAmount = 3
                }
            },
            reward = new QuestReward
            {
                currency = 300,
                experience = 150,
                itemIds = new List<string> { "furniture_magical_bed" },
                itemQuantities = new List<int> { 1 }
            }
        };
        
        collectionQuests.Add(decoratorQuest);
        
        // Rare Treasures
        Quest treasureQuest = new Quest
        {
            questId = "collect_rare_items",
            title = "Rare Treasures",
            description = "Collect rare and valuable items.",
            questType = QuestType.Collection,
            status = QuestStatus.NotStarted,
            isHidden = false,
            isRepeatable = false,
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    objectiveId = "collect_rare_items",
                    description = "Collect rare items (Rare rarity or higher)",
                    requiredAmount = 3
                }
            },
            reward = new QuestReward
            {
                currency = 500,
                experience = 250,
                itemIds = new List<string> { "special_ancient_artifact" },
                itemQuantities = new List<int> { 1 }
            }
        };
        
        collectionQuests.Add(treasureQuest);
        
        // Pet Enthusiast
        Quest petCollectionQuest = new Quest
        {
            questId = "collect_pets",
            title = "Pet Enthusiast",
            description = "Collect and care for multiple pets.",
            questType = QuestType.Collection,
            status = QuestStatus.NotStarted,
            isHidden = false,
            isRepeatable = false,
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    objectiveId = "unlock_pets",
                    description = "Unlock different pets",
                    requiredAmount = 3
                }
            },
            reward = new QuestReward
            {
                currency = 1000,
                experience = 500,
                unlockedPetId = "special_pet" // A special pet as reward
            }
        };
        
        collectionQuests.Add(petCollectionQuest);
    }
    
    private void CreateSocialQuests()
    {
        // Make Friends
        Quest friendsQuest = new Quest
        {
            questId = "social_friends",
            title = "Make Friends",
            description = "Help your pet make friends with other pets.",
            questType = QuestType.Social,
            status = QuestStatus.NotStarted,
            isHidden = false,
            isRepeatable = false,
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    objectiveId = "interact_with_pets",
                    description = "Interact with other pets",
                    requiredAmount = 5
                }
            },
            reward = new QuestReward
            {
                currency = 150,
                experience = 75,
                itemIds = new List<string> { "food_mood_enhancer" },
                itemQuantities = new List<int> { 2 }
            }
        };
        
        socialQuests.Add(friendsQuest);
        
        // Party Animal
        Quest partyQuest = new Quest
        {
            questId = "social_party",
            title = "Party Animal",
            description = "Attend pet social events.",
            questType = QuestType.Social,
            status = QuestStatus.NotStarted,
            isHidden = false,
            isRepeatable = false,
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    objectiveId = "attend_events",
                    description = "Attend pet social events",
                    requiredAmount = 3
                }
            },
            reward = new QuestReward
            {
                currency = 200,
                experience = 100,
                itemIds = new List<string> { "clothing_fancy_outfit" },
                itemQuantities = new List<int> { 1 }
            }
        };
        
        socialQuests.Add(partyQuest);
        
        // Pet Playdate
        Quest playdateQuest = new Quest
        {
            questId = "social_playdate",
            title = "Pet Playdate",
            description = "Arrange playdates for your pet.",
            questType = QuestType.Social,
            status = QuestStatus.NotStarted,
            isHidden = false,
            isRepeatable = false,
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    objectiveId = "arrange_playdates",
                    description = "Arrange playdates with other pets",
                    requiredAmount = 3
                }
            },
            reward = new QuestReward
            {
                currency = 175,
                experience = 85,
                itemIds = new List<string> { "toy_automatic" },
                itemQuantities = new List<int> { 1 }
            }
        };
        
        socialQuests.Add(playdateQuest);
    }
    
    private void CreateSpecialQuests()
    {
        // Seasonal Event: Winter Wonderland
        Quest winterQuest = new Quest
        {
            questId = "special_winter",
            title = "Winter Wonderland",
            description = "Participate in the winter festival.",
            questType = QuestType.Special,
            status = QuestStatus.NotStarted,
            isHidden = true, // Hidden until winter season
            isRepeatable = true, // Repeatable each winter
            isTimeLimited = true,
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    objectiveId = "build_snowman",
                    description = "Build snowmen with your pet",
                    requiredAmount = 3
                },
                new QuestObjective
                {
                    objectiveId = "collect_winter_items",
                    description = "Collect winter-themed items",
                    requiredAmount = 5
                }
            },
            reward = new QuestReward
            {
                currency = 300,
                experience = 150,
                itemIds = new List<string> { "clothing_winter_sweater", "decor_winter_set" },
                itemQuantities = new List<int> { 1, 1 }
            }
        };
        
        specialQuests.Add(winterQuest);
        
        // Seasonal Event: Halloween Spooktacular
        Quest halloweenQuest = new Quest
        {
            questId = "special_halloween",
            title = "Halloween Spooktacular",
            description = "Participate in the Halloween festivities.",
            questType = QuestType.Special,
            status = QuestStatus.NotStarted,
            isHidden = true, // Hidden until Halloween season
            isRepeatable = true, // Repeatable each Halloween
            isTimeLimited = true,
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    objectiveId = "trick_or_treat",
                    description = "Go trick-or-treating with your pet",
                    requiredAmount = 5
                },
                new QuestObjective
                {
                    objectiveId = "collect_halloween_items",
                    description = "Collect Halloween-themed items",
                    requiredAmount = 5
                }
            },
            reward = new QuestReward
            {
                currency = 300,
                experience = 150,
                itemIds = new List<string> { "clothing_costume", "decor_halloween_set" },
                itemQuantities = new List<int> { 1, 1 }
            }
        };
        
        specialQuests.Add(halloweenQuest);
        
        // Legendary Pet
        Quest legendaryPetQuest = new Quest
        {
            questId = "special_legendary_pet",
            title = "Legendary Pet",
            description = "Discover and hatch a legendary pet egg.",
            questType = QuestType.Special,
            status = QuestStatus.NotStarted,
            isHidden = true, // Hidden until player reaches high level
            isRepeatable = false,
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    objectiveId = "find_egg_pieces",
                    description = "Find pieces of the legendary egg",
                    requiredAmount = 5
                },
                new QuestObjective
                {
                    objectiveId = "hatch_egg",
                    description = "Hatch the legendary pet egg",
                    requiredAmount = 1
                }
            },
            reward = new QuestReward
            {
                currency = 1000,
                experience = 500,
                unlockedPetId = "legendary_pet" // A legendary pet as reward
            }
        };
        
        specialQuests.Add(legendaryPetQuest);
    }
    
    private void CreateDailyQuests()
    {
        // Daily Care
        Quest dailyCareQuest = new Quest
        {
            questId = "daily_care",
            title = "Daily Care",
            description = "Take care of your pet's daily needs.",
            questType = QuestType.PetCare,
            status = QuestStatus.NotStarted,
            isHidden = false,
            isRepeatable = true,
            isTimeLimited = true,
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    objectiveId = "feed_pet",
                    description = "Feed your pet",
                    requiredAmount = 2
                },
                new QuestObjective
                {
                    objectiveId = "play_with_pet",
                    description = "Play with your pet",
                    requiredAmount = 2
                },
                new QuestObjective
                {
                    objectiveId = "clean_pet",
                    description = "Clean your pet",
                    requiredAmount = 1
                }
            },
            reward = new QuestReward
            {
                currency = 50,
                experience = 25,
                itemIds = new List<string> { "food_basic_treat" },
                itemQuantities = new List<int> { 2 }
            }
        };
        
        dailyQuests.Add(dailyCareQuest);
        
        // Daily Exercise
        Quest dailyExerciseQuest = new Quest
        {
            questId = "daily_exercise",
            title = "Daily Exercise",
            description = "Make sure your pet gets enough exercise.",
            questType = QuestType.PetCare,
            status = QuestStatus.NotStarted,
            isHidden = false,
            isRepeatable = true,
            isTimeLimited = true,
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    objectiveId = "walk_pet",
                    description = "Take your pet for a walk",
                    requiredAmount = 1
                },
                new QuestObjective
                {
                    objectiveId = "play_active_games",
                    description = "Play active games with your pet",
                    requiredAmount = 2
                }
            },
            reward = new QuestReward
            {
                currency = 40,
                experience = 20,
                itemIds = new List<string> { "food_energy_boost" },
                itemQuantities = new List<int> { 1 }
            }
        };
        
        dailyQuests.Add(dailyExerciseQuest);
        
        // Daily Shopping
        Quest dailyShoppingQuest = new Quest
        {
            questId = "daily_shopping",
            title = "Daily Shopping",
            description = "Buy supplies for your pet.",
            questType = QuestType.Collection,
            status = QuestStatus.NotStarted,
            isHidden = false,
            isRepeatable = true,
            isTimeLimited = true,
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    objectiveId = "buy_items",
                    description = "Buy items from the shop",
                    requiredAmount = 3
                }
            },
            reward = new QuestReward
            {
                currency = 30,
                experience = 15,
                itemIds = new List<string> { "special_pet_whistle" },
                itemQuantities = new List<int> { 1 }
            }
        };
        
        dailyQuests.Add(dailyShoppingQuest);
    }
    
    private void CreateWeeklyQuests()
    {
        // Weekly Pet Show
        Quest weeklyShowQuest = new Quest
        {
            questId = "weekly_pet_show",
            title = "Weekly Pet Show",
            description = "Participate in the weekly pet show.",
            questType = QuestType.Social,
            status = QuestStatus.NotStarted,
            isHidden = false,
            isRepeatable = true,
            isTimeLimited = true,
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    objectiveId = "groom_pet",
                    description = "Groom your pet for the show",
                    requiredAmount = 1
                },
                new QuestObjective
                {
                    objectiveId = "participate_in_show",
                    description = "Participate in pet show events",
                    requiredAmount = 3
                }
            },
            reward = new QuestReward
            {
                currency = 200,
                experience = 100,
                itemIds = new List<string> { "clothing_fancy_outfit" },
                itemQuantities = new List<int> { 1 }
            }
        };
        
        weeklyQuests.Add(weeklyShowQuest);
        
        // Weekly Exploration
        Quest weeklyExplorationQuest = new Quest
        {
            questId = "weekly_exploration",
            title = "Weekly Exploration",
            description = "Explore different locations with your pet.",
            questType = QuestType.Exploration,
            status = QuestStatus.NotStarted,
            isHidden = false,
            isRepeatable = true,
            isTimeLimited = true,
            objectives = new List<QuestObjective>
            {
                new QuestObjective
                {
                    objectiveId = "visit_locations",
                    description = "Visit different locations",
                    requiredAmount = 4
                },
                new QuestObjective
                {
                    objectiveId = "find_treasures",
                    description = "Find hidden treasures",
                    requiredAmount = 2
                }
            },
            reward = new QuestReward
            {
                currency = 250,
                experience = 125,
                itemIds = new List<string> { "special_rare_gem" },
                itemQuantities = new List<int> { 1 }
            }
        };
        
        weeklyQuests.Add(weeklyExplorationQuest);
    }
    
    private void CreateAchievements()
    {
        // General Achievements
        
        // Pet Parent
        generalAchievements.Add(new Achievement
        {
            achievementId = "achievement_pet_parent",
            title = "Pet Parent",
            description = "Adopt your first pet",
            progressRequired = 1,
            rewardCurrency = 100
        });
        
        // Dedicated Owner
        generalAchievements.Add(new Achievement
        {
            achievementId = "achievement_dedicated_owner",
            title = "Dedicated Owner",
            description = "Play the game for 7 days",
            progressRequired = 7,
            rewardCurrency = 200
        });
        
        // Wealthy Pet Owner
        generalAchievements.Add(new Achievement
        {
            achievementId = "achievement_wealthy",
            title = "Wealthy Pet Owner",
            description = "Accumulate 5000 currency",
            progressRequired = 5000,
            rewardCurrency = 500
        });
        
        // Pet Achievements
        
        // Multi-Pet Household
        petAchievements.Add(new Achievement
        {
            achievementId = "achievement_multi_pet",
            title = "Multi-Pet Household",
            description = "Adopt 5 different pets",
            progressRequired = 5,
            rewardCurrency = 300
        });
        
        // Master Trainer
        petAchievements.Add(new Achievement
        {
            achievementId = "achievement_master_trainer",
            title = "Master Trainer",
            description = "Reach level 20 with any pet",
            progressRequired = 20,
            rewardCurrency = 500
        });
        
        // Perfect Care
        petAchievements.Add(new Achievement
        {
            achievementId = "achievement_perfect_care",
            title = "Perfect Care",
            description = "Keep all pet stats at maximum for 24 hours",
            progressRequired = 1,
            rewardCurrency = 400
        });
        
        // Exploration Achievements
        
        // World Traveler
        explorationAchievements.Add(new Achievement
        {
            achievementId = "achievement_world_traveler",
            title = "World Traveler",
            description = "Visit all locations in Animora",
            progressRequired = 8, // Number of locations
            rewardCurrency = 500
        });
        
        // Weather Watcher
        explorationAchievements.Add(new Achievement
        {
            achievementId = "achievement_weather_watcher",
            title = "Weather Watcher",
            description = "Experience all types of weather",
            progressRequired = 3, // Number of weather types
            rewardCurrency = 200
        });
        
        // Collection Achievements
        
        // Toy Enthusiast
        collectionAchievements.Add(new Achievement
        {
            achievementId = "achievement_toy_enthusiast",
            title = "Toy Enthusiast",
            description = "Collect 10 different toys",
            progressRequired = 10,
            rewardCurrency = 300
        });
        
        // Fashion Designer
        collectionAchievements.Add(new Achievement
        {
            achievementId = "achievement_fashion_designer",
            title = "Fashion Designer",
            description = "Collect 10 different clothing items",
            progressRequired = 10,
            rewardCurrency = 300
        });
        
        // Interior Decorator
        collectionAchievements.Add(new Achievement
        {
            achievementId = "achievement_interior_decorator",
            title = "Interior Decorator",
            description = "Collect 10 different furniture items",
            progressRequired = 10,
            rewardCurrency = 300
        });
        
        // Social Achievements
        
        // Social Butterfly
        socialAchievements.Add(new Achievement
        {
            achievementId = "achievement_social_butterfly",
            title = "Social Butterfly",
            description = "Interact with 20 different pets",
            progressRequired = 20,
            rewardCurrency = 300
        });
        
        // Event Enthusiast
        socialAchievements.Add(new Achievement
        {
            achievementId = "achievement_event_enthusiast",
            title = "Event Enthusiast",
            description = "Participate in 10 different events",
            progressRequired = 10,
            rewardCurrency = 400
        });
    }
}