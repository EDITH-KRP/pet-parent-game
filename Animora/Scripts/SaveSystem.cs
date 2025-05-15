using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class PetSaveData
{
    public string petName;
    public string petType;
    public int petLevel;
    public float petExperience;
    public float hunger;
    public float mood;
    public float energy;
    public float cleanliness;
    public float health;
}

[System.Serializable]
public class InventorySaveData
{
    public List<string> itemIds = new List<string>();
    public List<int> itemQuantities = new List<int>();
}

[System.Serializable]
public class QuestSaveData
{
    public List<string> activeQuestIds = new List<string>();
    public List<string> completedQuestIds = new List<string>();
    public Dictionary<string, List<int>> questObjectiveProgress = new Dictionary<string, List<int>>();
}

[System.Serializable]
public class AchievementSaveData
{
    public List<string> unlockedAchievementIds = new List<string>();
    public Dictionary<string, int> achievementProgress = new Dictionary<string, int>();
}

[System.Serializable]
public class WorldSaveData
{
    public string currentLocation;
    public List<string> unlockedLocations = new List<string>();
    public List<string> placedFurnitureIds = new List<string>();
    public List<Vector3> furniturePositions = new List<Vector3>();
    public List<Quaternion> furnitureRotations = new List<Quaternion>();
}

[System.Serializable]
public class GameSaveData
{
    public string playerName;
    public int currency;
    public int playerLevel;
    public float playerExperience;
    public List<string> unlockedPets = new List<string>();
    public string activePetName;
    public PetSaveData activePetData;
    public InventorySaveData inventoryData = new InventorySaveData();
    public QuestSaveData questData = new QuestSaveData();
    public AchievementSaveData achievementData = new AchievementSaveData();
    public WorldSaveData worldData = new WorldSaveData();
    public System.DateTime saveTime;
}

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance;
    
    [Header("Save Settings")]
    public string saveFileName = "animora_save.json";
    public bool autoSave = true;
    public float autoSaveInterval = 300f; // 5 minutes
    
    private float autoSaveTimer = 0f;
    private string savePath;
    
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
        
        // Set save path
        savePath = Path.Combine(Application.persistentDataPath, saveFileName);
    }
    
    private void Start()
    {
        // Load game data if it exists
        if (File.Exists(savePath))
        {
            LoadGame();
        }
    }
    
    private void Update()
    {
        // Auto-save
        if (autoSave)
        {
            autoSaveTimer += Time.deltaTime;
            
            if (autoSaveTimer >= autoSaveInterval)
            {
                autoSaveTimer = 0f;
                SaveGame();
            }
        }
    }
    
    // Save game data
    public void SaveGame()
    {
        GameSaveData saveData = new GameSaveData();
        
        // Save player data
        saveData.playerName = GameManager.Instance.playerData.playerName;
        saveData.currency = GameManager.Instance.playerData.currency;
        saveData.playerLevel = GameManager.Instance.playerData.level;
        saveData.playerExperience = GameManager.Instance.playerData.experience;
        saveData.unlockedPets = new List<string>(GameManager.Instance.playerData.unlockedPets);
        
        // Save active pet data
        if (GameManager.Instance.currentPet != null)
        {
            Pet activePet = GameManager.Instance.currentPet.GetComponent<Pet>();
            if (activePet != null)
            {
                saveData.activePetName = activePet.petName;
                
                PetSaveData petData = new PetSaveData
                {
                    petName = activePet.petName,
                    petType = activePet.petType.ToString(),
                    petLevel = activePet.petLevel,
                    petExperience = activePet.petExperience,
                    hunger = activePet.stats.hunger,
                    mood = activePet.stats.mood,
                    energy = activePet.stats.energy,
                    cleanliness = activePet.stats.cleanliness,
                    health = activePet.stats.health
                };
                
                saveData.activePetData = petData;
            }
        }
        
        // Save inventory data
        if (ItemSystem.Instance != null)
        {
            foreach (InventoryItem item in ItemSystem.Instance.inventory)
            {
                saveData.inventoryData.itemIds.Add(item.item.itemId);
                saveData.inventoryData.itemQuantities.Add(item.quantity);
            }
        }
        
        // Save quest data
        if (QuestSystem.Instance != null)
        {
            // Save active quests
            foreach (Quest quest in QuestSystem.Instance.activeQuests)
            {
                saveData.questData.activeQuestIds.Add(quest.questId);
                
                // Save objective progress
                List<int> objectiveProgress = new List<int>();
                foreach (QuestObjective objective in quest.objectives)
                {
                    objectiveProgress.Add(objective.currentAmount);
                }
                
                saveData.questData.questObjectiveProgress[quest.questId] = objectiveProgress;
            }
            
            // Save completed quests
            foreach (Quest quest in QuestSystem.Instance.GetCompletedQuests())
            {
                saveData.questData.completedQuestIds.Add(quest.questId);
            }
        }
        
        // Save achievement data
        if (QuestSystem.Instance != null)
        {
            foreach (Achievement achievement in QuestSystem.Instance.achievements)
            {
                if (achievement.isUnlocked)
                {
                    saveData.achievementData.unlockedAchievementIds.Add(achievement.achievementId);
                }
                
                saveData.achievementData.achievementProgress[achievement.achievementId] = achievement.currentProgress;
            }
        }
        
        // Save world data
        saveData.worldData.currentLocation = GameManager.Instance.currentLocation;
        saveData.worldData.unlockedLocations = new List<string>(GameManager.Instance.playerData.unlockedLocations);
        
        // Save furniture placement
        if (ItemSystem.Instance != null)
        {
            foreach (Item furniture in ItemSystem.Instance.placedFurniture)
            {
                // This is simplified - in a real game you'd need to store actual positions
                saveData.worldData.placedFurnitureIds.Add(furniture.itemId);
                saveData.worldData.furniturePositions.Add(Vector3.zero);
                saveData.worldData.furnitureRotations.Add(Quaternion.identity);
            }
        }
        
        // Save timestamp
        saveData.saveTime = System.DateTime.Now;
        
        // Convert to JSON
        string jsonData = JsonUtility.ToJson(saveData, true);
        
        // Write to file
        File.WriteAllText(savePath, jsonData);
        
        Debug.Log("Game saved successfully!");
    }
    
    // Load game data
    public void LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("Save file not found!");
            return;
        }
        
        // Read JSON data
        string jsonData = File.ReadAllText(savePath);
        
        // Parse JSON
        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(jsonData);
        
        if (saveData == null)
        {
            Debug.LogError("Failed to parse save data!");
            return;
        }
        
        // Load player data
        GameManager.Instance.playerData.playerName = saveData.playerName;
        GameManager.Instance.playerData.currency = saveData.currency;
        GameManager.Instance.playerData.level = saveData.playerLevel;
        GameManager.Instance.playerData.experience = saveData.playerExperience;
        GameManager.Instance.playerData.unlockedPets = new List<string>(saveData.unlockedPets);
        
        // Load inventory data
        if (ItemSystem.Instance != null && saveData.inventoryData != null)
        {
            // Clear current inventory
            ItemSystem.Instance.inventory.Clear();
            
            // Load saved items
            for (int i = 0; i < saveData.inventoryData.itemIds.Count && i < saveData.inventoryData.itemQuantities.Count; i++)
            {
                string itemId = saveData.inventoryData.itemIds[i];
                int quantity = saveData.inventoryData.itemQuantities[i];
                
                Item item = ItemSystem.Instance.GetItemById(itemId);
                if (item != null)
                {
                    ItemSystem.Instance.AddItem(item, quantity);
                }
            }
        }
        
        // Load quest data
        if (QuestSystem.Instance != null && saveData.questData != null)
        {
            // Clear current active quests
            QuestSystem.Instance.activeQuests.Clear();
            
            // Load active quests
            foreach (string questId in saveData.questData.activeQuestIds)
            {
                QuestSystem.Instance.StartQuest(questId);
                
                // Load objective progress
                if (saveData.questData.questObjectiveProgress.ContainsKey(questId))
                {
                    List<int> objectiveProgress = saveData.questData.questObjectiveProgress[questId];
                    Quest quest = QuestSystem.Instance.allQuests.Find(q => q.questId == questId);
                    
                    if (quest != null)
                    {
                        for (int i = 0; i < quest.objectives.Count && i < objectiveProgress.Count; i++)
                        {
                            quest.objectives[i].currentAmount = objectiveProgress[i];
                            quest.objectives[i].isCompleted = quest.objectives[i].currentAmount >= quest.objectives[i].requiredAmount;
                        }
                    }
                }
            }
            
            // Load completed quests
            foreach (string questId in saveData.questData.completedQuestIds)
            {
                Quest quest = QuestSystem.Instance.allQuests.Find(q => q.questId == questId);
                if (quest != null)
                {
                    quest.status = QuestStatus.Completed;
                }
            }
        }
        
        // Load achievement data
        if (QuestSystem.Instance != null && saveData.achievementData != null)
        {
            foreach (Achievement achievement in QuestSystem.Instance.achievements)
            {
                // Set unlocked status
                achievement.isUnlocked = saveData.achievementData.unlockedAchievementIds.Contains(achievement.achievementId);
                
                // Set progress
                if (saveData.achievementData.achievementProgress.ContainsKey(achievement.achievementId))
                {
                    achievement.currentProgress = saveData.achievementData.achievementProgress[achievement.achievementId];
                }
            }
        }
        
        // Load world data
        if (saveData.worldData != null)
        {
            // Set current location
            GameManager.Instance.currentLocation = saveData.worldData.currentLocation;
            
            // Set unlocked locations
            GameManager.Instance.playerData.unlockedLocations = new List<string>(saveData.worldData.unlockedLocations);
            foreach (string locationName in saveData.worldData.unlockedLocations)
            {
                foreach (GameLocation location in GameManager.Instance.gameLocations)
                {
                    if (location.locationName == locationName)
                    {
                        location.isUnlocked = true;
                    }
                }
            }
            
            // Load furniture placement
            // This would be implemented with actual furniture placement code
        }
        
        // Spawn active pet if any
        if (!string.IsNullOrEmpty(saveData.activePetName))
        {
            GameManager.Instance.SpawnPetByName(saveData.activePetName);
            
            // Apply pet stats
            if (saveData.activePetData != null && GameManager.Instance.currentPet != null)
            {
                Pet pet = GameManager.Instance.currentPet.GetComponent<Pet>();
                if (pet != null)
                {
                    pet.petLevel = saveData.activePetData.petLevel;
                    pet.petExperience = saveData.activePetData.petExperience;
                    pet.stats.hunger = saveData.activePetData.hunger;
                    pet.stats.mood = saveData.activePetData.mood;
                    pet.stats.energy = saveData.activePetData.energy;
                    pet.stats.cleanliness = saveData.activePetData.cleanliness;
                    pet.stats.health = saveData.activePetData.health;
                }
            }
        }
        
        Debug.Log("Game loaded successfully!");
    }
    
    // Delete save data
    public void DeleteSaveData()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save data deleted!");
        }
    }
    
    // Check if save data exists
    public bool SaveDataExists()
    {
        return File.Exists(savePath);
    }
    
    // Get save data info without loading the game
    public string GetSaveInfo()
    {
        if (!File.Exists(savePath))
        {
            return "No save data found.";
        }
        
        try
        {
            // Read JSON data
            string jsonData = File.ReadAllText(savePath);
            
            // Parse JSON
            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(jsonData);
            
            if (saveData == null)
            {
                return "Invalid save data.";
            }
            
            // Format save info
            string saveInfo = $"Player: {saveData.playerName}\n";
            saveInfo += $"Level: {saveData.playerLevel}\n";
            saveInfo += $"Currency: {saveData.currency}\n";
            saveInfo += $"Pets: {saveData.unlockedPets.Count}\n";
            saveInfo += $"Active Pet: {saveData.activePetName}\n";
            saveInfo += $"Last Saved: {saveData.saveTime}";
            
            return saveInfo;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error reading save data: {e.Message}");
            return "Error reading save data.";
        }
    }
}