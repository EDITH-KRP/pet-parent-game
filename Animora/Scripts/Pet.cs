using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum PetType
{
    Dog,
    Cat,
    Dragon,
    Unicorn,
    Fox,
    Rabbit,
    Owl,
    Phoenix,
    Robot,
    Slime,
    Panda,
    Penguin
}

public enum PetMood
{
    Happy,
    Content,
    Neutral,
    Sad,
    Sick
}

public enum PetActivity
{
    Idle,
    Playing,
    Sleeping,
    Eating,
    Bathing
}

[Serializable]
public class PetStats
{
    [Range(0, 100)] public float hunger = 100f;
    [Range(0, 100)] public float mood = 100f;
    [Range(0, 100)] public float energy = 100f;
    [Range(0, 100)] public float cleanliness = 100f;
    [Range(0, 100)] public float health = 100f;
    
    // Stat decay rates (per second)
    public float hungerDecayRate = 0.05f;
    public float moodDecayRate = 0.03f;
    public float energyDecayRate = 0.02f;
    public float cleanlinessDecayRate = 0.04f;
}

public class Pet : MonoBehaviour
{
    [Header("Pet Identity")]
    public string petName = "Unnamed Pet";
    public PetType petType;
    public int petLevel = 1;
    public float petExperience = 0f;
    
    [Header("Pet Stats")]
    public PetStats stats = new PetStats();
    
    [Header("State")]
    public PetMood currentMood = PetMood.Content;
    public PetActivity currentActivity = PetActivity.Idle;
    
    [Header("Components")]
    public Animator animator;
    public AudioSource audioSource;
    public ParticleSystem emotionParticles;
    public NavMeshAgent navAgent;
    
    [Header("Audio Clips")]
    public AudioClip[] happySounds;
    public AudioClip[] sadSounds;
    public AudioClip[] eatingSounds;
    public AudioClip[] playingSounds;
    
    [Header("AI Behavior")]
    public Transform[] wanderPoints;
    public float minWanderTime = 5f;
    public float maxWanderTime = 15f;
    public float reactionDistance = 2f;
    
    // Private variables
    private float nextWanderTime;
    private float moodUpdateTimer;
    private float aiDecisionTimer;
    private bool isPlayerInteracting;
    private Vector3 targetPosition;
    private Coroutine currentBehaviorRoutine;
    
    private void Awake()
    {
        // Initialize components if not set
        if (animator == null) animator = GetComponent<Animator>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (navAgent == null) navAgent = GetComponent<NavMeshAgent>();
    }
    
    private void Start()
    {
        // Initialize AI behavior
        nextWanderTime = Time.time + UnityEngine.Random.Range(minWanderTime, maxWanderTime);
        UpdateMoodState();
        StartCoroutine(PeriodicAIDecision());
    }
    
    private void Update()
    {
        // Update stats based on time
        UpdateStats();
        
        // Update mood every few seconds
        moodUpdateTimer -= Time.deltaTime;
        if (moodUpdateTimer <= 0)
        {
            UpdateMoodState();
            moodUpdateTimer = 3f; // Check mood every 3 seconds
        }
        
        // Handle AI behavior if player isn't interacting
        if (!isPlayerInteracting)
        {
            HandleAIBehavior();
        }
        
        // Update animations based on current state
        UpdateAnimations();
    }
    
    private void UpdateStats()
    {
        // Decrease stats over time
        stats.hunger = Mathf.Max(0, stats.hunger - stats.hungerDecayRate * Time.deltaTime);
        stats.mood = Mathf.Max(0, stats.mood - stats.moodDecayRate * Time.deltaTime);
        stats.energy = Mathf.Max(0, stats.energy - stats.energyDecayRate * Time.deltaTime);
        stats.cleanliness = Mathf.Max(0, stats.cleanliness - stats.cleanlinessDecayRate * Time.deltaTime);
        
        // Health decreases if other stats are very low
        if (stats.hunger < 20 || stats.energy < 20 || stats.cleanliness < 20)
        {
            stats.health = Mathf.Max(0, stats.health - 0.1f * Time.deltaTime);
        }
        else if (stats.health < 100 && stats.hunger > 50 && stats.energy > 50)
        {
            // Health regenerates slowly if other stats are good
            stats.health = Mathf.Min(100, stats.health + 0.05f * Time.deltaTime);
        }
    }
    
    private void UpdateMoodState()
    {
        // Calculate average well-being
        float wellbeing = (stats.hunger + stats.mood + stats.energy + stats.cleanliness + stats.health) / 5f;
        
        // Set mood based on well-being
        if (wellbeing > 80) currentMood = PetMood.Happy;
        else if (wellbeing > 60) currentMood = PetMood.Content;
        else if (wellbeing > 40) currentMood = PetMood.Neutral;
        else if (wellbeing > 20) currentMood = PetMood.Sad;
        else currentMood = PetMood.Sick;
        
        // Play appropriate emotion particles
        if (emotionParticles != null)
        {
            var main = emotionParticles.main;
            
            switch (currentMood)
            {
                case PetMood.Happy:
                    main.startColor = Color.yellow;
                    if (!emotionParticles.isPlaying) emotionParticles.Play();
                    break;
                case PetMood.Sad:
                    main.startColor = Color.blue;
                    if (!emotionParticles.isPlaying) emotionParticles.Play();
                    break;
                case PetMood.Sick:
                    main.startColor = Color.green;
                    if (!emotionParticles.isPlaying) emotionParticles.Play();
                    break;
                default:
                    if (emotionParticles.isPlaying) emotionParticles.Stop();
                    break;
            }
        }
    }
    
    private void HandleAIBehavior()
    {
        // Wander randomly if it's time
        if (Time.time > nextWanderTime && currentActivity == PetActivity.Idle)
        {
            if (wanderPoints != null && wanderPoints.Length > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, wanderPoints.Length);
                MoveToPosition(wanderPoints[randomIndex].position);
            }
            else
            {
                // If no wander points, just move to a random position nearby
                Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * 5f;
                randomDirection += transform.position;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomDirection, out hit, 5f, NavMesh.AllAreas))
                {
                    MoveToPosition(hit.position);
                }
            }
            
            nextWanderTime = Time.time + UnityEngine.Random.Range(minWanderTime, maxWanderTime);
        }
        
        // If energy is very low, find a place to sleep
        if (stats.energy < 20 && currentActivity != PetActivity.Sleeping)
        {
            StartCoroutine(SleepBehavior());
        }
    }
    
    private void MoveToPosition(Vector3 position)
    {
        if (navAgent != null && navAgent.isActiveAndEnabled)
        {
            navAgent.SetDestination(position);
            currentActivity = PetActivity.Playing;
            
            // After reaching destination, go back to idle
            StartCoroutine(ReturnToIdleAfterReachingDestination());
        }
    }
    
    private IEnumerator ReturnToIdleAfterReachingDestination()
    {
        while (navAgent.pathPending || navAgent.remainingDistance > navAgent.stoppingDistance)
        {
            yield return null;
        }
        
        currentActivity = PetActivity.Idle;
    }
    
    private IEnumerator PeriodicAIDecision()
    {
        while (true)
        {
            // Wait between 5-10 seconds before making a new decision
            yield return new WaitForSeconds(UnityEngine.Random.Range(5f, 10f));
            
            // Only make decisions if not already engaged in an activity
            if (!isPlayerInteracting && currentActivity == PetActivity.Idle)
            {
                // Make decisions based on needs
                if (stats.hunger < 30)
                {
                    // Pet is hungry, look for food
                    Debug.Log($"{petName} is looking for food");
                    PlaySound(sadSounds);
                }
                else if (stats.cleanliness < 30)
                {
                    // Pet is dirty, look for cleaning
                    Debug.Log($"{petName} wants to be cleaned");
                    PlaySound(sadSounds);
                }
                else if (stats.mood < 30)
                {
                    // Pet is sad, look for toys
                    Debug.Log($"{petName} wants to play");
                    PlaySound(sadSounds);
                }
                else if (stats.energy < 30)
                {
                    // Pet is tired, look for bed
                    StartCoroutine(SleepBehavior());
                }
                else
                {
                    // Pet is content, random behavior
                    int randomBehavior = UnityEngine.Random.Range(0, 3);
                    switch (randomBehavior)
                    {
                        case 0:
                            // Play animation of pet being happy
                            PlayHappyAnimation();
                            break;
                        case 1:
                            // Wander to a random point
                            if (wanderPoints != null && wanderPoints.Length > 0)
                            {
                                int randomIndex = UnityEngine.Random.Range(0, wanderPoints.Length);
                                MoveToPosition(wanderPoints[randomIndex].position);
                            }
                            break;
                        case 2:
                            // Just idle
                            break;
                    }
                }
            }
        }
    }
    
    private IEnumerator SleepBehavior()
    {
        // Stop current movement
        if (navAgent != null) navAgent.isStopped = true;
        
        currentActivity = PetActivity.Sleeping;
        
        // Play sleep animation
        if (animator != null)
        {
            animator.SetTrigger("Sleep");
        }
        
        // Recover energy while sleeping
        float sleepDuration = UnityEngine.Random.Range(5f, 15f);
        float startTime = Time.time;
        
        while (Time.time < startTime + sleepDuration && !isPlayerInteracting)
        {
            stats.energy = Mathf.Min(100, stats.energy + 1f * Time.deltaTime);
            yield return null;
        }
        
        // Wake up
        currentActivity = PetActivity.Idle;
        if (navAgent != null) navAgent.isStopped = false;
        
        // Play wake up animation
        if (animator != null)
        {
            animator.SetTrigger("WakeUp");
        }
    }
    
    private void UpdateAnimations()
    {
        if (animator != null)
        {
            // Set animation parameters based on current state
            animator.SetInteger("Mood", (int)currentMood);
            animator.SetInteger("Activity", (int)currentActivity);
            
            // Set movement parameters
            if (navAgent != null)
            {
                animator.SetFloat("Speed", navAgent.velocity.magnitude);
            }
        }
    }
    
    private void PlaySound(AudioClip[] sounds)
    {
        if (audioSource != null && sounds != null && sounds.Length > 0)
        {
            AudioClip clip = sounds[UnityEngine.Random.Range(0, sounds.Length)];
            audioSource.PlayOneShot(clip);
        }
    }
    
    private void PlayHappyAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Happy");
            PlaySound(happySounds);
        }
    }
    
    // Public interaction methods
    public void Feed()
    {
        StartCoroutine(FeedingBehavior());
    }
    
    private IEnumerator FeedingBehavior()
    {
        isPlayerInteracting = true;
        currentActivity = PetActivity.Eating;
        
        // Play eating animation
        if (animator != null)
        {
            animator.SetTrigger("Eat");
        }
        
        // Play eating sound
        PlaySound(eatingSounds);
        
        // Increase hunger stat
        stats.hunger = Mathf.Min(100, stats.hunger + 50);
        
        // Wait for eating animation to complete
        yield return new WaitForSeconds(3f);
        
        isPlayerInteracting = false;
        currentActivity = PetActivity.Idle;
    }
    
    public void Play()
    {
        StartCoroutine(PlayingBehavior());
    }
    
    private IEnumerator PlayingBehavior()
    {
        isPlayerInteracting = true;
        currentActivity = PetActivity.Playing;
        
        // Play playing animation
        if (animator != null)
        {
            animator.SetTrigger("Play");
        }
        
        // Play playing sound
        PlaySound(playingSounds);
        
        // Increase mood stat but decrease energy
        stats.mood = Mathf.Min(100, stats.mood + 40);
        stats.energy = Mathf.Max(0, stats.energy - 10);
        
        // Wait for playing animation to complete
        yield return new WaitForSeconds(5f);
        
        isPlayerInteracting = false;
        currentActivity = PetActivity.Idle;
    }
    
    public void Clean()
    {
        StartCoroutine(CleaningBehavior());
    }
    
    private IEnumerator CleaningBehavior()
    {
        isPlayerInteracting = true;
        currentActivity = PetActivity.Bathing;
        
        // Play bathing animation
        if (animator != null)
        {
            animator.SetTrigger("Bath");
        }
        
        // Increase cleanliness stat
        stats.cleanliness = Mathf.Min(100, stats.cleanliness + 60);
        
        // Wait for bathing animation to complete
        yield return new WaitForSeconds(4f);
        
        isPlayerInteracting = false;
        currentActivity = PetActivity.Idle;
    }
    
    public void Sleep()
    {
        if (currentActivity != PetActivity.Sleeping)
        {
            StartCoroutine(SleepBehavior());
        }
    }
    
    public void Heal()
    {
        stats.health = Mathf.Min(100, stats.health + 40);
        UpdateMoodState();
    }
    
    // React to player approach
    public void ReactToPlayer(bool isApproaching)
    {
        if (isApproaching && currentMood == PetMood.Happy)
        {
            PlayHappyAnimation();
        }
    }
    
    // Level up the pet
    public void AddExperience(float amount)
    {
        petExperience += amount;
        float experienceNeeded = petLevel * 100;
        
        if (petExperience >= experienceNeeded)
        {
            petLevel++;
            petExperience -= experienceNeeded;
            
            // Improve stats with level up
            stats.hungerDecayRate *= 0.95f;
            stats.moodDecayRate *= 0.95f;
            stats.energyDecayRate *= 0.95f;
            stats.cleanlinessDecayRate *= 0.95f;
            
            // Play level up effect
            if (emotionParticles != null)
            {
                var main = emotionParticles.main;
                main.startColor = Color.magenta;
                emotionParticles.Play();
            }
            
            Debug.Log($"{petName} leveled up to level {petLevel}!");
        }
    }
}