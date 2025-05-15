using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldLocation
{
    public string locationName;
    public Transform locationTransform;
    public List<Transform> interactionPoints = new List<Transform>();
    public List<GameObject> locationObjects = new List<GameObject>();
    public bool isIndoor = false;
}

[System.Serializable]
public class WeatherEffect
{
    public Weather weatherType;
    public ParticleSystem particleEffect;
    public AudioClip ambientSound;
    public Material skyboxMaterial;
    public Color fogColor = Color.white;
    public float fogDensity = 0.01f;
}

[System.Serializable]
public class TimeOfDaySettings
{
    public TimeOfDay timeOfDay;
    public Color ambientColor;
    public Color skyColor;
    public float lightIntensity = 1f;
    public Vector3 lightRotation;
}

public class WorldManager : MonoBehaviour
{
    [Header("World Locations")]
    public List<WorldLocation> locations = new List<WorldLocation>();
    
    [Header("Weather Effects")]
    public List<WeatherEffect> weatherEffects = new List<WeatherEffect>();
    public Transform weatherEffectsParent;
    
    [Header("Time of Day")]
    public List<TimeOfDaySettings> timeOfDaySettings = new List<TimeOfDaySettings>();
    public Light directionalLight;
    
    [Header("NPCs")]
    public List<GameObject> npcPrefabs = new List<GameObject>();
    public int maxNPCs = 5;
    public float npcSpawnInterval = 300f; // 5 minutes
    
    [Header("Interactive Objects")]
    public List<GameObject> interactiveObjectPrefabs = new List<GameObject>();
    
    // Private variables
    private Dictionary<string, WorldLocation> locationDictionary = new Dictionary<string, WorldLocation>();
    private ParticleSystem currentWeatherEffect;
    private AudioSource ambientAudioSource;
    private List<GameObject> activeNPCs = new List<GameObject>();
    private float npcSpawnTimer = 0f;
    
    private void Awake()
    {
        // Initialize location dictionary
        foreach (WorldLocation location in locations)
        {
            locationDictionary[location.locationName] = location;
        }
        
        // Initialize ambient audio source
        ambientAudioSource = gameObject.AddComponent<AudioSource>();
        ambientAudioSource.loop = true;
        ambientAudioSource.spatialBlend = 0f; // 2D sound
        ambientAudioSource.volume = 0.3f;
    }
    
    private void Start()
    {
        // Subscribe to game manager events
        GameManager.Instance.OnWeatherChanged += UpdateWeatherEffects;
        GameManager.Instance.OnTimeOfDayChanged += UpdateTimeOfDay;
        GameManager.Instance.OnLocationChanged += UpdateLocation;
        
        // Initial updates
        UpdateWeatherEffects(GameManager.Instance.currentWeather);
        UpdateTimeOfDay(GameManager.Instance.currentTimeOfDay);
        UpdateLocation(GameManager.Instance.currentLocation);
        
        // Start NPC spawning
        StartCoroutine(ManageNPCs());
    }
    
    private void Update()
    {
        // Update NPC spawn timer
        npcSpawnTimer += Time.deltaTime;
        if (npcSpawnTimer >= npcSpawnInterval)
        {
            npcSpawnTimer = 0f;
            SpawnRandomNPC();
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnWeatherChanged -= UpdateWeatherEffects;
            GameManager.Instance.OnTimeOfDayChanged -= UpdateTimeOfDay;
            GameManager.Instance.OnLocationChanged -= UpdateLocation;
        }
    }
    
    public void UpdateWeatherEffects(Weather weather)
    {
        // Stop current weather effect
        if (currentWeatherEffect != null)
        {
            currentWeatherEffect.Stop();
        }
        
        // Find matching weather effect
        WeatherEffect effect = weatherEffects.Find(e => e.weatherType == weather);
        
        if (effect != null)
        {
            // Apply skybox
            if (effect.skyboxMaterial != null)
            {
                RenderSettings.skybox = effect.skyboxMaterial;
            }
            
            // Apply fog
            RenderSettings.fogColor = effect.fogColor;
            RenderSettings.fogDensity = effect.fogDensity;
            
            // Start particle effect
            if (effect.particleEffect != null)
            {
                currentWeatherEffect = effect.particleEffect;
                currentWeatherEffect.Play();
            }
            
            // Play ambient sound
            if (effect.ambientSound != null && ambientAudioSource != null)
            {
                ambientAudioSource.clip = effect.ambientSound;
                ambientAudioSource.Play();
            }
        }
    }
    
    public void UpdateTimeOfDay(TimeOfDay timeOfDay)
    {
        // Find matching time of day settings
        TimeOfDaySettings settings = timeOfDaySettings.Find(s => s.timeOfDay == timeOfDay);
        
        if (settings != null)
        {
            // Apply ambient light
            RenderSettings.ambientLight = settings.ambientColor;
            
            // Apply directional light settings
            if (directionalLight != null)
            {
                directionalLight.intensity = settings.lightIntensity;
                directionalLight.transform.rotation = Quaternion.Euler(settings.lightRotation);
            }
            
            // Apply sky color
            Camera.main.backgroundColor = settings.skyColor;
        }
    }
    
    public void UpdateLocation(string locationName)
    {
        if (locationDictionary.ContainsKey(locationName))
        {
            WorldLocation location = locationDictionary[locationName];
            
            // Enable this location's objects
            foreach (GameObject obj in location.locationObjects)
            {
                obj.SetActive(true);
            }
            
            // Disable other locations' objects
            foreach (WorldLocation otherLocation in locations)
            {
                if (otherLocation.locationName != locationName)
                {
                    foreach (GameObject obj in otherLocation.locationObjects)
                    {
                        obj.SetActive(false);
                    }
                }
            }
            
            // Update fog based on indoor/outdoor
            RenderSettings.fog = !location.isIndoor;
            
            // Disable weather effects if indoor
            if (location.isIndoor && currentWeatherEffect != null)
            {
                currentWeatherEffect.Stop();
            }
            else if (!location.isIndoor && currentWeatherEffect != null)
            {
                currentWeatherEffect.Play();
            }
            
            // Spawn interactive objects at this location
            SpawnInteractiveObjects(location);
        }
    }
    
    private void SpawnInteractiveObjects(WorldLocation location)
    {
        // Clear existing interactive objects
        // This would need to be implemented with a tag or layer system
        
        // Spawn new interactive objects at interaction points
        foreach (Transform point in location.interactionPoints)
        {
            // Randomly decide whether to spawn an object
            if (Random.value > 0.3f)
            {
                // Pick a random interactive object
                int randomIndex = Random.Range(0, interactiveObjectPrefabs.Count);
                GameObject prefab = interactiveObjectPrefabs[randomIndex];
                
                // Instantiate at interaction point
                Instantiate(prefab, point.position, point.rotation);
            }
        }
    }
    
    private IEnumerator ManageNPCs()
    {
        while (true)
        {
            // Wait a bit
            yield return new WaitForSeconds(10f);
            
            // Clean up inactive NPCs
            for (int i = activeNPCs.Count - 1; i >= 0; i--)
            {
                if (activeNPCs[i] == null)
                {
                    activeNPCs.RemoveAt(i);
                }
            }
            
            // Spawn NPCs if needed
            while (activeNPCs.Count < maxNPCs)
            {
                SpawnRandomNPC();
                yield return new WaitForSeconds(1f);
            }
        }
    }
    
    private void SpawnRandomNPC()
    {
        if (npcPrefabs.Count == 0 || activeNPCs.Count >= maxNPCs)
            return;
        
        // Get current location
        if (locationDictionary.ContainsKey(GameManager.Instance.currentLocation))
        {
            WorldLocation location = locationDictionary[GameManager.Instance.currentLocation];
            
            if (location.interactionPoints.Count > 0)
            {
                // Pick a random spawn point
                int randomPointIndex = Random.Range(0, location.interactionPoints.Count);
                Transform spawnPoint = location.interactionPoints[randomPointIndex];
                
                // Pick a random NPC prefab
                int randomNPCIndex = Random.Range(0, npcPrefabs.Count);
                GameObject npcPrefab = npcPrefabs[randomNPCIndex];
                
                // Spawn NPC
                GameObject npc = Instantiate(npcPrefab, spawnPoint.position, spawnPoint.rotation);
                activeNPCs.Add(npc);
                
                // Set up NPC behavior
                // This would be implemented with an NPC behavior script
            }
        }
    }
    
    // Public method to get a random interaction point at the current location
    public Transform GetRandomInteractionPoint()
    {
        if (locationDictionary.ContainsKey(GameManager.Instance.currentLocation))
        {
            WorldLocation location = locationDictionary[GameManager.Instance.currentLocation];
            
            if (location.interactionPoints.Count > 0)
            {
                int randomIndex = Random.Range(0, location.interactionPoints.Count);
                return location.interactionPoints[randomIndex];
            }
        }
        
        return null;
    }
}