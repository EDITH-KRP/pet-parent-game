using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PetTypeBalancing
{
    public PetType petType;
    
    [Header("Stat Decay Rates (per minute)")]
    public float hungerDecayRate = 2f;
    public float moodDecayRate = 1.5f;
    public float energyDecayRate = 1f;
    public float cleanlinessDecayRate = 1.2f;
    public float healthDecayRate = 0.5f;
    
    [Header("Stat Recovery Rates (per action)")]
    public float feedingEffect = 25f;
    public float playingEffect = 30f;
    public float sleepingEffect = 40f; // per minute
    public float cleaningEffect = 50f;
    public float healingEffect = 35f;
    
    [Header("Experience Gain")]
    public float baseExperienceRate = 1f;
    public float levelMultiplier = 1.2f;
    
    [Header("Special Traits")]
    public bool isNocturnal = false;
    public bool prefersIndoors = false;
    public bool prefersOutdoors = false;
    public Weather preferredWeather = Weather.Sunny;
    public Weather dislikedWeather = Weather.Rainy;
}

public class PetBalancing : MonoBehaviour
{
    public static PetBalancing Instance;
    
    [Header("General Settings")]
    public float globalStatDecayMultiplier = 1f;
    public float globalExperienceMultiplier = 1f;
    public int experiencePerLevel = 100;
    public float levelExperienceScaling = 1.5f;
    
    [Header("Pet Type Balancing")]
    public List<PetTypeBalancing> petTypeBalancing = new List<PetTypeBalancing>();
    
    [Header("Environmental Effects")]
    public float indoorCleanlinessDecayMultiplier = 0.7f;
    public float outdoorCleanlinessDecayMultiplier = 1.5f;
    public float rainyWeatherEnergyDecayMultiplier = 1.3f;
    public float snowyWeatherEnergyDecayMultiplier = 1.5f;
    public float nightTimeEnergyRecoveryMultiplier = 1.5f;
    
    [Header("Mood Modifiers")]
    public float lowHungerMoodPenalty = -0.5f; // per minute when hunger < 20
    public float lowEnergyMoodPenalty = -0.5f; // per minute when energy < 20
    public float lowCleanlinessMoodPenalty = -0.5f; // per minute when cleanliness < 20
    public float lowHealthMoodPenalty = -1f; // per minute when health < 20
    
    [Header("Health Modifiers")]
    public float lowHungerHealthPenalty = -0.2f; // per minute when hunger < 10
    public float lowCleanlinessHealthPenalty = -0.2f; // per minute when cleanliness < 10
    
    // Dictionary for quick lookup
    private Dictionary<PetType, PetTypeBalancing> balancingDict = new Dictionary<PetType, PetTypeBalancing>();
    
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
        
        // Initialize dictionary
        foreach (PetTypeBalancing balancing in petTypeBalancing)
        {
            balancingDict[balancing.petType] = balancing;
        }
        
        // Create default balancing for all pet types if not defined
        foreach (PetType petType in System.Enum.GetValues(typeof(PetType)))
        {
            if (!balancingDict.ContainsKey(petType))
            {
                CreateDefaultBalancing(petType);
            }
        }
    }
    
    private void CreateDefaultBalancing(PetType petType)
    {
        PetTypeBalancing balancing = new PetTypeBalancing
        {
            petType = petType
        };
        
        // Adjust based on pet type
        switch (petType)
        {
            case PetType.Dog:
                balancing.hungerDecayRate = 2.5f;
                balancing.moodDecayRate = 1.8f;
                balancing.energyDecayRate = 2.2f;
                balancing.cleanlinessDecayRate = 2.0f;
                balancing.prefersOutdoors = true;
                break;
                
            case PetType.Cat:
                balancing.hungerDecayRate = 1.8f;
                balancing.moodDecayRate = 2.0f;
                balancing.energyDecayRate = 1.5f;
                balancing.cleanlinessDecayRate = 1.0f;
                balancing.isNocturnal = true;
                break;
                
            case PetType.Dragon:
                balancing.hungerDecayRate = 3.0f;
                balancing.moodDecayRate = 1.5f;
                balancing.energyDecayRate = 1.0f;
                balancing.cleanlinessDecayRate = 0.8f;
                balancing.preferredWeather = Weather.Sunny;
                balancing.dislikedWeather = Weather.Snowy;
                break;
                
            case PetType.Unicorn:
                balancing.hungerDecayRate = 1.5f;
                balancing.moodDecayRate = 2.2f;
                balancing.energyDecayRate = 1.8f;
                balancing.cleanlinessDecayRate = 0.5f;
                balancing.preferredWeather = Weather.Sunny;
                balancing.baseExperienceRate = 1.2f;
                break;
                
            case PetType.Fox:
                balancing.hungerDecayRate = 2.2f;
                balancing.moodDecayRate = 1.7f;
                balancing.energyDecayRate = 2.0f;
                balancing.cleanlinessDecayRate = 1.8f;
                balancing.isNocturnal = true;
                balancing.prefersOutdoors = true;
                break;
                
            case PetType.Rabbit:
                balancing.hungerDecayRate = 2.8f;
                balancing.moodDecayRate = 2.0f;
                balancing.energyDecayRate = 2.5f;
                balancing.cleanlinessDecayRate = 1.5f;
                balancing.prefersOutdoors = true;
                break;
                
            case PetType.Owl:
                balancing.hungerDecayRate = 1.7f;
                balancing.moodDecayRate = 1.5f;
                balancing.energyDecayRate = 1.3f;
                balancing.cleanlinessDecayRate = 0.7f;
                balancing.isNocturnal = true;
                break;
                
            case PetType.Phoenix:
                balancing.hungerDecayRate = 2.0f;
                balancing.moodDecayRate = 1.8f;
                balancing.energyDecayRate = 1.2f;
                balancing.cleanlinessDecayRate = 0.5f;
                balancing.healthDecayRate = 0.3f;
                balancing.preferredWeather = Weather.Sunny;
                balancing.baseExperienceRate = 1.3f;
                break;
                
            case PetType.Robot:
                balancing.hungerDecayRate = 1.0f;
                balancing.moodDecayRate = 1.2f;
                balancing.energyDecayRate = 2.5f;
                balancing.cleanlinessDecayRate = 0.8f;
                balancing.healthDecayRate = 0.7f;
                balancing.dislikedWeather = Weather.Rainy;
                break;
                
            case PetType.Slime:
                balancing.hungerDecayRate = 1.5f;
                balancing.moodDecayRate = 1.3f;
                balancing.energyDecayRate = 1.0f;
                balancing.cleanlinessDecayRate = 0.0f; // Slimes don't get dirty
                balancing.healthDecayRate = 0.8f;
                balancing.preferredWeather = Weather.Rainy;
                break;
                
            case PetType.Panda:
                balancing.hungerDecayRate = 2.7f;
                balancing.moodDecayRate = 1.4f;
                balancing.energyDecayRate = 1.7f;
                balancing.cleanlinessDecayRate = 1.3f;
                balancing.prefersOutdoors = true;
                break;
                
            case PetType.Penguin:
                balancing.hungerDecayRate = 2.3f;
                balancing.moodDecayRate = 1.6f;
                balancing.energyDecayRate = 1.5f;
                balancing.cleanlinessDecayRate = 1.0f;
                balancing.preferredWeather = Weather.Snowy;
                balancing.dislikedWeather = Weather.Sunny;
                break;
        }
        
        petTypeBalancing.Add(balancing);
        balancingDict[petType] = balancing;
    }
    
    // Get balancing for a specific pet type
    public PetTypeBalancing GetBalancing(PetType petType)
    {
        if (balancingDict.ContainsKey(petType))
        {
            return balancingDict[petType];
        }
        
        // Create default balancing if not found
        CreateDefaultBalancing(petType);
        return balancingDict[petType];
    }
    
    // Calculate stat decay rates for a pet
    public void CalculateDecayRates(Pet pet, out float hungerDecay, out float moodDecay, out float energyDecay, out float cleanlinessDecay, out float healthDecay)
    {
        PetTypeBalancing balancing = GetBalancing(pet.petType);
        
        // Base decay rates
        hungerDecay = balancing.hungerDecayRate * globalStatDecayMultiplier;
        moodDecay = balancing.moodDecayRate * globalStatDecayMultiplier;
        energyDecay = balancing.energyDecayRate * globalStatDecayMultiplier;
        cleanlinessDecay = balancing.cleanlinessDecayRate * globalStatDecayMultiplier;
        healthDecay = balancing.healthDecayRate * globalStatDecayMultiplier;
        
        // Apply environmental modifiers
        
        // Indoor/Outdoor effects
        bool isIndoor = GameManager.Instance.IsCurrentLocationIndoor();
        if (isIndoor)
        {
            cleanlinessDecay *= indoorCleanlinessDecayMultiplier;
            
            // Preference modifiers
            if (balancing.prefersOutdoors)
            {
                moodDecay *= 1.2f; // Pets that prefer outdoors get sadder indoors
            }
            else if (balancing.prefersIndoors)
            {
                moodDecay *= 0.8f; // Pets that prefer indoors get happier indoors
            }
        }
        else
        {
            cleanlinessDecay *= outdoorCleanlinessDecayMultiplier;
            
            // Preference modifiers
            if (balancing.prefersOutdoors)
            {
                moodDecay *= 0.8f; // Pets that prefer outdoors get happier outdoors
            }
            else if (balancing.prefersIndoors)
            {
                moodDecay *= 1.2f; // Pets that prefer indoors get sadder outdoors
            }
        }
        
        // Weather effects
        Weather currentWeather = GameManager.Instance.currentWeather;
        if (currentWeather == Weather.Rainy)
        {
            energyDecay *= rainyWeatherEnergyDecayMultiplier;
            
            if (currentWeather == balancing.dislikedWeather)
            {
                moodDecay *= 1.3f;
            }
            else if (currentWeather == balancing.preferredWeather)
            {
                moodDecay *= 0.7f;
            }
        }
        else if (currentWeather == Weather.Snowy)
        {
            energyDecay *= snowyWeatherEnergyDecayMultiplier;
            
            if (currentWeather == balancing.dislikedWeather)
            {
                moodDecay *= 1.3f;
                energyDecay *= 1.2f;
            }
            else if (currentWeather == balancing.preferredWeather)
            {
                moodDecay *= 0.7f;
            }
        }
        else if (currentWeather == balancing.preferredWeather)
        {
            moodDecay *= 0.7f;
        }
        else if (currentWeather == balancing.dislikedWeather)
        {
            moodDecay *= 1.3f;
        }
        
        // Time of day effects
        TimeOfDay currentTime = GameManager.Instance.currentTimeOfDay;
        bool isNight = (currentTime == TimeOfDay.Night);
        
        if (isNight)
        {
            if (balancing.isNocturnal)
            {
                energyDecay *= 0.7f; // Nocturnal pets have more energy at night
                moodDecay *= 0.8f; // Nocturnal pets are happier at night
            }
            else
            {
                energyDecay *= 1.2f; // Non-nocturnal pets lose energy faster at night
            }
        }
        else
        {
            if (balancing.isNocturnal)
            {
                energyDecay *= 1.2f; // Nocturnal pets lose energy faster during the day
            }
        }
        
        // Stat-based modifiers
        
        // Low hunger affects mood and health
        if (pet.stats.hunger < 20f)
        {
            moodDecay += lowHungerMoodPenalty;
            
            if (pet.stats.hunger < 10f)
            {
                healthDecay += lowHungerHealthPenalty;
            }
        }
        
        // Low energy affects mood
        if (pet.stats.energy < 20f)
        {
            moodDecay += lowEnergyMoodPenalty;
        }
        
        // Low cleanliness affects mood and health
        if (pet.stats.cleanliness < 20f)
        {
            moodDecay += lowCleanlinessMoodPenalty;
            
            if (pet.stats.cleanliness < 10f)
            {
                healthDecay += lowCleanlinessHealthPenalty;
            }
        }
        
        // Low health affects mood
        if (pet.stats.health < 20f)
        {
            moodDecay += lowHealthMoodPenalty;
        }
    }
    
    // Calculate experience required for a level
    public int GetExperienceForLevel(int level)
    {
        return Mathf.RoundToInt(experiencePerLevel * Mathf.Pow(level, levelExperienceScaling));
    }
    
    // Calculate experience gain for an action
    public float CalculateExperienceGain(Pet pet, float baseAmount)
    {
        PetTypeBalancing balancing = GetBalancing(pet.petType);
        
        float amount = baseAmount * balancing.baseExperienceRate * globalExperienceMultiplier;
        
        // Apply level scaling
        amount *= 1f + (pet.petLevel - 1) * (balancing.levelMultiplier - 1f);
        
        return amount;
    }
    
    // Calculate feeding effect
    public float CalculateFeedingEffect(Pet pet)
    {
        PetTypeBalancing balancing = GetBalancing(pet.petType);
        return balancing.feedingEffect;
    }
    
    // Calculate playing effect
    public float CalculatePlayingEffect(Pet pet)
    {
        PetTypeBalancing balancing = GetBalancing(pet.petType);
        return balancing.playingEffect;
    }
    
    // Calculate sleeping effect per minute
    public float CalculateSleepingEffect(Pet pet)
    {
        PetTypeBalancing balancing = GetBalancing(pet.petType);
        float effect = balancing.sleepingEffect;
        
        // Boost recovery at night for all pets
        if (GameManager.Instance.currentTimeOfDay == TimeOfDay.Night)
        {
            effect *= nightTimeEnergyRecoveryMultiplier;
        }
        
        return effect;
    }
    
    // Calculate cleaning effect
    public float CalculateCleaningEffect(Pet pet)
    {
        PetTypeBalancing balancing = GetBalancing(pet.petType);
        return balancing.cleaningEffect;
    }
    
    // Calculate healing effect
    public float CalculateHealingEffect(Pet pet)
    {
        PetTypeBalancing balancing = GetBalancing(pet.petType);
        return balancing.healingEffect;
    }
}