# PuppergeistAccess

An accessibility mod for Puppergeist that adds comprehensive screen reader support, making the game fully playable for blind and visually impaired players.

## Features

- **Dialogue Announcements**: Automatically reads all dialogue text with character names
- **Menu Navigation**: Full keyboard navigation with screen reader announcements for all menu elements
- **Image Descriptions**: Configurable descriptions for important visual elements
- **Dialogue Options**: Navigate and select dialogue choices with arrow keys or number keys
- **Interaction Announcements**: Notifies when approaching interactive objects
- **Minigame Results**: Announces scores and rankings after minigames
- **Repeat Function**: Press R to re-read the current dialogue

## Installation

### Requirements
- Puppergeist (Demo or Full Game)
- [MelonLoader v0.7.2](https://github.com/LavaGang/MelonLoader/releases) or later
- Windows 10/11 with a screen reader (NVDA, JAWS, or Narrator)

### Steps
1. Install MelonLoader for Puppergeist
2. Download the latest release from the [Releases](../../releases) page
3. Extract `PuppergeistAccess.dll` to `[Game Directory]/Mods/`
4. Extract `ImageDescriptions.json` to `[Game Directory]/Mods/`
5. Launch the game - you should hear "PuppergeistAccess mod loaded"

## Usage

### Keyboard Controls
- **F1**: Show help message
- **F12**: Toggle debug mode
- **R**: Repeat current dialogue (re-reads from screen)
- **Arrow Keys / W/S**: Navigate dialogue options
- **Number Keys (1-9)**: Quick select dialogue options
- **Tab / Arrow Keys**: Navigate menus

### Customizing Image Descriptions
Edit `ImageDescriptions.json` in the Mods folder to add descriptions for any sprite:

```json
{
  "descriptions": {
    "sprite_name": "Your description here"
  }
}
```

Unknown sprites are logged in debug mode (F12) for easy identification.

## Building from Source

### Requirements
- .NET SDK 4.7.2 or later
- Visual Studio 2019+ or VS Code with C# extension

### Build Steps
```bash
dotnet build PuppergeistAccess.csproj -c Release
```

The compiled DLL will be in `bin/Release/net472/PuppergeistAccess.dll`

## Technical Details

- **Mod Loader**: MelonLoader v0.7.2
- **Target Framework**: .NET Framework 4.7.2
- **Screen Reader Library**: Tolk (supports NVDA, JAWS, SAPI, System Access, Window-Eyes)
- **Patching**: Harmony for runtime method patching

## Documentation

See the `docs/` folder for detailed guides:
- `ACCESSIBILITY_MODDING_GUIDE.md`: Patterns and best practices
- `technical-reference.md`: MelonLoader, Harmony, and Tolk reference
- `game-api.md`: Documented game methods and key bindings
- Additional guides for localization, state management, and more

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.

## License

This mod is provided as-is for accessibility purposes. Puppergeist is property of its respective owners.

## Credits

- Developed with assistance from Claude (Anthropic)
- Tolk library by Davy Kager
- MelonLoader by LavaGang
- Harmony by pardeike

## Support

If you encounter issues or have suggestions, please open an issue on GitHub.
