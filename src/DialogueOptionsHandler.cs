using TMPro;
using UnityEngine;

namespace PuppergeistAccess
{
    /// <summary>
    /// Handles dialogue option navigation announcements.
    /// </summary>
    public static class DialogueOptionsHandler
    {
        private static bool _enabled = true;
        private static string _lastAnnouncedOption = "";
        private static int _delayFrames = 0;
        private static int _frameDelay = 2; // Wait 2 frames for game to update

        public static bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        /// <summary>
        /// Updates option navigation monitoring. Call from Main.OnUpdate().
        /// </summary>
        public static void Update()
        {
            if (!_enabled)
                return;

            try
            {
                // Check for arrow key presses
                bool upPressed = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
                bool downPressed = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);
                bool numberPressed = false;

                // Check for number keys 1-9
                for (int i = 0; i < 9; i++)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                    {
                        numberPressed = true;
                        break;
                    }
                }

                if (upPressed || downPressed || numberPressed)
                {
                    _delayFrames = _frameDelay;
                }

                // After delay, announce the highlighted option
                if (_delayFrames > 0)
                {
                    _delayFrames--;
                    if (_delayFrames == 0)
                    {
                        AnnounceHighlightedOption();
                    }
                }
            }
            catch (System.Exception ex)
            {
                DebugLogger.Log($"DialogueOptionsHandler: Error in Update: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the currently highlighted option text without announcing it.
        /// </summary>
        public static string GetCurrentHighlightedOption()
        {
            try
            {
                // Find all TextMeshProUGUI components
                var allTexts = Object.FindObjectsOfType<TextMeshProUGUI>();

                foreach (var textComp in allTexts)
                {
                    if (textComp == null || !textComp.gameObject.activeInHierarchy)
                        continue;

                    string text = textComp.text;
                    if (string.IsNullOrEmpty(text))
                        continue;

                    // Check if this text contains the white color tag (highlighted option)
                    if (text.Contains("<color=white>") && text.Contains("</color>"))
                    {
                        // Extract the highlighted text
                        int startIndex = text.IndexOf("<color=white>") + "<color=white>".Length;
                        int endIndex = text.IndexOf("</color>", startIndex);

                        if (endIndex > startIndex)
                        {
                            string highlightedText = text.Substring(startIndex, endIndex - startIndex);

                            // Remove any remaining tags
                            highlightedText = System.Text.RegularExpressions.Regex.Replace(highlightedText, "<.*?>", "");

                            if (!string.IsNullOrEmpty(highlightedText))
                            {
                                return highlightedText;
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                DebugLogger.Log($"DialogueOptionsHandler: Error getting highlighted option: {ex.Message}");
            }

            return "";
        }

        private static void AnnounceHighlightedOption()
        {
            string highlightedText = GetCurrentHighlightedOption();
            if (!string.IsNullOrEmpty(highlightedText))
            {
                _lastAnnouncedOption = highlightedText;
                ScreenReader.Speak(highlightedText, true);
                DebugLogger.Log($"DialogueOptionsHandler: Announced option - {highlightedText}");
            }
        }
    }
}
