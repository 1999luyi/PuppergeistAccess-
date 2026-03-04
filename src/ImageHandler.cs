using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace PuppergeistAccess
{
    /// <summary>
    /// Handles image descriptions from JSON configuration.
    /// </summary>
    public static class ImageHandler
    {
        private static bool _enabled = true;
        private static Dictionary<string, string> _descriptions = new Dictionary<string, string>();
        private static HashSet<string> _announcedSpriteNames = new HashSet<string>();
        private static float _lastCheckTime = 0f;
        private static float _checkInterval = 0.2f; // Reduced from 0.5s to catch short-lived images
        private static bool _initialized = false;

        public static bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        /// <summary>
        /// Initializes and loads image descriptions from JSON.
        /// </summary>
        public static void Initialize()
        {
            if (_initialized) return;

            try
            {
                string modsPath = System.IO.Path.Combine(Application.dataPath, "..");
                modsPath = System.IO.Path.Combine(modsPath, "Mods");
                string jsonPath = System.IO.Path.Combine(modsPath, "ImageDescriptions.json");

                if (!File.Exists(jsonPath))
                {
                    MelonLoader.MelonLogger.Warning($"ImageHandler: ImageDescriptions.json not found at {jsonPath}");
                    _initialized = true;
                    return;
                }

                string jsonContent = File.ReadAllText(jsonPath);
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ImageDescriptionsData>(jsonContent);

                if (data != null && data.descriptions != null)
                {
                    _descriptions = data.descriptions;
                    MelonLoader.MelonLogger.Msg($"ImageHandler: Loaded {_descriptions.Count} image descriptions");
                }

                _initialized = true;
            }
            catch (System.Exception ex)
            {
                MelonLoader.MelonLogger.Error($"ImageHandler: Error loading descriptions: {ex.Message}");
                _initialized = true;
            }
        }

        /// <summary>
        /// Updates image monitoring. Call from Main.OnUpdate().
        /// </summary>
        public static void Update()
        {
            if (!_enabled || !_initialized) return;

            // Throttle checks
            if (Time.time - _lastCheckTime < _checkInterval)
                return;

            _lastCheckTime = Time.time;

            try
            {
                // Find all active Image components (UI)
                var allImages = Object.FindObjectsOfType<Image>();

                foreach (var image in allImages)
                {
                    if (image == null || !image.gameObject.activeInHierarchy)
                        continue;

                    // Get sprite name
                    if (image.sprite == null)
                        continue;

                    string spriteName = image.sprite.name;

                    // Check if we have a description for this sprite
                    if (_descriptions.ContainsKey(spriteName))
                    {
                        // Only announce if not already announced
                        if (!_announcedSpriteNames.Contains(spriteName))
                        {
                            string description = _descriptions[spriteName];
                            ScreenReader.Speak(description, true);
                            _announcedSpriteNames.Add(spriteName);
                            DebugLogger.Log($"ImageHandler: Announced description for sprite '{spriteName}' (UI Image)");
                        }
                    }
                    else
                    {
                        // Log unknown sprites (but don't spam - only log once per sprite name)
                        if (!_announcedSpriteNames.Contains(spriteName))
                        {
                            DebugLogger.Log($"ImageHandler: Found sprite '{spriteName}' (no description available) (UI Image)");
                            // Don't add to _announcedSpriteNames so we keep checking in case description is added
                        }
                    }
                }

                // Also check SpriteRenderer components (non-UI sprites)
                var allSpriteRenderers = Object.FindObjectsOfType<SpriteRenderer>();

                foreach (var spriteRenderer in allSpriteRenderers)
                {
                    if (spriteRenderer == null || !spriteRenderer.gameObject.activeInHierarchy)
                        continue;

                    // Get sprite name
                    if (spriteRenderer.sprite == null)
                        continue;

                    string spriteName = spriteRenderer.sprite.name;

                    // Check if we have a description for this sprite
                    if (_descriptions.ContainsKey(spriteName))
                    {
                        // Only announce if not already announced
                        if (!_announcedSpriteNames.Contains(spriteName))
                        {
                            string description = _descriptions[spriteName];
                            ScreenReader.Speak(description, true);
                            _announcedSpriteNames.Add(spriteName);
                            DebugLogger.Log($"ImageHandler: Announced description for sprite '{spriteName}' (SpriteRenderer)");
                        }
                    }
                    else
                    {
                        // Log unknown sprites
                        if (!_announcedSpriteNames.Contains(spriteName))
                        {
                            DebugLogger.Log($"ImageHandler: Found sprite '{spriteName}' (no description available) (SpriteRenderer)");
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                DebugLogger.Log($"ImageHandler: Error in Update: {ex.Message}");
            }
        }

        [System.Serializable]
        private class ImageDescriptionsData
        {
            public Dictionary<string, string> descriptions;
        }
    }
}
