using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class GameLocation
{
    public string locationName;
    public string sceneName;
    public Sprite locationIcon;
    public string description;
    public bool isUnlocked = true;
}

[Serializable]
public class SeasonalEvent
{
    public string eventName;
    public Sprite eventIcon;
    public DateTime startDate;
    public DateTime endDate;
    public Color themeColor = Color.white;
    public List<GameObject> seasonalItems = new List<GameObject>();
    public List<GameObject> seasonalDecorations = new List<GameObject>();
}

[Serializable]
public class PlayerData
{
    public string playerName = "Player";
    public int currency = 100;
    public int level = 1;
    public float experience = 0f;
    public List<string> unlockedPets = new List<string>();
    public List<string> unlockedItems = new List<string>();
    public List<string> unlockedLocations = new List<string>();
    public Dictionary<string, int> achievements = new Dictionary<string, int>();
}

public enum TimeOfDay
{
    Morning,
    Afternoon,
    Evening,
    Night
}

public enum Weather
{
    Sunny,
    Cloudy,
    Rainy,
    Snowy,
    Foggy
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("Pet Management")]
    public GameObject currentPet;
    public List<GameObject> availablePetPrefabs = new List<GameObject>();
    public Transform petSpawnPoint;
    
    [Header("Game World")]
    public List<GameLocation> gameLocations = new List<GameLocation>();
    public string currentLocation = "Pet Home";
    
    [Header("Time System")]
    public float timeScale = 1f;
    public float dayDuration = 480f; // 8 minutes = 1 day
    public TimeOfDay currentTimeOfDay = TimeOfDay.Morning;
    private float currentDayTime = 0f;
    
    [Header("Weather System")]
    public Weather currentWeather = Weather.Sunny;
    public float weatherChangeInterval = 300f; // 5 minutes
    private float weatherTimer = 0f;
    
    [Header("Seasonal Events")]
    public List<SeasonalEvent> seasonalEvents = new List<SeasonalEvent>();
    public SeasonalEvent currentEvent;
    
    [Header("Player Data")]
    public PlayerData playerData = new PlayerData();
    
    [Header("UI References")]
    public GameObject mainMenuUI;
    public GameObject gameplayUI;
    public GameObject shopUI;
    public GameObject settingsUI;
    
    // Private variables
    private bool isGamePaused = false;
    private Dictionary<string, GameObject> petPrefabDictionary = new Dictionary<string, GameObject>();
    
    void Awake()
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
        
        // Initialize pet prefab dictionary for easy access
        foreach (GameObject petPrefab in availablePetPrefabs)
        {
            Pet pet = petPrefab.GetComponent<Pet>();
            if (pet != null)
            {
                petPrefabDictionary[pet.petName] = petPrefab;
            }
        }
        
        // Initialize player data
        if (playerData.unlockedPets.Count == 0 && availablePetPrefabs.Count > 0)
        {
            // Give player at least one pet to start with
            Pet startingPet = availablePetPrefabs[0].GetComponent<Pet>();
            if (startingPet != null)
            {
                playerData.unlockedPets.Add(startingPet.petName);
            }
        }
        
        // Start the day/night cycle
        StartCoroutine(DayNightCycle());
        
        // Start weather system
        StartCoroutine(WeatherSystem());
        
        // Check for seasonal events
        CheckForSeasonalEvents();
    }
    
    void Update()
    {
        // Update day time
        currentDayTime += Time.deltaTime * timeScale;
        if (currentDayTime >= dayDuration)
        {
            currentDayTime = 0f;
        }
        
        // Update time of day
        UpdateTimeOfDay();
        
        // Update weather timer
        weatherTimer += Time.deltaTime;
        if (weatherTimer >= weatherChangeInterval)
        {
            weatherTimer = 0f;
            ChangeWeather();
        }
    }
    
    private void UpdateTimeOfDay()
    {
        float normalizedTime = currentDayTime / dayDuration;
        
        if (normalizedTime < 0.25f)
        {
            currentTimeOfDay = TimeOfDay.Morning;
        }
        else if (normalizedTime < 0.5f)
        {
            currentTimeOfDay = TimeOfDay.Afternoon;
        }
        else if (normalizedTime < 0.75f)
        {
            currentTimeOfDay = TimeOfDay.Evening;
        }
        else
        {
            currentTimeOfDay = TimeOfDay.Night;
        }
    }
    
    private IEnumerator DayNightCycle()
    {
        while (true)
        {
            // Update lighting and environment based on time of day
            float normalizedTime = currentDayTime / dayDuration;
            
            // Here you would update directional light, skybox, ambient lighting, etc.
            // For example:
            // directionalLight.intensity = Mathf.Lerp(0.1f, 1f, Mathf.Sin(normalizedTime * Mathf.PI));
            
            yield return null;
        }
    }
    
    private IEnumerator WeatherSystem()
    {
        while (true)
        {
            // Wait for weather change interval
            yield return new WaitForSeconds(weatherChangeInterval);
            
            // Change weather
            ChangeWeather();
        }
    }
    
    private void ChangeWeather()
    {
        // Randomly select new weather
        Weather[] weatherTypes = (Weather[])Enum.GetValues(typeof(Weather));
        Weather newWeather = (Weather)UnityEngine.Random.Range(0, weatherTypes.Length);
        
        // Don't pick the same weather twice in a row
        if (newWeather == currentWeather)
        {
            newWeather = (Weather)(((int)newWeather + 1) % weatherTypes.Length);
        }
        
        currentWeather = newWeather;
        
        // Apply weather effects (would be implemented with particle systems, post-processing, etc.)
        ApplyWeatherEffects();
    }
    
    private void ApplyWeatherEffects()
    {
        // Apply different effects based on current weather
        switch (currentWeather)
        {
            case Weather.Sunny:
                Debug.Log("Weather changed to Sunny");
                // Activate sun particles, bright lighting
                break;
            case Weather.Cloudy:
                Debug.Log("Weather changed to Cloudy");
                // Dim lighting, add cloud shadows
                break;
            case Weather.Rainy:
                Debug.Log("Weather changed to Rainy");
                // Activate rain particles, wet surface effects
                break;
            case Weather.Snowy:
                Debug.Log("Weather changed to Snowy");
                // Activate snow particles, add snow accumulation
                break;
            case Weather.Foggy:
                Debug.Log("Weather changed to Foggy");
                // Activate fog effect, reduce visibility
                break;
        }
    }
    
    private void CheckForSeasonalEvents()
    {
        DateTime currentDate = DateTime.Now;
        
        foreach (SeasonalEvent seasonalEvent in seasonalEvents)
        {
            if (currentDate >= seasonalEvent.startDate && currentDate <= seasonalEvent.endDate)
            {
                // Activate this seasonal event
                currentEvent = seasonalEvent;
                ApplySeasonalEvent(seasonalEvent);
                break;
            }
        }
    }
    
    private void ApplySeasonalEvent(SeasonalEvent seasonalEvent)
    {
        Debug.Log($"Seasonal event active: {seasonalEvent.eventName}");
        
        // Spawn seasonal decorations
        foreach (GameObject decoration in seasonalEvent.seasonalDecorations)
        {
            // Instantiate decorations at appropriate locations
            // This would be implemented with specific spawn points in the scene
        }
        
        // Add seasonal items to shop
        // This would update the shop inventory
    }
    
    public void SpawnPet(GameObject petPrefab)
    {
        if (currentPet != null)
        {
            Destroy(currentPet);
        }
        
        Vector3 spawnPosition = petSpawnPoint != null ? petSpawnPoint.position : Vector3.zero;
        currentPet = Instantiate(petPrefab, spawnPosition, Quaternion.identity);
    }
    
    public void SpawnPetByName(string petName)
    {
        if (petPrefabDictionary.ContainsKey(petName))
        {
            SpawnPet(petPrefabDictionary[petName]);
        }
        else
        {
            Debug.LogWarning($"Pet prefab with name {petName} not found!");
        }
    }
    
    public void ChangeLocation(string locationName)
    {
        foreach (GameLocation location in gameLocations)
        {
            if (location.locationName == locationName && location.isUnlocked)
            {
                currentLocation = locationName;
                SceneManager.LoadScene(location.sceneName);
                return;
            }
        }
        
        Debug.LogWarning($"Location {locationName} not found or not unlocked!");
    }
    
    public void AddCurrency(int amount)
    {
        playerData.currency += amount;
    }
    
    public bool SpendCurrency(int amount)
    {
        if (playerData.currency >= amount)
        {
            playerData.currency -= amount;
            return true;
        }
        return false;
    }
    
    public void AddExperience(float amount)
    {
        playerData.experience += amount;
        float experienceNeeded = playerData.level * 100;
        
        if (playerData.experience >= experienceNeeded)
        {
            playerData.level++;
            playerData.experience -= experienceNeeded;
            
            // Reward for leveling up
            AddCurrency(playerData.level * 10);
            
            Debug.Log($"Player leveled up to level {playerData.level}!");
        }
    }
    
    public void UnlockPet(string petName)
    {
        if (!playerData.unlockedPets.Contains(petName))
        {
            playerData.unlockedPets.Add(petName);
        }
    }
    
    public void UnlockItem(string itemName)
    {
        if (!playerData.unlockedItems.Contains(itemName))
        {
            playerData.unlockedItems.Add(itemName);
        }
    }
    
    public void UnlockLocation(string locationName)
    {
        if (!playerData.unlockedLocations.Contains(locationName))
        {
            playerData.unlockedLocations.Add(locationName);
            
            // Also update the location in the gameLocations list
            foreach (GameLocation location in gameLocations)
            {
                if (location.locationName == locationName)
                {
                    location.isUnlocked = true;
                    break;
                }
            }
        }
    }
    
    public void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
    }
    
    public void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
    }
    
    public void ShowMainMenu()
    {
        if (mainMenuUI != null) mainMenuUI.SetActive(true);
        if (gameplayUI != null) gameplayUI.SetActive(false);
        if (shopUI != null) shopUI.SetActive(false);
        if (settingsUI != null) settingsUI.SetActive(false);
    }
    
    public void ShowGameplayUI()
    {
        if (mainMenuUI != null) mainMenuUI.SetActive(false);
        if (gameplayUI != null) gameplayUI.SetActive(true);
        if (shopUI != null) shopUI.SetActive(false);
        if (settingsUI != null) settingsUI.SetActive(false);
    }
    
    public void ShowShopUI()
    {
        if (mainMenuUI != null) mainMenuUI.SetActive(false);
        if (gameplayUI != null) gameplayUI.SetActive(false);
        if (shopUI != null) shopUI.SetActive(true);
        if (settingsUI != null) settingsUI.SetActive(false);
    }
    
    public void ShowSettingsUI()
    {
        if (mainMenuUI != null) mainMenuUI.SetActive(false);
        if (gameplayUI != null) gameplayUI.SetActive(false);
        if (shopUI != null) shopUI.SetActive(false);
        if (settingsUI != null) settingsUI.SetActive(true);
    }
    
    // Save and load game data
    public void SaveGame()
    {
        // This would be implemented with PlayerPrefs or a more robust save system
        Debug.Log("Game saved!");
    }
    
    public void LoadGame()
    {
        // This would be implemented with PlayerPrefs or a more robust save system
        Debug.Log("Game loaded!");
    }
}