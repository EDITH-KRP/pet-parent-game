using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LocationRequirement
{
    public string requirementName;
    public int playerLevelRequired;
    public int currencyRequired;
    public List<string> prerequisiteLocations = new List<string>();
    public List<string> prerequisiteQuestIds = new List<string>();
}

[System.Serializable]
public class LocationWeatherEffect
{
    public Weather weatherType;
    public float moodEffect;
    public float energyEffect;
    public float healthEffect;
}

[System.Serializable]
public class LocationTimeEffect
{
    public TimeOfDay timeOfDay;
    public float moodEffect;
    public float energyEffect;
}

[System.Serializable]
public class LocationInteractable
{
    public string interactableName;
    public GameObject prefab;
    public Vector3 position;
    public Quaternion rotation;
    public bool isUnlocked = true;
    public int unlockCost;
}

public class GameLocations : MonoBehaviour
{
    [System.Serializable]
    public class DetailedGameLocation
    {
        [Header("Basic Info")]
        public string locationId;
        public string locationName;
        public string description;
        public Sprite locationIcon;
        public bool isUnlocked = false;
        public bool isIndoor = false;
        
        [Header("Requirements")]
        public LocationRequirement unlockRequirement;
        
        [Header("Environment")]
        public GameObject environmentPrefab;
        public AudioClip backgroundMusic;
        public AudioClip[] ambientSounds;
        
        [Header("Effects")]
        public LocationWeatherEffect[] weatherEffects;
        public LocationTimeEffect[] timeEffects;
        
        [Header("Interactables")]
        public List<LocationInteractable> interactables = new List<LocationInteractable>();
        
        [Header("NPCs")]
        public List<GameObject> locationNPCs = new List<GameObject>();
        public int maxActiveNPCs = 3;
        
        [Header("Special Features")]
        public bool hasShop = false;
        public bool hasMinigames = false;
        public string[] availableMinigames;
        public bool hasSpecialEvents = false;
    }
    
    public List<DetailedGameLocation> gameLocations = new List<DetailedGameLocation>();
    
    // Singleton instance
    public static GameLocations Instance;
    
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
    }
    
    private void Start()
    {
        // Initialize locations
        InitializeLocations();
    }
    
    private void InitializeLocations()
    {
        // Add default locations if none exist
        if (gameLocations.Count == 0)
        {
            CreateDefaultLocations();
        }
        
        // Register locations with GameManager
        foreach (DetailedGameLocation location in gameLocations)
        {
            GameLocation simpleLocation = new GameLocation
            {
                locationName = location.locationName,
                locationDescription = location.description,
                locationIcon = location.locationIcon,
                isUnlocked = location.isUnlocked
            };
            
            GameManager.Instance.gameLocations.Add(simpleLocation);
        }
    }
    
    private void CreateDefaultLocations()
    {
        // Pet Home
        DetailedGameLocation petHome = new DetailedGameLocation
        {
            locationId = "pet_home",
            locationName = "Pet Home",
            description = "A cozy home for you and your pets.",
            isUnlocked = true,
            isIndoor = true,
            hasShop = false,
            maxActiveNPCs = 0
        };
        
        // Pet Park
        DetailedGameLocation petPark = new DetailedGameLocation
        {
            locationId = "pet_park",
            locationName = "Pet Park",
            description = "A spacious park where pets can play and socialize.",
            isUnlocked = true,
            isIndoor = false,
            hasShop = false,
            maxActiveNPCs = 5,
            weatherEffects = new LocationWeatherEffect[]
            {
                new LocationWeatherEffect { weatherType = Weather.Sunny, moodEffect = 5f, energyEffect = -2f, healthEffect = 1f },
                new LocationWeatherEffect { weatherType = Weather.Rainy, moodEffect = -3f, energyEffect = -5f, healthEffect = -1f },
                new LocationWeatherEffect { weatherType = Weather.Snowy, moodEffect = 2f, energyEffect = -8f, healthEffect = -2f }
            }
        };
        
        // Pet Café
        DetailedGameLocation petCafe = new DetailedGameLocation
        {
            locationId = "pet_cafe",
            locationName = "Pet Café",
            description = "A friendly café where pets and owners can relax and enjoy treats.",
            isUnlocked = true,
            isIndoor = true,
            hasShop = true,
            maxActiveNPCs = 4
        };
        
        // Pet Spa & Clinic
        DetailedGameLocation petSpa = new DetailedGameLocation
        {
            locationId = "pet_spa",
            locationName = "Pet Spa & Clinic",
            description = "A facility for pet grooming and healthcare.",
            isUnlocked = true,
            isIndoor = true,
            hasShop = true,
            maxActiveNPCs = 2
        };
        
        // Enchanted Forest
        DetailedGameLocation enchantedForest = new DetailedGameLocation
        {
            locationId = "enchanted_forest",
            locationName = "Enchanted Forest",
            description = "A magical forest filled with wonders and mysteries.",
            isUnlocked = false,
            isIndoor = false,
            hasShop = false,
            hasSpecialEvents = true,
            maxActiveNPCs = 3,
            unlockRequirement = new LocationRequirement
            {
                requirementName = "Forest Explorer",
                playerLevelRequired = 5,
                currencyRequired = 500,
                prerequisiteQuestIds = new List<string> { "quest_forest_key" }
            }
        };
        
        // Crystal Caves
        DetailedGameLocation crystalCaves = new DetailedGameLocation
        {
            locationId = "crystal_caves",
            locationName = "Crystal Caves",
            description = "Mysterious caves filled with glowing crystals and hidden treasures.",
            isUnlocked = false,
            isIndoor = true,
            hasShop = false,
            hasMinigames = true,
            maxActiveNPCs = 2,
            unlockRequirement = new LocationRequirement
            {
                requirementName = "Cave Explorer",
                playerLevelRequired = 8,
                currencyRequired = 1000,
                prerequisiteLocations = new List<string> { "enchanted_forest" }
            }
        };
        
        // Sky Islands
        DetailedGameLocation skyIslands = new DetailedGameLocation
        {
            locationId = "sky_islands",
            locationName = "Sky Islands",
            description = "Floating islands high above the clouds, home to rare flying creatures.",
            isUnlocked = false,
            isIndoor = false,
            hasShop = true,
            hasSpecialEvents = true,
            maxActiveNPCs = 4,
            unlockRequirement = new LocationRequirement
            {
                requirementName = "Sky Explorer",
                playerLevelRequired = 12,
                currencyRequired = 2000,
                prerequisiteQuestIds = new List<string> { "quest_sky_key" }
            }
        };
        
        // Underwater Realm
        DetailedGameLocation underwaterRealm = new DetailedGameLocation
        {
            locationId = "underwater_realm",
            locationName = "Underwater Realm",
            description = "A magical underwater world where pets can breathe and explore.",
            isUnlocked = false,
            isIndoor = false,
            hasShop = true,
            hasMinigames = true,
            maxActiveNPCs = 5,
            unlockRequirement = new LocationRequirement
            {
                requirementName = "Ocean Explorer",
                playerLevelRequired = 15,
                currencyRequired = 3000,
                prerequisiteQuestIds = new List<string> { "quest_ocean_key" }
            }
        };
        
        // Add all locations to the list
        gameLocations.Add(petHome);
        gameLocations.Add(petPark);
        gameLocations.Add(petCafe);
        gameLocations.Add(petSpa);
        gameLocations.Add(enchantedForest);
        gameLocations.Add(crystalCaves);
        gameLocations.Add(skyIslands);
        gameLocations.Add(underwaterRealm);
    }
    
    // Get location by ID
    public DetailedGameLocation GetLocationById(string locationId)
    {
        return gameLocations.Find(loc => loc.locationId == locationId);
    }
    
    // Get location by name
    public DetailedGameLocation GetLocationByName(string locationName)
    {
        return gameLocations.Find(loc => loc.locationName == locationName);
    }
    
    // Check if a location can be unlocked
    public bool CanUnlockLocation(string locationId)
    {
        DetailedGameLocation location = GetLocationById(locationId);
        
        if (location == null || location.isUnlocked)
            return false;
            
        LocationRequirement req = location.unlockRequirement;
        
        if (req == null)
            return true;
            
        // Check player level
        if (GameManager.Instance.playerData.level < req.playerLevelRequired)
            return false;
            
        // Check currency
        if (GameManager.Instance.playerData.currency < req.currencyRequired)
            return false;
            
        // Check prerequisite locations
        foreach (string prereqLocation in req.prerequisiteLocations)
        {
            DetailedGameLocation prereq = GetLocationById(prereqLocation);
            if (prereq == null || !prereq.isUnlocked)
                return false;
        }
        
        // Check prerequisite quests
        foreach (string questId in req.prerequisiteQuestIds)
        {
            if (QuestSystem.Instance == null)
                continue;
                
            Quest quest = QuestSystem.Instance.allQuests.Find(q => q.questId == questId);
            if (quest == null || quest.status != QuestStatus.Completed)
                return false;
        }
        
        return true;
    }
    
    // Unlock a location
    public bool UnlockLocation(string locationId)
    {
        DetailedGameLocation location = GetLocationById(locationId);
        
        if (location == null || location.isUnlocked)
            return false;
            
        if (!CanUnlockLocation(locationId))
            return false;
            
        // Deduct currency if required
        if (location.unlockRequirement != null && location.unlockRequirement.currencyRequired > 0)
        {
            GameManager.Instance.SpendCurrency(location.unlockRequirement.currencyRequired);
        }
        
        // Unlock the location
        location.isUnlocked = true;
        
        // Also unlock in GameManager
        foreach (GameLocation gameLoc in GameManager.Instance.gameLocations)
        {
            if (gameLoc.locationName == location.locationName)
            {
                gameLoc.isUnlocked = true;
                break;
            }
        }
        
        // Add to player's unlocked locations
        if (!GameManager.Instance.playerData.unlockedLocations.Contains(location.locationName))
        {
            GameManager.Instance.playerData.unlockedLocations.Add(location.locationName);
        }
        
        return true;
    }
    
    // Get weather effects for a location
    public LocationWeatherEffect GetWeatherEffect(string locationId, Weather weather)
    {
        DetailedGameLocation location = GetLocationById(locationId);
        
        if (location == null || location.weatherEffects == null)
            return null;
            
        foreach (LocationWeatherEffect effect in location.weatherEffects)
        {
            if (effect.weatherType == weather)
                return effect;
        }
        
        return null;
    }
    
    // Get time effects for a location
    public LocationTimeEffect GetTimeEffect(string locationId, TimeOfDay timeOfDay)
    {
        DetailedGameLocation location = GetLocationById(locationId);
        
        if (location == null || location.timeEffects == null)
            return null;
            
        foreach (LocationTimeEffect effect in location.timeEffects)
        {
            if (effect.timeOfDay == timeOfDay)
                return effect;
        }
        
        return null;
    }
    
    // Spawn location environment
    public GameObject SpawnLocationEnvironment(string locationId, Transform parent = null)
    {
        DetailedGameLocation location = GetLocationById(locationId);
        
        if (location == null || location.environmentPrefab == null)
            return null;
            
        GameObject environment = Instantiate(location.environmentPrefab, Vector3.zero, Quaternion.identity);
        
        if (parent != null)
        {
            environment.transform.parent = parent;
        }
        
        return environment;
    }
    
    // Spawn location interactables
    public List<GameObject> SpawnLocationInteractables(string locationId, Transform parent = null)
    {
        DetailedGameLocation location = GetLocationById(locationId);
        
        if (location == null || location.interactables == null)
            return null;
            
        List<GameObject> spawnedObjects = new List<GameObject>();
        
        foreach (LocationInteractable interactable in location.interactables)
        {
            if (interactable.isUnlocked && interactable.prefab != null)
            {
                GameObject obj = Instantiate(interactable.prefab, interactable.position, interactable.rotation);
                
                if (parent != null)
                {
                    obj.transform.parent = parent;
                }
                
                spawnedObjects.Add(obj);
            }
        }
        
        return spawnedObjects;
    }
    
    // Spawn location NPCs
    public List<GameObject> SpawnLocationNPCs(string locationId, Transform parent = null)
    {
        DetailedGameLocation location = GetLocationById(locationId);
        
        if (location == null || location.locationNPCs == null || location.locationNPCs.Count == 0)
            return null;
            
        List<GameObject> spawnedNPCs = new List<GameObject>();
        
        // Determine how many NPCs to spawn
        int npcCount = Random.Range(1, location.maxActiveNPCs + 1);
        
        for (int i = 0; i < npcCount; i++)
        {
            // Pick a random NPC prefab
            int randomIndex = Random.Range(0, location.locationNPCs.Count);
            GameObject npcPrefab = location.locationNPCs[randomIndex];
            
            if (npcPrefab != null)
            {
                // Get a random position within the location
                Vector3 spawnPos = GetRandomPositionInLocation(locationId);
                
                GameObject npc = Instantiate(npcPrefab, spawnPos, Quaternion.identity);
                
                if (parent != null)
                {
                    npc.transform.parent = parent;
                }
                
                spawnedNPCs.Add(npc);
            }
        }
        
        return spawnedNPCs;
    }
    
    // Get a random position within a location
    private Vector3 GetRandomPositionInLocation(string locationId)
    {
        // This is a simplified implementation
        // In a real game, you would use NavMesh sampling or other techniques
        
        // For now, just return a random position near the origin
        float x = Random.Range(-10f, 10f);
        float z = Random.Range(-10f, 10f);
        
        return new Vector3(x, 0f, z);
    }
}