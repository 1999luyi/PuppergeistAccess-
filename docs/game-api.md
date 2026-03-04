# Puppergeist Game API Documentation

This document contains analyzed information about Puppergeist's internal systems.

## Game Key Bindings (Original)

### Movement
- **A / Left Arrow**: Move left
- **D / Right Arrow**: Move right
- **W / Up Arrow**: Menu navigation up
- **S / Down Arrow**: Menu navigation down

### Interaction
- **Space**:
  - Interact with objects/NPCs
  - Advance dialogue
  - Confirm menu selections
  - Used in minigames
- **2 / Up Arrow**: Pet interaction (when `$can_pet` variable is true)

### UI/Menus
- **Tab**: Open/close inventory
- **Escape**: Pause menu
- **Backtick (`)**: Chat log toggle
- **1-9 Number keys**:
  - Dialogue option selection
  - Minigame spell selection
  - Quick actions

### System
- **F5**: Quick save
- **F6**: Quick load
- **Left Control**: Alternative to Space for dialogue advancement

### Minigame Keys
- **Arrow keys (Up/Down/Left/Right)**: Minigame controls
- **Number keys (1-4+)**: Spell/action selection in minigames
- **Space**: Minigame actions

## Safe Mod Keys

Based on the analysis above, the following keys are SAFE for mod use (not used by the game):

- **F1**: ✓ Safe (used for Help)
- **F2**: ✓ Safe
- **F3**: ✓ Safe
- **F4**: ✓ Safe
- **F7**: ✓ Safe
- **F8**: ✓ Safe
- **F9**: ✓ Safe
- **F10**: ✓ Safe
- **F11**: ✓ Safe (may be fullscreen in some games, test)
- **F12**: ✓ Safe (used for Debug toggle)

**DO NOT USE:**
- F5, F6 (save/load)
- Tab, Escape, Backtick
- Space, Enter, Arrow keys
- Number keys 1-9
- WASD keys
- Left Control

## Input System

### Classes
- **Player.cs**: Handles player movement and interaction input
- **GameManager.cs**: Handles dialogue system input, save/load
- **PauseMenu.cs**: Handles pause menu toggle
- **Inventory.cs**: Handles inventory toggle
- **ChatLog.cs**: Handles chat log toggle
- **MinigameDialogue.cs**: Handles minigame-specific input
- **SpecialDialogueLine.cs**: Handles special dialogue progression

### Input Method
- Uses Unity's legacy Input system (`Input.GetKey`, `Input.GetKeyDown`, `Input.GetKeyUp`)
- No custom input manager detected
- Direct KeyCode checks throughout the codebase

## UI System

### Text Rendering
- **Framework**: TextMeshPro (TMP_Text, TextMeshProUGUI)
- All UI text uses TextMeshPro components

### Dialogue System Classes
- **MinimalLineView**: Displays dialogue lines
  - `lineText` (TextMeshProUGUI): Main dialogue text
  - `characterNameText` (TextMeshProUGUI): Character name display
  - `RunLine(LocalizedLine)`: Called when new dialogue line appears
  - Supports typewriter effect and fade animations

- **MinimalOptionsView**: Displays dialogue choices
  - `lastLineText` (TextMeshProUGUI): Shows last dialogue line
  - `RunOptions(DialogueOption[])`: Called when choices appear
  - Creates OptionView instances for each choice

- **MinimalDialogueRunner**: Main dialogue controller
  - Manages dialogue flow
  - Integrates with Yarn Spinner

### Accessing UI Text
- **Method**: Use Unity's `FindObjectOfType<>()` or `FindObjectsOfType<>()` to locate TextMeshProUGUI components
- **Reflection**: Not needed for basic text access - TextMeshProUGUI.text is public
- **Pattern**: Monitor MinimalLineView.RunLine() and MinimalOptionsView.RunOptions() for dialogue events

### Key UI Components
- CanvasGroup: Used for fade effects and interactivity control
- All dialogue views inherit from DialogueViewBase (Yarn.Unity)

## State Management

### Singleton Pattern
- **GameManager**: Main game singleton
  - `GameManager.Instance`: Static instance accessor
  - Persists across scenes (DontDestroyOnLoad)
  - Controls dialogue system, player, camera

### Key Properties
- `GameManager.Instance.Player`: Current player instance
- `GameManager.Instance.CurrentMode`: Active dialogue view mode
- `GameManager.Instance.runner`: DialogueRunner instance

### Game State
- Uses Yarn Spinner's variable system for game state
- Variables accessible via DialogueRunner
- Save/Load system: F5 (quick save), F6 (quick load)

### Architecture Decision
**Recommendation**: Use Harmony patches to hook into dialogue events
- Patch `MinimalLineView.RunLine()` to announce dialogue text
- Patch `MinimalOptionsView.RunOptions()` to announce choices
- Access GameManager.Instance for game state queries
- No complex state tracking needed - react to game events

## Localization System

### Framework
- **Yarn Spinner Localization**: Game uses Yarn Spinner's built-in localization
- **LineProvider**: Yarn.Unity.LineProviderBehaviour handles text localization
- **LocaleCode**: Current language code accessible via `LineProvider.LocaleCode`

### Implementation
- MinimalDialogueRunner has LineProvider component
- All dialogue text is localized through Yarn's system
- LocalizedLine objects contain translated text

### For Mod Localization
- **Mod uses separate localization**: Our Loc.cs system is independent
- **Language detection**: Use Unity's `Application.systemLanguage` (already implemented in Loc.cs)
- **No conflict**: Mod localization doesn't interfere with game's Yarn localization

## Architecture Decisions

### State Management Strategy
**Decision**: Event-driven architecture using Harmony patches
- **Why**: Game already has well-defined event points (RunLine, RunOptions)
- **How**: Patch key methods to intercept dialogue and UI events
- **Benefits**:
  - No polling needed
  - React immediately to game events
  - Minimal performance impact
  - Clean separation from game code

### Key Patch Points (Tier 2)
- `MinimalLineView.RunLine()`: Announce dialogue text
- `MinimalOptionsView.RunOptions()`: Announce dialogue choices
- `Player.Update()`: Monitor player state (if needed)
- `Inventory` methods: Announce inventory changes (if needed)

### Handler Organization
- **DialogueHandler**: Manages dialogue announcements
- **MenuHandler**: Manages menu navigation (inventory, pause menu)
- **InputHandler**: Already implemented (F1, F12)
- Each handler is independent and can be enabled/disabled

## Game Variables

- `$can_pet`: Boolean variable controlling pet interaction availability

(More to be documented as discovered)
