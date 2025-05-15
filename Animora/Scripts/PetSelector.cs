using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class PetCategory
{
    public string categoryName;
    public List<GameObject> petPrefabs = new List<GameObject>();
}

public class PetSelector : MonoBehaviour
{
    [Header("Pet Prefabs")]
    public List<GameObject> petPrefabs = new List<GameObject>();
    public List<PetCategory> petCategories = new List<PetCategory>();
    
    [Header("UI Elements")]
    public Transform petCardContainer;
    public GameObject petCardPrefab;
    public TMP_Dropdown categoryDropdown;
    public Button unlockButton;
    public TextMeshProUGUI currencyText;
    public TextMeshProUGUI petDescriptionText;
    
    [Header("Pet Preview")]
    public Transform previewSpawnPoint;
    public Camera previewCamera;
    public Light previewLight;
    
    // Private variables
    private GameObject previewPet;
    private int selectedPetIndex = -1;
    private List<GameObject> filteredPets = new List<GameObject>();
    
    private void Start()
    {
        // Initialize category dropdown
        if (categoryDropdown != null)
        {
            categoryDropdown.ClearOptions();
            
            List<string> options = new List<string>();
            options.Add("All Pets");
            
            foreach (PetCategory category in petCategories)
            {
                options.Add(category.categoryName);
            }
            
            categoryDropdown.AddOptions(options);
            categoryDropdown.onValueChanged.AddListener(OnCategoryChanged);
        }
        
        // Initialize unlock button
        if (unlockButton != null)
        {
            unlockButton.onClick.AddListener(OnUnlockButtonClicked);
            unlockButton.gameObject.SetActive(false); // Hide initially
        }
        
        // Initialize with all pets
        filteredPets = new List<GameObject>(petPrefabs);
        PopulatePetCards();
        
        // Update currency display
        UpdateCurrencyDisplay();
    }
    
    private void OnDestroy()
    {
        // Clean up preview pet
        if (previewPet != null)
        {
            Destroy(previewPet);
        }
    }
    
    private void PopulatePetCards()
    {
        // Clear existing cards
        foreach (Transform child in petCardContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Create cards for each pet in filtered list
        for (int i = 0; i < filteredPets.Count; i++)
        {
            GameObject petPrefab = filteredPets[i];
            Pet pet = petPrefab.GetComponent<Pet>();
            
            if (pet != null)
            {
                GameObject cardObj = Instantiate(petCardPrefab, petCardContainer);
                PetCard petCard = cardObj.GetComponent<PetCard>();
                
                if (petCard != null)
                {
                    // Set up pet card
                    petCard.SetPetInfo(pet.petName, pet.petType.ToString(), i);
                    
                    // Check if pet is unlocked
                    bool isUnlocked = GameManager.Instance.playerData.unlockedPets.Contains(pet.petName);
                    petCard.SetUnlocked(isUnlocked);
                    
                    // Add click listener
                    int index = i; // Create a copy for the closure
                    petCard.SetClickCallback(() => OnPetCardClicked(index));
                }
            }
        }
    }
    
    private void OnCategoryChanged(int categoryIndex)
    {
        // Filter pets by category
        if (categoryIndex == 0)
        {
            // "All Pets" category
            filteredPets = new List<GameObject>(petPrefabs);
        }
        else
        {
            // Specific category
            int actualCategoryIndex = categoryIndex - 1; // Adjust for "All Pets" option
            
            if (actualCategoryIndex >= 0 && actualCategoryIndex < petCategories.Count)
            {
                filteredPets = petCategories[actualCategoryIndex].petPrefabs;
            }
        }
        
        // Repopulate pet cards
        PopulatePetCards();
        
        // Clear selection
        selectedPetIndex = -1;
        UpdatePetPreview();
    }
    
    private void OnPetCardClicked(int index)
    {
        if (index >= 0 && index < filteredPets.Count)
        {
            selectedPetIndex = index;
            UpdatePetPreview();
            
            // Get pet info
            GameObject petPrefab = filteredPets[index];
            Pet pet = petPrefab.GetComponent<Pet>();
            
            if (pet != null)
            {
                // Update description
                if (petDescriptionText != null)
                {
                    petDescriptionText.text = $"Name: {pet.petName}\nType: {pet.petType}\n\n" +
                                             $"This {pet.petType} is waiting for a loving home! " +
                                             $"Each pet has unique needs and personalities.";
                }
                
                // Check if pet is unlocked
                bool isUnlocked = GameManager.Instance.playerData.unlockedPets.Contains(pet.petName);
                
                // Show/hide unlock button
                if (unlockButton != null)
                {
                    unlockButton.gameObject.SetActive(!isUnlocked);
                    
                    // Update button text based on currency
                    TextMeshProUGUI buttonText = unlockButton.GetComponentInChildren<TextMeshProUGUI>();
                    if (buttonText != null)
                    {
                        int unlockCost = GetPetUnlockCost(pet);
                        buttonText.text = $"Unlock ({unlockCost})";
                        
                        // Disable button if not enough currency
                        unlockButton.interactable = GameManager.Instance.playerData.currency >= unlockCost;
                    }
                }
            }
        }
    }
    
    private void UpdatePetPreview()
    {
        // Clean up previous preview
        if (previewPet != null)
        {
            Destroy(previewPet);
            previewPet = null;
        }
        
        // Create new preview if a pet is selected
        if (selectedPetIndex >= 0 && selectedPetIndex < filteredPets.Count)
        {
            GameObject petPrefab = filteredPets[selectedPetIndex];
            
            if (previewSpawnPoint != null)
            {
                previewPet = Instantiate(petPrefab, previewSpawnPoint.position, previewSpawnPoint.rotation);
                
                // Disable AI behavior for preview
                Pet pet = previewPet.GetComponent<Pet>();
                if (pet != null)
                {
                    // Disable NavMeshAgent if present
                    var navAgent = previewPet.GetComponent<UnityEngine.AI.NavMeshAgent>();
                    if (navAgent != null)
                    {
                        navAgent.enabled = false;
                    }
                    
                    // Set to idle animation
                    var animator = previewPet.GetComponent<Animator>();
                    if (animator != null)
                    {
                        animator.SetInteger("Activity", (int)PetActivity.Idle);
                    }
                }
            }
        }
    }
    
    private void OnUnlockButtonClicked()
    {
        if (selectedPetIndex >= 0 && selectedPetIndex < filteredPets.Count)
        {
            GameObject petPrefab = filteredPets[selectedPetIndex];
            Pet pet = petPrefab.GetComponent<Pet>();
            
            if (pet != null)
            {
                int unlockCost = GetPetUnlockCost(pet);
                
                // Check if player has enough currency
                if (GameManager.Instance.SpendCurrency(unlockCost))
                {
                    // Unlock the pet
                    GameManager.Instance.UnlockPet(pet.petName);
                    
                    // Update UI
                    UpdateCurrencyDisplay();
                    PopulatePetCards();
                    
                    // Hide unlock button
                    if (unlockButton != null)
                    {
                        unlockButton.gameObject.SetActive(false);
                    }
                    
                    // Show notification
                    UIManager uiManager = FindObjectOfType<UIManager>();
                    if (uiManager != null)
                    {
                        uiManager.ShowNotification($"Unlocked {pet.petName}!");
                    }
                }
            }
        }
    }
    
    private int GetPetUnlockCost(Pet pet)
    {
        // Base cost depends on pet type
        int baseCost = 100;
        
        switch (pet.petType)
        {
            case PetType.Dog:
                baseCost = 100;
                break;
            case PetType.Cat:
                baseCost = 100;
                break;
            case PetType.Dragon:
                baseCost = 300;
                break;
            case PetType.Unicorn:
                baseCost = 500;
                break;
            default:
                baseCost = 100;
                break;
        }
        
        return baseCost;
    }
    
    private void UpdateCurrencyDisplay()
    {
        if (currencyText != null)
        {
            currencyText.text = $"{GameManager.Instance.playerData.currency}";
        }
    }
    
    // Public method to select a pet and return to gameplay
    public void SelectPet(int index)
    {
        if (index >= 0 && index < petPrefabs.Count)
        {
            // Spawn the selected pet
            GameManager.Instance.SpawnPet(petPrefabs[index]);
            
            // Return to gameplay UI
            UIManager uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null)
            {
                uiManager.ShowGameplayHUD();
            }
        }
    }
    
    // Public method to select a pet by name
    public void SelectPetByName(string petName)
    {
        for (int i = 0; i < petPrefabs.Count; i++)
        {
            Pet pet = petPrefabs[i].GetComponent<Pet>();
            if (pet != null && pet.petName == petName)
            {
                SelectPet(i);
                return;
            }
        }
    }
}

// Helper class for pet card UI
public class PetCard : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI typeText;
    public Image petIcon;
    public GameObject lockedOverlay;
    public Button cardButton;
    
    private System.Action clickCallback;
    
    private void Awake()
    {
        if (cardButton != null)
        {
            cardButton.onClick.AddListener(() => clickCallback?.Invoke());
        }
    }
    
    public void SetPetInfo(string name, string type, int index)
    {
        if (nameText != null) nameText.text = name;
        if (typeText != null) typeText.text = type;
        
        // Pet icon would be set here if available
    }
    
    public void SetUnlocked(bool isUnlocked)
    {
        if (lockedOverlay != null)
        {
            lockedOverlay.SetActive(!isUnlocked);
        }
    }
    
    public void SetClickCallback(System.Action callback)
    {
        clickCallback = callback;
    }
}