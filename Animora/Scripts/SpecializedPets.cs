using UnityEngine;
using System.Collections;

// Base class for specialized pets with unique abilities
public abstract class SpecializedPet : Pet
{
    [Header("Special Ability")]
    public string specialAbilityName = "Special Ability";
    public string specialAbilityDescription = "This pet has a special ability.";
    public float specialAbilityCooldown = 60f; // seconds
    public ParticleSystem specialAbilityEffect;
    public AudioClip specialAbilitySound;
    
    protected float lastSpecialAbilityTime = -999f;
    
    public bool CanUseSpecialAbility()
    {
        return Time.time - lastSpecialAbilityTime >= specialAbilityCooldown;
    }
    
    public virtual void UseSpecialAbility()
    {
        if (!CanUseSpecialAbility())
            return;
            
        lastSpecialAbilityTime = Time.time;
        
        // Play effects
        if (specialAbilityEffect != null)
        {
            specialAbilityEffect.Play();
        }
        
        // Play sound
        if (specialAbilitySound != null)
        {
            AudioSource.PlayClipAtPoint(specialAbilitySound, transform.position);
        }
    }
}

// Dragon pet with fire breath ability
public class DragonPet : SpecializedPet
{
    [Header("Dragon Settings")]
    public float fireBreathRange = 5f;
    public float fireBreathDamage = 10f;
    public GameObject fireBreathPrefab;
    
    private void Start()
    {
        petType = PetType.Dragon;
        specialAbilityName = "Fire Breath";
        specialAbilityDescription = "Breathes fire that can light candles and campfires.";
    }
    
    public override void UseSpecialAbility()
    {
        base.UseSpecialAbility();
        
        // Dragon-specific fire breath implementation
        if (fireBreathPrefab != null)
        {
            GameObject fireBreath = Instantiate(fireBreathPrefab, transform.position + transform.forward * 0.5f, transform.rotation);
            fireBreath.transform.parent = transform;
            
            // Find any interactable objects in range
            Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward, fireBreathRange);
            foreach (var hitCollider in hitColliders)
            {
                // Check for objects that can be lit
                IFlammable flammable = hitCollider.GetComponent<IFlammable>();
                if (flammable != null)
                {
                    flammable.Ignite();
                }
            }
            
            // Destroy the fire breath effect after a delay
            Destroy(fireBreath, 3f);
        }
    }
}

// Unicorn pet with healing ability
public class UnicornPet : SpecializedPet
{
    [Header("Unicorn Settings")]
    public float healingRange = 5f;
    public float healingAmount = 20f;
    public GameObject healingAuraPrefab;
    
    private void Start()
    {
        petType = PetType.Unicorn;
        specialAbilityName = "Healing Aura";
        specialAbilityDescription = "Creates a magical aura that heals nearby pets.";
    }
    
    public override void UseSpecialAbility()
    {
        base.UseSpecialAbility();
        
        // Unicorn-specific healing implementation
        if (healingAuraPrefab != null)
        {
            GameObject healingAura = Instantiate(healingAuraPrefab, transform.position, Quaternion.identity);
            healingAura.transform.parent = transform;
            
            // Find any pets in range
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, healingRange);
            foreach (var hitCollider in hitColliders)
            {
                Pet otherPet = hitCollider.GetComponent<Pet>();
                if (otherPet != null && otherPet != this)
                {
                    // Heal the pet
                    otherPet.stats.health = Mathf.Min(100f, otherPet.stats.health + healingAmount);
                    
                    // Show healing effect on the pet
                    GameObject healEffect = Instantiate(healingAuraPrefab, otherPet.transform.position, Quaternion.identity);
                    healEffect.transform.localScale = Vector3.one * 0.5f;
                    Destroy(healEffect, 2f);
                }
            }
            
            // Destroy the healing aura after a delay
            Destroy(healingAura, 5f);
        }
    }
}

// Phoenix pet with rebirth ability
public class PhoenixPet : SpecializedPet
{
    [Header("Phoenix Settings")]
    public GameObject rebirthEffectPrefab;
    public float healthBoostOnRebirth = 50f;
    
    private bool hasRebirthAvailable = true;
    
    private void Start()
    {
        petType = PetType.Phoenix;
        specialAbilityName = "Rebirth";
        specialAbilityDescription = "Can be reborn from ashes once per day, restoring health.";
    }
    
    private void Update()
    {
        base.Update();
        
        // Auto-trigger rebirth when health is critically low
        if (stats.health < 10f && hasRebirthAvailable)
        {
            UseSpecialAbility();
        }
    }
    
    public override void UseSpecialAbility()
    {
        if (!hasRebirthAvailable)
            return;
            
        base.UseSpecialAbility();
        
        // Phoenix-specific rebirth implementation
        if (rebirthEffectPrefab != null)
        {
            GameObject rebirthEffect = Instantiate(rebirthEffectPrefab, transform.position, Quaternion.identity);
            
            // Hide the phoenix briefly
            StartCoroutine(RebirthSequence(rebirthEffect));
        }
        
        hasRebirthAvailable = false;
        
        // Reset rebirth availability after 24 hours (game time)
        StartCoroutine(ResetRebirthAvailability());
    }
    
    private IEnumerator RebirthSequence(GameObject rebirthEffect)
    {
        // Hide the phoenix
        GetComponent<Renderer>().enabled = false;
        
        // Wait for the effect
        yield return new WaitForSeconds(3f);
        
        // Restore health
        stats.health = Mathf.Min(100f, stats.health + healthBoostOnRebirth);
        
        // Show the phoenix again
        GetComponent<Renderer>().enabled = true;
        
        // Destroy the effect
        Destroy(rebirthEffect);
    }
    
    private IEnumerator ResetRebirthAvailability()
    {
        // Wait for 24 hours of game time (this would be adjusted based on your game's time scale)
        yield return new WaitForSeconds(1440f); // 24 minutes = 24 game hours at 1 min = 1 hour
        
        hasRebirthAvailable = true;
    }
}

// Robot pet with upgrade ability
public class RobotPet : SpecializedPet
{
    [Header("Robot Settings")]
    public int batteryLevel = 100;
    public float batteryDrainRate = 0.5f; // per minute
    public GameObject upgradeFX;
    
    [System.Serializable]
    public class RobotUpgrade
    {
        public string upgradeName;
        public string description;
        public int level = 0;
        public int maxLevel = 3;
        public float[] bonusValues; // Different values for each level
    }
    
    public RobotUpgrade[] availableUpgrades = new RobotUpgrade[]
    {
        new RobotUpgrade { 
            upgradeName = "Battery Efficiency", 
            description = "Reduces battery drain rate",
            bonusValues = new float[] { 0.1f, 0.2f, 0.3f }
        },
        new RobotUpgrade { 
            upgradeName = "Movement Speed", 
            description = "Increases movement speed",
            bonusValues = new float[] { 0.15f, 0.3f, 0.5f }
        },
        new RobotUpgrade { 
            upgradeName = "Learning Algorithm", 
            description = "Increases experience gain",
            bonusValues = new float[] { 0.1f, 0.25f, 0.5f }
        }
    };
    
    private void Start()
    {
        petType = PetType.Robot;
        specialAbilityName = "System Upgrade";
        specialAbilityDescription = "Installs upgrades to improve various systems.";
    }
    
    private void Update()
    {
        base.Update();
        
        // Drain battery over time
        float effectiveDrainRate = batteryDrainRate;
        
        // Apply battery efficiency upgrade if available
        if (availableUpgrades[0].level > 0)
        {
            effectiveDrainRate *= (1f - availableUpgrades[0].bonusValues[availableUpgrades[0].level - 1]);
        }
        
        batteryLevel = Mathf.Max(0, batteryLevel - (int)(effectiveDrainRate * Time.deltaTime / 60f));
        
        // If battery is depleted, reduce energy
        if (batteryLevel <= 0)
        {
            stats.energy = Mathf.Max(0, stats.energy - 5f * Time.deltaTime);
        }
        
        // Apply movement speed upgrade if available
        if (availableUpgrades[1].level > 0)
        {
            float speedBonus = availableUpgrades[1].bonusValues[availableUpgrades[1].level - 1];
            GetComponent<NavMeshAgent>().speed = baseMovementSpeed * (1f + speedBonus);
        }
    }
    
    public override void UseSpecialAbility()
    {
        base.UseSpecialAbility();
        
        // Show upgrade interface
        // This would typically trigger a UI element for the player to select an upgrade
        
        // For demonstration, let's just upgrade a random system
        int randomUpgradeIndex = Random.Range(0, availableUpgrades.Length);
        UpgradeSystem(randomUpgradeIndex);
    }
    
    public void UpgradeSystem(int upgradeIndex)
    {
        if (upgradeIndex < 0 || upgradeIndex >= availableUpgrades.Length)
            return;
            
        RobotUpgrade upgrade = availableUpgrades[upgradeIndex];
        
        if (upgrade.level >= upgrade.maxLevel)
            return;
            
        // Increase upgrade level
        upgrade.level++;
        
        // Show upgrade effect
        if (upgradeFX != null)
        {
            GameObject fx = Instantiate(upgradeFX, transform.position, Quaternion.identity);
            fx.transform.parent = transform;
            Destroy(fx, 3f);
        }
        
        // Apply immediate effects of the upgrade
        if (upgradeIndex == 0) // Battery Efficiency
        {
            // Already handled in Update()
        }
        else if (upgradeIndex == 1) // Movement Speed
        {
            // Already handled in Update()
        }
        else if (upgradeIndex == 2) // Learning Algorithm
        {
            // This will be applied when gaining experience
        }
    }
    
    // Override AddExperience to apply Learning Algorithm upgrade
    public override void AddExperience(float amount)
    {
        if (availableUpgrades[2].level > 0)
        {
            float expBonus = availableUpgrades[2].bonusValues[availableUpgrades[2].level - 1];
            amount *= (1f + expBonus);
        }
        
        base.AddExperience(amount);
    }
    
    // Recharge battery
    public void RechargeBattery(int amount)
    {
        batteryLevel = Mathf.Min(100, batteryLevel + amount);
        
        // Also restore some energy
        stats.energy = Mathf.Min(100f, stats.energy + amount * 0.5f);
    }
}

// Slime pet with split ability
public class SlimePet : SpecializedPet
{
    [Header("Slime Settings")]
    public Color slimeColor = Color.green;
    public GameObject slimeSplitPrefab;
    public float splitDuration = 30f; // How long the split slime exists
    
    private GameObject splitSlime = null;
    
    private void Start()
    {
        petType = PetType.Slime;
        specialAbilityName = "Split";
        specialAbilityDescription = "Can temporarily split into two slimes.";
        
        // Set the slime color
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = slimeColor;
        }
    }
    
    public override void UseSpecialAbility()
    {
        base.UseSpecialAbility();
        
        // Don't allow splitting if already split
        if (splitSlime != null)
            return;
            
        // Create a split slime
        if (slimeSplitPrefab != null)
        {
            // Create the split slime slightly offset from the original
            Vector3 spawnPos = transform.position + transform.right * 1.5f;
            splitSlime = Instantiate(slimeSplitPrefab, spawnPos, transform.rotation);
            
            // Set the same color
            Renderer splitRenderer = splitSlime.GetComponent<Renderer>();
            if (splitRenderer != null)
            {
                splitRenderer.material.color = slimeColor;
            }
            
            // Make the split slime smaller
            splitSlime.transform.localScale = transform.localScale * 0.7f;
            
            // Destroy the split slime after the duration
            StartCoroutine(ReabsorbSplit());
        }
    }
    
    private IEnumerator ReabsorbSplit()
    {
        yield return new WaitForSeconds(splitDuration);
        
        if (splitSlime != null)
        {
            // Play reabsorb effect
            ParticleSystem reabsorbFX = GetComponentInChildren<ParticleSystem>();
            if (reabsorbFX != null)
            {
                reabsorbFX.Play();
            }
            
            // Destroy the split slime
            Destroy(splitSlime);
            splitSlime = null;
            
            // Gain a small mood boost from splitting
            stats.mood = Mathf.Min(100f, stats.mood + 10f);
        }
    }
}

// Interface for objects that can be ignited by the dragon
public interface IFlammable
{
    void Ignite();
}