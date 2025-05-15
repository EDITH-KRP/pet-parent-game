using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Main Menu")]
    public GameObject mainMenuPanel;
    public Button playButton;
    public Button petSelectButton;
    public Button settingsButton;
    public Button quitButton;
    
    [Header("Pet Selection")]
    public GameObject petSelectionPanel;
    public Transform petButtonContainer;
    public GameObject petButtonPrefab;
    
    [Header("Gameplay HUD")]
    public GameObject gameplayHUDPanel;
    public Slider hungerSlider;
    public Slider moodSlider;
    public Slider energySlider;
    public Slider cleanlinessSlider;
    public Slider healthSlider;
    public TextMeshProUGUI petNameText;
    public TextMeshProUGUI petLevelText;
    public TextMeshProUGUI currencyText;
    public TextMeshProUGUI locationText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI weatherText;
    
    [Header("Quick Actions")]
    public Button feedButton;
    public Button playButton_HUD;
    public Button cleanButton;
    public Button sleepButton;
    public Button healButton;
    
    [Header("Location Selection")]
    public GameObject locationPanel;
    public Transform locationButtonContainer;
    public GameObject locationButtonPrefab;
    
    [Header("Shop")]
    public GameObject shopPanel;
    public Transform shopItemContainer;
    public GameObject shopItemPrefab;
    public TextMeshProUGUI shopCurrencyText;
    
    [Header("Settings")]
    public GameObject settingsPanel;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Toggle fullscreenToggle;
    public TMP_Dropdown qualityDropdown;
    
    [Header("Notifications")]
    public GameObject notificationPanel;
    public TextMeshProUGUI notificationText;
    public float notificationDuration = 3f;
    
    [Header("Loading")]
    public GameObject loadingPanel;
    public Slider loadingProgressBar;
    
    [Header("Visual Effects")]
    public ParticleSystem currencyParticles;
    public ParticleSystem levelUpParticles;
    
    // Private variables
    private Coroutine notificationCoroutine;
    private Dictionary<string, Button> quickActionButtons = new Dictionary<string, Button>();
    
    private void Awake()
    {
        // Initialize quick action buttons dictionary
        quickActionButtons["Feed"] = feedButton;
        quickActionButtons["Play"] = playButton_HUD;
        quickActionButtons["Clean"] = cleanButton;
        quickActionButtons["Sleep"] = sleepButton;
        quickActionButtons["Heal"] = healButton;
        
        // Add listeners to buttons
        if (playButton != null) playButton.onClick.AddListener(OnPlayGameClicked);
        if (petSelectButton != null) petSelectButton.onClick.AddListener(OnPetSelectClicked);
        if (settingsButton != null) settingsButton.onClick.AddListener(OnSettingsClicked);
        if (quitButton != null) quitButton.onClick.AddListener(OnQuitClicked);
        
        if (feedButton != null) feedButton.onClick.AddListener(OnFeedClicked);
        if (playButton_HUD != null) playButton_HUD.onClick.AddListener(OnPlayClicked);
        if (cleanButton != null) cleanButton.onClick.AddListener(OnCleanClicked);
        if (sleepButton != null) sleepButton.onClick.AddListener(OnSleepClicked);
        if (healButton != null) healButton.onClick.AddListener(OnHealClicked);
    }
    
    private void Start()
    {
        // Show main menu by default
        ShowMainMenu();
        
        // Hide notification panel initially
        if (notificationPanel != null) notificationPanel.SetActive(false);
        
        // Hide loading panel initially
        if (loadingPanel != null) loadingPanel.SetActive(false);
    }
    
    private void Update()
    {
        // Update HUD if gameplay panel is active
        if (gameplayHUDPanel != null && gameplayHUDPanel.activeSelf)
        {
            UpdateHUD();
        }
    }
    
    private void UpdateHUD()
    {
        if (GameManager.Instance.currentPet != null)
        {
            Pet pet = GameManager.Instance.currentPet.GetComponent<Pet>();
            if (pet != null)
            {
                // Update pet stats
                if (hungerSlider != null) hungerSlider.value = pet.stats.hunger / 100f;
                if (moodSlider != null) moodSlider.value = pet.stats.mood / 100f;
                if (energySlider != null) energySlider.value = pet.stats.energy / 100f;
                if (cleanlinessSlider != null) cleanlinessSlider.value = pet.stats.cleanliness / 100f;
                if (healthSlider != null) healthSlider.value = pet.stats.health / 100f;
                
                // Update pet info
                if (petNameText != null) petNameText.text = pet.petName;
                if (petLevelText != null) petLevelText.text = $"Level {pet.petLevel}";
            }
        }
        
        // Update player info
        if (currencyText != null) currencyText.text = $"{GameManager.Instance.playerData.currency}";
        
        // Update location info
        if (locationText != null) locationText.text = GameManager.Instance.currentLocation;
        
        // Update time and weather
        if (timeText != null) timeText.text = GameManager.Instance.currentTimeOfDay.ToString();
        if (weatherText != null) weatherText.text = GameManager.Instance.currentWeather.ToString();
    }
    
    // Main menu functions
    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        petSelectionPanel.SetActive(false);
        gameplayHUDPanel.SetActive(false);
        locationPanel.SetActive(false);
        shopPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }
    
    private void OnPlayGameClicked()
    {
        // If player has at least one pet, go to gameplay
        if (GameManager.Instance.playerData.unlockedPets.Count > 0)
        {
            // Spawn the first unlocked pet if none is active
            if (GameManager.Instance.currentPet == null)
            {
                GameManager.Instance.SpawnPetByName(GameManager.Instance.playerData.unlockedPets[0]);
            }
            
            ShowGameplayHUD();
        }
        else
        {
            // If no pets unlocked, go to pet selection
            ShowPetSelection();
        }
    }
    
    private void OnPetSelectClicked()
    {
        ShowPetSelection();
    }
    
    private void OnSettingsClicked()
    {
        ShowSettings();
    }
    
    private void OnQuitClicked()
    {
        // Save game before quitting
        GameManager.Instance.SaveGame();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    // Pet selection functions
    public void ShowPetSelection()
    {
        mainMenuPanel.SetActive(false);
        petSelectionPanel.SetActive(true);
        gameplayHUDPanel.SetActive(false);
        locationPanel.SetActive(false);
        shopPanel.SetActive(false);
        settingsPanel.SetActive(false);
        
        // Populate pet buttons
        PopulatePetButtons();
    }
    
    private void PopulatePetButtons()
    {
        // Clear existing buttons
        foreach (Transform child in petButtonContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create buttons for each unlocked pet
        foreach (string petName in GameManager.Instance.playerData.unlockedPets)
        {
            GameObject buttonObj = Instantiate(petButtonPrefab, petButtonContainer);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            
            if (buttonText != null)
            {
                buttonText.text = petName;
            }
            
            if (button != null)
            {
                string petNameCopy = petName; // Create a copy for the closure
                button.onClick.AddListener(() => OnPetSelected(petNameCopy));
            }
        }
    }
    
    private void OnPetSelected(string petName)
    {
        GameManager.Instance.SpawnPetByName(petName);
        ShowGameplayHUD();
    }
    
    // Gameplay HUD functions
    public void ShowGameplayHUD()
    {
        mainMenuPanel.SetActive(false);
        petSelectionPanel.SetActive(false);
        gameplayHUDPanel.SetActive(true);
        locationPanel.SetActive(false);
        shopPanel.SetActive(false);
        settingsPanel.SetActive(false);
        
        // Update HUD immediately
        UpdateHUD();
    }
    
    // Pet interaction functions
    public void OnFeedClicked()
    {
        if (GameManager.Instance.currentPet != null)
        {
            Pet pet = GameManager.Instance.currentPet.GetComponent<Pet>();
            if (pet != null)
            {
                pet.Feed();
                ShowNotification($"Feeding {pet.petName}");
            }
        }
    }
    
    public void OnPlayClicked()
    {
        if (GameManager.Instance.currentPet != null)
        {
            Pet pet = GameManager.Instance.currentPet.GetComponent<Pet>();
            if (pet != null)
            {
                pet.Play();
                ShowNotification($"Playing with {pet.petName}");
            }
        }
    }
    
    public void OnCleanClicked()
    {
        if (GameManager.Instance.currentPet != null)
        {
            Pet pet = GameManager.Instance.currentPet.GetComponent<Pet>();
            if (pet != null)
            {
                pet.Clean();
                ShowNotification($"Cleaning {pet.petName}");
            }
        }
    }
    
    public void OnSleepClicked()
    {
        if (GameManager.Instance.currentPet != null)
        {
            Pet pet = GameManager.Instance.currentPet.GetComponent<Pet>();
            if (pet != null)
            {
                pet.Sleep();
                ShowNotification($"{pet.petName} is going to sleep");
            }
        }
    }
    
    public void OnHealClicked()
    {
        if (GameManager.Instance.currentPet != null)
        {
            Pet pet = GameManager.Instance.currentPet.GetComponent<Pet>();
            if (pet != null)
            {
                pet.Heal();
                ShowNotification($"Healing {pet.petName}");
            }
        }
    }
    
    // Location functions
    public void ShowLocationPanel()
    {
        mainMenuPanel.SetActive(false);
        petSelectionPanel.SetActive(false);
        gameplayHUDPanel.SetActive(true); // Keep HUD visible
        locationPanel.SetActive(true);
        shopPanel.SetActive(false);
        settingsPanel.SetActive(false);
        
        // Populate location buttons
        PopulateLocationButtons();
    }
    
    private void PopulateLocationButtons()
    {
        // Clear existing buttons
        foreach (Transform child in locationButtonContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create buttons for each location
        foreach (GameLocation location in GameManager.Instance.gameLocations)
        {
            if (location.isUnlocked)
            {
                GameObject buttonObj = Instantiate(locationButtonPrefab, locationButtonContainer);
                Button button = buttonObj.GetComponent<Button>();
                TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                Image buttonImage = buttonObj.GetComponent<Image>();
                
                if (buttonText != null)
                {
                    buttonText.text = location.locationName;
                }
                
                if (buttonImage != null && location.locationIcon != null)
                {
                    buttonImage.sprite = location.locationIcon;
                }
                
                if (button != null)
                {
                    string locationNameCopy = location.locationName; // Create a copy for the closure
                    button.onClick.AddListener(() => OnLocationSelected(locationNameCopy));
                }
            }
        }
    }
    
    private void OnLocationSelected(string locationName)
    {
        // Show loading screen
        ShowLoadingScreen();
        
        // Change location
        GameManager.Instance.ChangeLocation(locationName);
        
        // Hide location panel
        locationPanel.SetActive(false);
    }
    
    // Shop functions
    public void ShowShop()
    {
        mainMenuPanel.SetActive(false);
        petSelectionPanel.SetActive(false);
        gameplayHUDPanel.SetActive(true); // Keep HUD visible
        locationPanel.SetActive(false);
        shopPanel.SetActive(true);
        settingsPanel.SetActive(false);
        
        // Update shop currency display
        if (shopCurrencyText != null)
        {
            shopCurrencyText.text = $"{GameManager.Instance.playerData.currency}";
        }
        
        // Populate shop items (would be implemented with actual shop items)
    }
    
    // Settings functions
    public void ShowSettings()
    {
        mainMenuPanel.SetActive(false);
        petSelectionPanel.SetActive(false);
        gameplayHUDPanel.SetActive(false);
        locationPanel.SetActive(false);
        shopPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }
    
    public void OnMusicVolumeChanged(float value)
    {
        // Set music volume
        // AudioManager.Instance.SetMusicVolume(value);
    }
    
    public void OnSFXVolumeChanged(float value)
    {
        // Set SFX volume
        // AudioManager.Instance.SetSFXVolume(value);
    }
    
    public void OnFullscreenToggled(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
    
    public void OnQualityChanged(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    
    // Notification system
    public void ShowNotification(string message)
    {
        if (notificationPanel != null && notificationText != null)
        {
            // Stop any existing notification
            if (notificationCoroutine != null)
            {
                StopCoroutine(notificationCoroutine);
            }
            
            // Show new notification
            notificationText.text = message;
            notificationPanel.SetActive(true);
            
            // Start notification timer
            notificationCoroutine = StartCoroutine(HideNotificationAfterDelay());
        }
    }
    
    private IEnumerator HideNotificationAfterDelay()
    {
        yield return new WaitForSeconds(notificationDuration);
        notificationPanel.SetActive(false);
        notificationCoroutine = null;
    }
    
    // Loading screen
    public void ShowLoadingScreen()
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
            
            // Reset progress bar
            if (loadingProgressBar != null)
            {
                loadingProgressBar.value = 0f;
            }
            
            // Start loading animation
            StartCoroutine(AnimateLoadingBar());
        }
    }
    
    private IEnumerator AnimateLoadingBar()
    {
        if (loadingProgressBar != null)
        {
            float progress = 0f;
            
            while (progress < 1f)
            {
                progress += Time.deltaTime;
                loadingProgressBar.value = progress;
                yield return null;
            }
            
            // Hide loading panel when done
            loadingPanel.SetActive(false);
        }
    }
    
    // Scene management
    public void ChangeScene(string sceneName)
    {
        ShowLoadingScreen();
        SceneManager.LoadScene(sceneName);
    }
    
    // Visual effects
    public void PlayCurrencyParticles()
    {
        if (currencyParticles != null)
        {
            currencyParticles.Play();
        }
    }
    
    public void PlayLevelUpParticles()
    {
        if (levelUpParticles != null)
        {
            levelUpParticles.Play();
        }
    }
}