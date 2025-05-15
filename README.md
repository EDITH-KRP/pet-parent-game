# ANIMORA – Virtual Pet Simulation Game

Animora is a vibrant, immersive virtual pet simulation game that blends fantasy and realism in a colorful open-world city. The game features a clean, whimsical, cartoon-style aesthetic inspired by games like Animal Crossing and Nintendogs, with an emphasis on joyful, immersive environments.

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

## How to Run
1. Open the project in Unity Hub (Unity 2022.3 LTS or newer recommended)
2. Set `MainMenu` as the starting scene
3. Ensure TextMeshPro packages are imported
4. Link prefabs to their respective script references in the Inspector
5. Configure NavMesh for the environments
6. Hit Play!

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