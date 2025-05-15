# ANIMORA – Virtual Pet Simulation Game (Project Scaffold)

This repository contains a scaffold/template for Animora, a vibrant, immersive virtual pet simulation game that blends fantasy and realism in a colorful open-world city. The game features a clean, whimsical, cartoon-style aesthetic inspired by games like Animal Crossing and Nintendogs, with an emphasis on joyful, immersive environments.

**Note:** This is not a complete Unity project. Please follow the setup instructions below to create a proper Unity project using these scaffold files.

## Tech Stack
- Unity 2022+
- C# in Visual Studio Code
- NavMesh for pet AI navigation
- Particle systems for visual effects
- Unity's new UI system with TextMeshPro
- Unity ML-Agents (planned for future AI enhancements)
- Photon Multiplayer (planned for future multiplayer features)
- Firebase backend (planned for future cloud saves)

## Key Features

### 🌎 Open-World Environment
- **Pet Park**: Grassy area with fountains, play zones, and interactive elements
- **Café**: Outdoor seating area where pets can socialize and players can purchase treats
- **Spa & Vet Clinic**: Where pets can be groomed, healed, and pampered
- **Pet Homes**: Customizable interiors where pets live and players can decorate

### 🐾 Pet System
- **Diverse Pet Types**: Both realistic (dogs, cats) and fantasy (dragons, unicorns) pets
- **Comprehensive Stats**: Hunger, mood, energy, cleanliness, and health
- **AI-Driven Behaviors**: Pets play, sleep, and react to player actions autonomously
- **Animation Sets**: Idle, happy, sad, sick, playing, running, and more
- **Pet Leveling**: Pets gain experience and level up, unlocking new abilities

### 🎮 Gameplay Systems
- **Pet Care**: Feed, play with, clean, and heal your pets
- **Customization**: Dress pets in various outfits and accessories
- **Decoration**: Furnish and decorate pet homes with furniture and decorations
- **Quests & Achievements**: Complete tasks to earn rewards and unlock new content
- **Shop System**: Purchase items, furniture, and accessories with in-game currency

### 🎨 UI Design
- **Vibrant Cartoon Style**: Soft gradients and round corners for a friendly feel
- **Main Menu**: "Play", "Pet Select", "Settings" options
- **In-Game HUD**: Shows pet stats and quick actions (feed, play, clean)
- **Shop UI**: Icons for clothes, accessories, furniture with clear categories

### 🌤️ World Effects
- **Day/Night Cycle**: Real-time cycle affecting lighting and pet behaviors
- **Dynamic Weather**: Sunny, rain, snow, fog with visual effects
- **Seasonal Themes**: Halloween, Winterfest, Summer Fun with special decorations

### 💾 Save System
- **Persistent Progress**: Save and load game data
- **Multiple Save Slots**: Support for multiple game saves (planned)
- **Cloud Saves**: Sync progress across devices (planned)

## Core Scripts

### Pet Management
- **Pet.cs**: Core pet behavior, stats, and AI
- **PetSelector.cs**: Handles pet selection and customization

### Game Systems
- **GameManager.cs**: Central game state management
- **UIManager.cs**: Handles all UI interactions and displays
- **WorldManager.cs**: Manages the open-world environment and locations
- **ItemSystem.cs**: Inventory and item management
- **ShopSystem.cs**: In-game store and economy
- **QuestSystem.cs**: Quests and achievements
- **SaveSystem.cs**: Game data persistence

## Project Setup Instructions

### Important Note
This repository is a scaffold/template and does not contain a complete Unity project structure. You'll need to create a new Unity project and import these files into it.

### Setup Steps
1. **Create a new Unity project**:
   - Open Unity Hub
   - Click "New Project"
   - Select Unity 2022.3 LTS or newer
   - Choose "3D" or "3D (URP)" template
   - Name your project "Animora"
   - Set location to a directory of your choice
   - Click "Create Project"

2. **Import the scaffold files**:
   - Navigate to `p:\Animora_Unity_Scaffold\pet-parent-game\Animora` in your file explorer
   - Copy all contents from this folder
   - Paste into your new Unity project's "Assets" folder

3. **Create required folders**:
   - In your Unity project, create the folder structure as outlined in the "Project Structure" section below
   - Organize the imported files into their appropriate folders

4. **Configure your project**:
   - Create and save at least one scene (e.g., MainMenu)
   - Go to File > Build Settings
   - Add your scene(s) to the build
   - Ensure TextMeshPro packages are imported
   - Configure NavMesh for the environments

5. **Build the project**:
   - Go to the top menu and click "Build"
   - Select "Build Windows" to create a standalone executable

6. **Run the game**:
   - Set your starting scene (e.g., MainMenu)
   - Hit Play in the Unity Editor to test

## Project Structure
```
Animora/
├── Scripts/           # C# scripts for game logic
├── Prefabs/           # Reusable game objects
│   ├── Pets/          # Pet prefabs (dogs, cats, fantasy creatures)
│   ├── UI/            # UI element prefabs
│   ├── Environment/   # World object prefabs
│   └── Effects/       # Particle and visual effect prefabs
├── Scenes/            # Unity scenes
│   ├── MainMenu       # Game start menu
│   ├── PetHome        # Player's home base
│   ├── PetPark        # Outdoor play area
│   ├── Cafe           # Social area
│   └── SpaClinic      # Pet care facility
├── Art/               # Visual assets
│   ├── Models/        # 3D models
│   ├── Textures/      # Texture files
│   ├── Animations/    # Animation files
│   └── UI/            # UI graphics
├── Audio/             # Sound assets
│   ├── Music/         # Background music
│   ├── SFX/           # Sound effects
│   └── PetSounds/     # Pet vocalizations
└── Resources/         # Runtime-loaded assets
```

## Future Enhancements
- **VR/AR Support**: Life-sized pet interactions in VR/AR
- **Multiplayer**: Social zones where players can interact with each other's pets
- **Advanced AI**: More complex pet behaviors and personalities
- **Mini-games**: Pet training and skill-based activities
- **Mobile Version**: Adaptation for iOS and Android platforms

## Credits
Developed as a Unity project scaffold for Animora, a virtual pet simulation game.

## License
This project is intended for educational and portfolio purposes.