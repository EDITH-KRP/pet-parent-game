using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PetAnimationSet
{
    public PetType petType;
    public RuntimeAnimatorController animatorController;
    public Avatar avatar;
    
    [Header("Animation Clips")]
    public AnimationClip idleAnimation;
    public AnimationClip walkAnimation;
    public AnimationClip runAnimation;
    public AnimationClip sleepAnimation;
    public AnimationClip eatAnimation;
    public AnimationClip playAnimation;
    public AnimationClip cleanAnimation;
    public AnimationClip sickAnimation;
    public AnimationClip happyAnimation;
    public AnimationClip sadAnimation;
    
    [Header("Special Animations")]
    public AnimationClip specialAbilityAnimation;
    public AnimationClip levelUpAnimation;
}

[System.Serializable]
public class MoodAnimationThreshold
{
    public float moodThreshold;
    public string animationTrigger;
}

public class PetAnimationController : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public Pet pet;
    
    [Header("Animation Sets")]
    public List<PetAnimationSet> animationSets = new List<PetAnimationSet>();
    
    [Header("Animation Parameters")]
    public string activityParam = "Activity";
    public string speedParam = "Speed";
    public string moodParam = "Mood";
    public string healthParam = "Health";
    
    [Header("Animation Triggers")]
    public string eatTrigger = "Eat";
    public string playTrigger = "Play";
    public string cleanTrigger = "Clean";
    public string sleepTrigger = "Sleep";
    public string wakeTrigger = "Wake";
    public string specialTrigger = "Special";
    public string levelUpTrigger = "LevelUp";
    
    [Header("Mood Animation")]
    public List<MoodAnimationThreshold> moodThresholds = new List<MoodAnimationThreshold>();
    
    [Header("Blend Settings")]
    public float animationBlendTime = 0.25f;
    public float movementAnimationThreshold = 0.1f;
    
    // Private variables
    private PetAnimationSet currentAnimationSet;
    private Dictionary<PetType, PetAnimationSet> animationSetDictionary = new Dictionary<PetType, PetAnimationSet>();
    private PetActivity lastActivity = PetActivity.Idle;
    private float lastMood = 50f;
    private float lastHealth = 100f;
    
    private void Awake()
    {
        // Get references if not assigned
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        
        if (pet == null)
        {
            pet = GetComponent<Pet>();
        }
        
        // Initialize animation set dictionary
        foreach (PetAnimationSet animSet in animationSets)
        {
            animationSetDictionary[animSet.petType] = animSet;
        }
    }
    
    private void Start()
    {
        // Set up the appropriate animation set for this pet
        if (pet != null)
        {
            SetAnimationSetForPetType(pet.petType);
        }
    }
    
    private void Update()
    {
        if (pet == null || animator == null)
            return;
            
        // Update animation parameters based on pet state
        UpdateAnimationParameters();
        
        // Check for mood changes
        CheckMoodChanges();
        
        // Check for health changes
        CheckHealthChanges();
        
        // Check for activity changes
        CheckActivityChanges();
    }
    
    // Set the animation set for a specific pet type
    public void SetAnimationSetForPetType(PetType petType)
    {
        if (animationSetDictionary.ContainsKey(petType))
        {
            currentAnimationSet = animationSetDictionary[petType];
            
            // Apply animator controller and avatar
            if (animator != null && currentAnimationSet.animatorController != null)
            {
                animator.runtimeAnimatorController = currentAnimationSet.animatorController;
                
                if (currentAnimationSet.avatar != null)
                {
                    animator.avatar = currentAnimationSet.avatar;
                }
            }
        }
        else
        {
            Debug.LogWarning($"No animation set found for pet type: {petType}");
        }
    }
    
    // Update animation parameters based on pet state
    private void UpdateAnimationParameters()
    {
        // Set activity parameter
        animator.SetInteger(activityParam, (int)pet.currentActivity);
        
        // Set speed parameter based on movement
        float speed = 0f;
        if (pet.navAgent != null && pet.navAgent.enabled)
        {
            speed = pet.navAgent.velocity.magnitude / pet.navAgent.speed;
        }
        animator.SetFloat(speedParam, speed, animationBlendTime, Time.deltaTime);
        
        // Set mood parameter
        animator.SetFloat(moodParam, pet.stats.mood / 100f);
        
        // Set health parameter
        animator.SetFloat(healthParam, pet.stats.health / 100f);
    }
    
    // Check for mood changes and trigger appropriate animations
    private void CheckMoodChanges()
    {
        float currentMood = pet.stats.mood;
        
        // Check if mood crossed any thresholds
        foreach (MoodAnimationThreshold threshold in moodThresholds)
        {
            if ((lastMood < threshold.moodThreshold && currentMood >= threshold.moodThreshold) ||
                (lastMood >= threshold.moodThreshold && currentMood < threshold.moodThreshold))
            {
                // Trigger the animation
                animator.SetTrigger(threshold.animationTrigger);
                break;
            }
        }
        
        lastMood = currentMood;
    }
    
    // Check for health changes and trigger appropriate animations
    private void CheckHealthChanges()
    {
        float currentHealth = pet.stats.health;
        
        // Check if health dropped significantly
        if (lastHealth - currentHealth > 20f)
        {
            // Trigger sick animation
            animator.SetTrigger("Sick");
        }
        
        lastHealth = currentHealth;
    }
    
    // Check for activity changes and trigger appropriate animations
    private void CheckActivityChanges()
    {
        PetActivity currentActivity = pet.currentActivity;
        
        // If activity changed, trigger appropriate animation
        if (currentActivity != lastActivity)
        {
            switch (currentActivity)
            {
                case PetActivity.Eating:
                    animator.SetTrigger(eatTrigger);
                    break;
                    
                case PetActivity.Playing:
                    animator.SetTrigger(playTrigger);
                    break;
                    
                case PetActivity.Sleeping:
                    animator.SetTrigger(sleepTrigger);
                    break;
                    
                case PetActivity.Bathing:
                    animator.SetTrigger(cleanTrigger);
                    break;
            }
            
            // If waking up from sleep
            if (lastActivity == PetActivity.Sleeping && currentActivity != PetActivity.Sleeping)
            {
                animator.SetTrigger(wakeTrigger);
            }
        }
        
        lastActivity = currentActivity;
    }
    
    // Trigger specific animations
    
    public void TriggerEatAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(eatTrigger);
        }
    }
    
    public void TriggerPlayAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(playTrigger);
        }
    }
    
    public void TriggerCleanAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(cleanTrigger);
        }
    }
    
    public void TriggerSleepAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(sleepTrigger);
        }
    }
    
    public void TriggerWakeAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(wakeTrigger);
        }
    }
    
    public void TriggerSpecialAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(specialTrigger);
        }
    }
    
    public void TriggerLevelUpAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(levelUpTrigger);
        }
    }
    
    // Play a specific animation clip directly
    public void PlayAnimationClip(string clipName, float crossfadeTime = 0.25f)
    {
        if (animator != null)
        {
            animator.CrossFade(clipName, crossfadeTime);
        }
    }
    
    // Get animation clip by name from current animation set
    public AnimationClip GetAnimationClip(string clipName)
    {
        if (currentAnimationSet == null)
            return null;
            
        switch (clipName.ToLower())
        {
            case "idle":
                return currentAnimationSet.idleAnimation;
            case "walk":
                return currentAnimationSet.walkAnimation;
            case "run":
                return currentAnimationSet.runAnimation;
            case "sleep":
                return currentAnimationSet.sleepAnimation;
            case "eat":
                return currentAnimationSet.eatAnimation;
            case "play":
                return currentAnimationSet.playAnimation;
            case "clean":
                return currentAnimationSet.cleanAnimation;
            case "sick":
                return currentAnimationSet.sickAnimation;
            case "happy":
                return currentAnimationSet.happyAnimation;
            case "sad":
                return currentAnimationSet.sadAnimation;
            case "special":
                return currentAnimationSet.specialAbilityAnimation;
            case "levelup":
                return currentAnimationSet.levelUpAnimation;
            default:
                return null;
        }
    }
    
    // Get animation duration
    public float GetAnimationDuration(string clipName)
    {
        AnimationClip clip = GetAnimationClip(clipName);
        
        if (clip != null)
        {
            return clip.length;
        }
        
        return 0f;
    }
}