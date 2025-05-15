using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestType
{
    PetCare,
    Exploration,
    Collection,
    Social,
    Special
}

public enum QuestStatus
{
    NotStarted,
    InProgress,
    Completed,
    Failed
}

[System.Serializable]
public class QuestObjective
{
    public string objectiveId;
    public string description;
    public int requiredAmount = 1;
    public int currentAmount = 0;
    public bool isCompleted = false;
    
    public float GetProgress()
    {
        return Mathf.Clamp01((float)currentAmount / requiredAmount);
    }
    
    public void UpdateProgress(int amount)
    {
        currentAmount = Mathf.Min(currentAmount + amount, requiredAmount);
        isCompleted = currentAmount >= requiredAmount;
    }
}

[System.Serializable]
public class QuestReward
{
    public int currency;
    public float experience;
    public List<string> itemIds = new List<string>();
    public List<int> itemQuantities = new List<int>();
    public string unlockedLocationId;
    public string unlockedPetId;
}

[System.Serializable]
public class Quest
{
    public string questId;
    public string title;
    public string description;
    public QuestType questType;
    public QuestStatus status = QuestStatus.NotStarted;
    public List<QuestObjective> objectives = new List<QuestObjective>();
    public QuestReward reward = new QuestReward();
    public List<string> prerequisiteQuestIds = new List<string>();
    public bool isRepeatable = false;
    public bool isHidden = false;
    public bool isTimeLimited = false;
    public System.DateTime expirationDate;
    
    public bool AreAllObjectivesCompleted()
    {
        foreach (QuestObjective objective in objectives)
        {
            if (!objective.isCompleted)
                return false;
        }
        
        return true;
    }
    
    public float GetOverallProgress()
    {
        if (objectives.Count == 0)
            return 0f;
        
        float totalProgress = 0f;
        
        foreach (QuestObjective objective in objectives)
        {
            totalProgress += objective.GetProgress();
        }
        
        return totalProgress / objectives.Count;
    }
}

[System.Serializable]
public class Achievement
{
    public string achievementId;
    public string title;
    public string description;
    public Sprite icon;
    public int progressRequired = 1;
    public int currentProgress = 0;
    public bool isUnlocked = false;
    public int rewardCurrency;
    
    public float GetProgress()
    {
        return Mathf.Clamp01((float)currentProgress / progressRequired);
    }
    
    public void UpdateProgress(int amount)
    {
        currentProgress = Mathf.Min(currentProgress + amount, progressRequired);
        isUnlocked = currentProgress >= progressRequired;
    }
}

public class QuestSystem : MonoBehaviour
{
    public static QuestSystem Instance;
    
    [Header("Quests")]
    public List<Quest> allQuests = new List<Quest>();
    public List<Quest> activeQuests = new List<Quest>();
    public int maxActiveQuests = 5;
    
    [Header("Achievements")]
    public List<Achievement> achievements = new List<Achievement>();
    
    // Private variables
    private Dictionary<string, Quest> questDictionary = new Dictionary<string, Quest>();
    private Dictionary<string, Achievement> achievementDictionary = new Dictionary<string, Achievement>();
    
    // Events
    public delegate void QuestStatusChangedHandler(Quest quest);
    public event QuestStatusChangedHandler OnQuestStatusChanged;
    
    public delegate void QuestObjectiveUpdatedHandler(Quest quest, QuestObjective objective);
    public event QuestObjectiveUpdatedHandler OnQuestObjectiveUpdated;
    
    public delegate void AchievementUnlockedHandler(Achievement achievement);
    public event AchievementUnlockedHandler OnAchievementUnlocked;
    
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
        
        // Initialize dictionaries
        foreach (Quest quest in allQuests)
        {
            questDictionary[quest.questId] = quest;
        }
        
        foreach (Achievement achievement in achievements)
        {
            achievementDictionary[achievement.achievementId] = achievement;
        }
    }
    
    private void Start()
    {
        // Check for available quests
        CheckForAvailableQuests();
    }
    
    private void Update()
    {
        // Check for expired quests
        CheckForExpiredQuests();
    }
    
    private void CheckForAvailableQuests()
    {
        foreach (Quest quest in allQuests)
        {
            if (quest.status == QuestStatus.NotStarted && !quest.isHidden)
            {
                bool prerequisitesMet = true;
                
                // Check prerequisites
                foreach (string prerequisiteId in quest.prerequisiteQuestIds)
                {
                    if (questDictionary.ContainsKey(prerequisiteId))
                    {
                        Quest prerequisiteQuest = questDictionary[prerequisiteId];
                        if (prerequisiteQuest.status != QuestStatus.Completed)
                        {
                            prerequisitesMet = false;
                            break;
                        }
                    }
                }
                
                // If prerequisites are met and we have room, add to active quests
                if (prerequisitesMet && activeQuests.Count < maxActiveQuests)
                {
                    StartQuest(quest.questId);
                }
            }
        }
    }
    
    private void CheckForExpiredQuests()
    {
        for (int i = activeQuests.Count - 1; i >= 0; i--)
        {
            Quest quest = activeQuests[i];
            
            if (quest.isTimeLimited && System.DateTime.Now > quest.expirationDate)
            {
                // Quest has expired
                quest.status = QuestStatus.Failed;
                activeQuests.RemoveAt(i);
                
                // Trigger event
                OnQuestStatusChanged?.Invoke(quest);
            }
        }
    }
    
    // Start a quest
    public bool StartQuest(string questId)
    {
        if (!questDictionary.ContainsKey(questId))
        {
            Debug.LogWarning($"Quest with ID {questId} not found!");
            return false;
        }
        
        Quest quest = questDictionary[questId];
        
        // Check if quest is already active or completed
        if (quest.status != QuestStatus.NotStarted && !quest.isRepeatable)
        {
            return false;
        }
        
        // Check if we have room for another active quest
        if (activeQuests.Count >= maxActiveQuests)
        {
            return false;
        }
        
        // Reset quest progress if repeatable
        if (quest.isRepeatable)
        {
            foreach (QuestObjective objective in quest.objectives)
            {
                objective.currentAmount = 0;
                objective.isCompleted = false;
            }
        }
        
        // Set quest as active
        quest.status = QuestStatus.InProgress;
        
        // Add to active quests if not already there
        if (!activeQuests.Contains(quest))
        {
            activeQuests.Add(quest);
        }
        
        // Trigger event
        OnQuestStatusChanged?.Invoke(quest);
        
        return true;
    }
    
    // Complete a quest
    public bool CompleteQuest(string questId)
    {
        if (!questDictionary.ContainsKey(questId))
        {
            Debug.LogWarning($"Quest with ID {questId} not found!");
            return false;
        }
        
        Quest quest = questDictionary[questId];
        
        // Check if quest is in progress and all objectives are completed
        if (quest.status != QuestStatus.InProgress || !quest.AreAllObjectivesCompleted())
        {
            return false;
        }
        
        // Set quest as completed
        quest.status = QuestStatus.Completed;
        
        // Remove from active quests
        activeQuests.Remove(quest);
        
        // Award rewards
        AwardQuestRewards(quest);
        
        // Trigger event
        OnQuestStatusChanged?.Invoke(quest);
        
        // Check for newly available quests
        CheckForAvailableQuests();
        
        return true;
    }
    
    // Abandon a quest
    public bool AbandonQuest(string questId)
    {
        if (!questDictionary.ContainsKey(questId))
        {
            Debug.LogWarning($"Quest with ID {questId} not found!");
            return false;
        }
        
        Quest quest = questDictionary[questId];
        
        // Check if quest is in progress
        if (quest.status != QuestStatus.InProgress)
        {
            return false;
        }
        
        // Set quest as not started (or failed if you prefer)
        quest.status = QuestStatus.NotStarted;
        
        // Remove from active quests
        activeQuests.Remove(quest);
        
        // Trigger event
        OnQuestStatusChanged?.Invoke(quest);
        
        return true;
    }
    
    // Update quest objective progress
    public void UpdateQuestObjective(string questId, string objectiveId, int amount = 1)
    {
        if (!questDictionary.ContainsKey(questId))
        {
            Debug.LogWarning($"Quest with ID {questId} not found!");
            return;
        }
        
        Quest quest = questDictionary[questId];
        
        // Check if quest is in progress
        if (quest.status != QuestStatus.InProgress)
        {
            return;
        }
        
        // Find the objective
        QuestObjective objective = quest.objectives.Find(obj => obj.objectiveId == objectiveId);
        
        if (objective != null)
        {
            // Update progress
            objective.UpdateProgress(amount);
            
            // Trigger event
            OnQuestObjectiveUpdated?.Invoke(quest, objective);
            
            // Check if all objectives are completed
            if (quest.AreAllObjectivesCompleted())
            {
                CompleteQuest(questId);
            }
        }
    }
    
    // Award quest rewards
    private void AwardQuestRewards(Quest quest)
    {
        QuestReward reward = quest.reward;
        
        // Award currency
        if (reward.currency > 0)
        {
            GameManager.Instance.AddCurrency(reward.currency);
        }
        
        // Award experience
        if (reward.experience > 0)
        {
            GameManager.Instance.AddExperience(reward.experience);
            
            // Also award experience to current pet if any
            if (GameManager.Instance.currentPet != null)
            {
                Pet pet = GameManager.Instance.currentPet.GetComponent<Pet>();
                if (pet != null)
                {
                    pet.AddExperience(reward.experience);
                }
            }
        }
        
        // Award items
        for (int i = 0; i < reward.itemIds.Count && i < reward.itemQuantities.Count; i++)
        {
            string itemId = reward.itemIds[i];
            int quantity = reward.itemQuantities[i];
            
            ItemSystem.Instance.AddItem(itemId, quantity);
        }
        
        // Unlock location if any
        if (!string.IsNullOrEmpty(reward.unlockedLocationId))
        {
            GameManager.Instance.UnlockLocation(reward.unlockedLocationId);
        }
        
        // Unlock pet if any
        if (!string.IsNullOrEmpty(reward.unlockedPetId))
        {
            GameManager.Instance.UnlockPet(reward.unlockedPetId);
        }
    }
    
    // Update achievement progress
    public void UpdateAchievement(string achievementId, int amount = 1)
    {
        if (!achievementDictionary.ContainsKey(achievementId))
        {
            Debug.LogWarning($"Achievement with ID {achievementId} not found!");
            return;
        }
        
        Achievement achievement = achievementDictionary[achievementId];
        
        // Check if achievement is already unlocked
        if (achievement.isUnlocked)
        {
            return;
        }
        
        // Update progress
        int previousProgress = achievement.currentProgress;
        achievement.UpdateProgress(amount);
        
        // Check if achievement was just unlocked
        if (!achievement.isUnlocked || previousProgress >= achievement.progressRequired)
        {
            return;
        }
        
        // Award currency reward
        if (achievement.rewardCurrency > 0)
        {
            GameManager.Instance.AddCurrency(achievement.rewardCurrency);
        }
        
        // Trigger event
        OnAchievementUnlocked?.Invoke(achievement);
    }
    
    // Get all available quests (not started and prerequisites met)
    public List<Quest> GetAvailableQuests()
    {
        List<Quest> availableQuests = new List<Quest>();
        
        foreach (Quest quest in allQuests)
        {
            if (quest.status == QuestStatus.NotStarted && !quest.isHidden)
            {
                bool prerequisitesMet = true;
                
                // Check prerequisites
                foreach (string prerequisiteId in quest.prerequisiteQuestIds)
                {
                    if (questDictionary.ContainsKey(prerequisiteId))
                    {
                        Quest prerequisiteQuest = questDictionary[prerequisiteId];
                        if (prerequisiteQuest.status != QuestStatus.Completed)
                        {
                            prerequisitesMet = false;
                            break;
                        }
                    }
                }
                
                if (prerequisitesMet)
                {
                    availableQuests.Add(quest);
                }
            }
        }
        
        return availableQuests;
    }
    
    // Get all completed quests
    public List<Quest> GetCompletedQuests()
    {
        List<Quest> completedQuests = new List<Quest>();
        
        foreach (Quest quest in allQuests)
        {
            if (quest.status == QuestStatus.Completed)
            {
                completedQuests.Add(quest);
            }
        }
        
        return completedQuests;
    }
    
    // Get all unlocked achievements
    public List<Achievement> GetUnlockedAchievements()
    {
        List<Achievement> unlockedAchievements = new List<Achievement>();
        
        foreach (Achievement achievement in achievements)
        {
            if (achievement.isUnlocked)
            {
                unlockedAchievements.Add(achievement);
            }
        }
        
        return unlockedAchievements;
    }
    
    // Get achievement progress percentage
    public float GetAchievementProgress()
    {
        if (achievements.Count == 0)
            return 0f;
        
        int unlockedCount = 0;
        
        foreach (Achievement achievement in achievements)
        {
            if (achievement.isUnlocked)
            {
                unlockedCount++;
            }
        }
        
        return (float)unlockedCount / achievements.Count;
    }
}