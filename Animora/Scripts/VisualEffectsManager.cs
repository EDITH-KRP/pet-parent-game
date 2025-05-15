using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ParticleEffectPrefab
{
    public string effectName;
    public ParticleSystem particlePrefab;
    public AudioClip soundEffect;
    public float duration = 2f;
    public bool looping = false;
    public bool attachToTarget = false;
}

public class VisualEffectsManager : MonoBehaviour
{
    public static VisualEffectsManager Instance;
    
    [Header("Particle Effects")]
    public List<ParticleEffectPrefab> particleEffects = new List<ParticleEffectPrefab>();
    
    [Header("Pet Interaction Effects")]
    public ParticleEffectPrefab feedingEffect;
    public ParticleEffectPrefab playingEffect;
    public ParticleEffectPrefab cleaningEffect;
    public ParticleEffectPrefab healingEffect;
    public ParticleEffectPrefab sleepingEffect;
    public ParticleEffectPrefab levelUpEffect;
    
    [Header("UI Effects")]
    public ParticleEffectPrefab currencyGainEffect;
    public ParticleEffectPrefab itemAcquiredEffect;
    public ParticleEffectPrefab questCompletedEffect;
    public ParticleEffectPrefab achievementUnlockedEffect;
    
    [Header("Environment Effects")]
    public ParticleSystem rainEffect;
    public ParticleSystem snowEffect;
    public ParticleSystem fogEffect;
    public ParticleSystem leavesEffect;
    
    [Header("Settings")]
    public bool enableParticleEffects = true;
    public float globalParticleScale = 1f;
    public float globalParticleSpeed = 1f;
    
    // Dictionary for quick lookup
    private Dictionary<string, ParticleEffectPrefab> effectDictionary = new Dictionary<string, ParticleEffectPrefab>();
    
    // Active particle systems
    private List<ParticleSystem> activeParticleSystems = new List<ParticleSystem>();
    
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
        foreach (ParticleEffectPrefab effect in particleEffects)
        {
            effectDictionary[effect.effectName] = effect;
        }
        
        // Add pet interaction effects to dictionary
        if (feedingEffect != null) effectDictionary["Feeding"] = feedingEffect;
        if (playingEffect != null) effectDictionary["Playing"] = playingEffect;
        if (cleaningEffect != null) effectDictionary["Cleaning"] = cleaningEffect;
        if (healingEffect != null) effectDictionary["Healing"] = healingEffect;
        if (sleepingEffect != null) effectDictionary["Sleeping"] = sleepingEffect;
        if (levelUpEffect != null) effectDictionary["LevelUp"] = levelUpEffect;
        
        // Add UI effects to dictionary
        if (currencyGainEffect != null) effectDictionary["CurrencyGain"] = currencyGainEffect;
        if (itemAcquiredEffect != null) effectDictionary["ItemAcquired"] = itemAcquiredEffect;
        if (questCompletedEffect != null) effectDictionary["QuestCompleted"] = questCompletedEffect;
        if (achievementUnlockedEffect != null) effectDictionary["AchievementUnlocked"] = achievementUnlockedEffect;
    }
    
    private void Update()
    {
        // Clean up completed particle systems
        for (int i = activeParticleSystems.Count - 1; i >= 0; i--)
        {
            ParticleSystem ps = activeParticleSystems[i];
            if (ps == null || !ps.IsAlive())
            {
                if (ps != null)
                {
                    Destroy(ps.gameObject);
                }
                activeParticleSystems.RemoveAt(i);
            }
        }
    }
    
    // Play a particle effect by name
    public ParticleSystem PlayEffect(string effectName, Vector3 position, Quaternion rotation = default, Transform parent = null)
    {
        if (!enableParticleEffects)
            return null;
            
        if (!effectDictionary.ContainsKey(effectName))
        {
            Debug.LogWarning($"Effect '{effectName}' not found!");
            return null;
        }
        
        ParticleEffectPrefab effectPrefab = effectDictionary[effectName];
        
        if (effectPrefab.particlePrefab == null)
        {
            Debug.LogWarning($"Particle prefab for effect '{effectName}' is null!");
            return null;
        }
        
        // Create the particle system
        ParticleSystem particleSystem = Instantiate(effectPrefab.particlePrefab, position, rotation);
        
        // Apply global settings
        var main = particleSystem.main;
        main.startSizeMultiplier *= globalParticleScale;
        main.simulationSpeed = globalParticleSpeed;
        
        // Set parent if specified and attachment is enabled
        if (parent != null && effectPrefab.attachToTarget)
        {
            particleSystem.transform.SetParent(parent);
            particleSystem.transform.localPosition = Vector3.zero;
        }
        
        // Play sound effect if available
        if (effectPrefab.soundEffect != null)
        {
            AudioSource.PlayClipAtPoint(effectPrefab.soundEffect, position);
        }
        
        // Start the particle system
        particleSystem.Play();
        
        // Add to active list
        activeParticleSystems.Add(particleSystem);
        
        // Destroy after duration if not looping
        if (!effectPrefab.looping)
        {
            Destroy(particleSystem.gameObject, effectPrefab.duration);
        }
        
        return particleSystem;
    }
    
    // Play a particle effect on a pet
    public ParticleSystem PlayPetEffect(string effectName, Pet pet)
    {
        if (pet == null)
            return null;
            
        return PlayEffect(effectName, pet.transform.position + Vector3.up * 0.5f, Quaternion.identity, pet.transform);
    }
    
    // Play a UI effect at a screen position
    public ParticleSystem PlayUIEffect(string effectName, Vector2 screenPosition)
    {
        if (!enableParticleEffects)
            return null;
            
        if (!effectDictionary.ContainsKey(effectName))
        {
            Debug.LogWarning($"Effect '{effectName}' not found!");
            return null;
        }
        
        // Convert screen position to world position
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 10f));
        
        return PlayEffect(effectName, worldPosition);
    }
    
    // Play weather effect
    public void PlayWeatherEffect(Weather weather)
    {
        // Stop all current weather effects
        StopWeatherEffects();
        
        // Play the appropriate weather effect
        switch (weather)
        {
            case Weather.Rainy:
                if (rainEffect != null)
                {
                    rainEffect.gameObject.SetActive(true);
                    rainEffect.Play();
                }
                break;
                
            case Weather.Snowy:
                if (snowEffect != null)
                {
                    snowEffect.gameObject.SetActive(true);
                    snowEffect.Play();
                }
                break;
                
            case Weather.Foggy:
                if (fogEffect != null)
                {
                    fogEffect.gameObject.SetActive(true);
                    fogEffect.Play();
                }
                break;
                
            case Weather.Windy:
                if (leavesEffect != null)
                {
                    leavesEffect.gameObject.SetActive(true);
                    leavesEffect.Play();
                }
                break;
        }
    }
    
    // Stop all weather effects
    public void StopWeatherEffects()
    {
        if (rainEffect != null)
        {
            rainEffect.Stop();
            rainEffect.gameObject.SetActive(false);
        }
        
        if (snowEffect != null)
        {
            snowEffect.Stop();
            snowEffect.gameObject.SetActive(false);
        }
        
        if (fogEffect != null)
        {
            fogEffect.Stop();
            fogEffect.gameObject.SetActive(false);
        }
        
        if (leavesEffect != null)
        {
            leavesEffect.Stop();
            leavesEffect.gameObject.SetActive(false);
        }
    }
    
    // Create a custom particle effect
    public ParticleSystem CreateCustomEffect(string effectName, Color color, float size, float duration, bool looping = false)
    {
        if (!enableParticleEffects)
            return null;
            
        // Use a default effect as a template
        if (!effectDictionary.ContainsKey("Feeding")) // Use feeding as default template
        {
            Debug.LogWarning("No template effect found for custom effect!");
            return null;
        }
        
        ParticleEffectPrefab templateEffect = effectDictionary["Feeding"];
        
        // Create a new particle system based on the template
        ParticleSystem newEffect = Instantiate(templateEffect.particlePrefab);
        
        // Customize the particle system
        var main = newEffect.main;
        main.startColor = color;
        main.startSize = size * globalParticleScale;
        main.duration = duration;
        main.loop = looping;
        
        // Add to dictionary for future use
        ParticleEffectPrefab newEffectPrefab = new ParticleEffectPrefab
        {
            effectName = effectName,
            particlePrefab = newEffect,
            duration = duration,
            looping = looping
        };
        
        effectDictionary[effectName] = newEffectPrefab;
        particleEffects.Add(newEffectPrefab);
        
        return newEffect;
    }
    
    // Stop and destroy all active particle systems
    public void ClearAllEffects()
    {
        foreach (ParticleSystem ps in activeParticleSystems)
        {
            if (ps != null)
            {
                ps.Stop();
                Destroy(ps.gameObject);
            }
        }
        
        activeParticleSystems.Clear();
        StopWeatherEffects();
    }
}