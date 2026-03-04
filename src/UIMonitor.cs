using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PuppergeistAccess
{
    /// <summary>
    /// Monitors UI text changes and announces new text.
    /// </summary>
    public static class UIMonitor
    {
        private static bool _enabled = true;
        private static Dictionary<TextMeshProUGUI, string> _lastTexts = new Dictionary<TextMeshProUGUI, string>();
        private static float _lastCheckTime = 0f;
        private static float _checkInterval = 0.5f; // Check every 0.5 seconds

        public static bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        /// <summary>
        /// Updates UI monitoring. Call from Main.OnUpdate().
        /// </summary>
        public static void Update()
        {
            if (!_enabled) return;

            // Throttle checks
            if (Time.time - _lastCheckTime < _checkInterval)
                return;

            _lastCheckTime = Time.time;

            try
            {
                // Find all active TextMeshProUGUI components
                var allTexts = Object.FindObjectsOfType<TextMeshProUGUI>();

                foreach (var textComp in allTexts)
                {
                    if (textComp == null || !textComp.gameObject.activeInHierarchy)
                        continue;

                    string currentText = textComp.text;

                    // Skip empty text
                    if (string.IsNullOrWhiteSpace(currentText))
                        continue;

                    // Check if this is new or changed text
                    if (!_lastTexts.ContainsKey(textComp))
                    {
                        // New text component
                        _lastTexts[textComp] = currentText;

                        // Only announce if it's substantial (more than just a button label)
                        if (currentText.Length > 20 && !IsUILabel(textComp))
                        {
                            ScreenReader.Speak(currentText, false);
                            DebugLogger.Log($"UIMonitor: Announced new text - {currentText.Substring(0, System.Math.Min(50, currentText.Length))}...");
                        }
                    }
                    else if (_lastTexts[textComp] != currentText)
                    {
                        // Text changed
                        _lastTexts[textComp] = currentText;

                        if (currentText.Length > 20 && !IsUILabel(textComp))
                        {
                            ScreenReader.Speak(currentText, false);
                            DebugLogger.Log($"UIMonitor: Announced changed text - {currentText.Substring(0, System.Math.Min(50, currentText.Length))}...");
                        }
                    }
                }

                // Clean up destroyed components
                var toRemove = new List<TextMeshProUGUI>();
                foreach (var key in _lastTexts.Keys)
                {
                    if (key == null)
                        toRemove.Add(key);
                }
                foreach (var key in toRemove)
                {
                    _lastTexts.Remove(key);
                }
            }
            catch (System.Exception ex)
            {
                DebugLogger.Log($"UIMonitor: Error in Update: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if a text component is likely a UI label (button, menu item, etc.)
        /// </summary>
        private static bool IsUILabel(TextMeshProUGUI textComp)
        {
            // Check parent for Button or other UI components
            var parent = textComp.transform.parent;
            if (parent != null)
            {
                if (parent.GetComponent<UnityEngine.UI.Button>() != null)
                    return true;
                if (parent.GetComponent<UnityEngine.UI.Toggle>() != null)
                    return true;
            }

            return false;
        }
    }
}
