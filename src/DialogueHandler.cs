using HarmonyLib;
using System.Collections;
using System.Reflection;

namespace PuppergeistAccess
{
    /// <summary>
    /// Handles dialogue announcements by patching MinimalLineView and MinimalOptionsView.
    /// </summary>
    public static class DialogueHandler
    {
        private static bool _enabled = true;
        private static string _lastAnnouncedLine = "";
        private static object _currentDialogueLine = null;
        private static bool _rKeyPressed = false;

        /// <summary>
        /// Enables or disables dialogue announcements.
        /// </summary>
        public static bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        /// <summary>
        /// Checks for R key press to repeat current dialogue. Call from Update.
        /// </summary>
        public static void Update()
        {
            // R key: Repeat current dialogue
            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.R))
            {
                if (!_rKeyPressed)
                {
                    _rKeyPressed = true;
                    RepeatCurrentDialogue();
                }
            }
            else if (UnityEngine.Input.GetKeyUp(UnityEngine.KeyCode.R))
            {
                _rKeyPressed = false;
            }
        }

        /// <summary>
        /// Re-reads and announces the current dialogue line from screen.
        /// </summary>
        private static void RepeatCurrentDialogue()
        {
            if (_currentDialogueLine == null)
            {
                DebugLogger.Log("DialogueHandler: No current dialogue to repeat");
                return;
            }

            try
            {
                // Get character name and text WITHOUT character name
                var characterNameProp = AccessTools.Property(_currentDialogueLine.GetType(), "CharacterName");
                var textWithoutCharNameProp = AccessTools.Property(_currentDialogueLine.GetType(), "TextWithoutCharacterName");

                if (textWithoutCharNameProp == null)
                {
                    DebugLogger.Log("DialogueHandler: Could not find TextWithoutCharacterName property");
                    return;
                }

                string characterName = characterNameProp?.GetValue(_currentDialogueLine) as string;
                var textObj = textWithoutCharNameProp.GetValue(_currentDialogueLine);

                if (textObj == null)
                {
                    DebugLogger.Log("DialogueHandler: TextWithoutCharacterName object is null");
                    return;
                }

                // Text is a Yarn.Markup.MarkupParseResult, get its Text FIELD
                var textField = AccessTools.Field(textObj.GetType(), "Text");
                if (textField == null)
                {
                    DebugLogger.Log($"DialogueHandler: Could not find Text field on {textObj.GetType().Name}");
                    return;
                }

                string text = textField.GetValue(textObj) as string;

                if (string.IsNullOrEmpty(text))
                {
                    DebugLogger.Log("DialogueHandler: Text is empty");
                    return;
                }

                // Remove rich text tags
                text = System.Text.RegularExpressions.Regex.Replace(text, @"<[^>]+>", "");
                text = text.Trim();

                if (string.IsNullOrEmpty(text))
                {
                    DebugLogger.Log("DialogueHandler: Text is empty after removing tags");
                    return;
                }

                // Skip minigame control data
                if (!string.IsNullOrEmpty(characterName) &&
                    (characterName == "Pop" || characterName.StartsWith("Spell")))
                {
                    DebugLogger.Log($"DialogueHandler: Skipping minigame control data: {characterName}");
                    return;
                }

                // Build announcement
                string announcement;
                if (!string.IsNullOrEmpty(characterName))
                {
                    announcement = $"{characterName}: {text}";
                }
                else
                {
                    announcement = text;
                }

                // Always announce when R is pressed (don't check for duplicates)
                ScreenReader.Speak(announcement, true);
                DebugLogger.Log($"DialogueHandler: Repeated - {announcement}");
            }
            catch (System.Exception ex)
            {
                MelonLoader.MelonLogger.Error($"DialogueHandler: Error repeating dialogue: {ex.Message}");
            }
        }

        /// <summary>
        /// Applies Harmony patches for dialogue handling.
        /// </summary>
        public static void Initialize(HarmonyLib.Harmony harmony)
        {
            MelonLoader.MelonLogger.Msg("DialogueHandler: Applying patches...");

            try
            {
                // Patch GameManager.RunLine (the main dialogue handler)
                var runLineMethod = AccessTools.Method("GameManager:RunLine");
                if (runLineMethod != null)
                {
                    var postfix = AccessTools.Method(typeof(DialogueHandler), nameof(RunLine_Postfix));
                    harmony.Patch(runLineMethod, postfix: new HarmonyMethod(postfix));
                    MelonLoader.MelonLogger.Msg("DialogueHandler: Patched GameManager.RunLine");
                }
                else
                {
                    MelonLoader.MelonLogger.Warning("DialogueHandler: Could not find GameManager.RunLine");
                }

                // Also patch CutsceneDialogue.RunLine for cutscenes
                var cutsceneRunLineMethod = AccessTools.Method("CutsceneDialogue:RunLine");
                if (cutsceneRunLineMethod != null)
                {
                    var postfix = AccessTools.Method(typeof(DialogueHandler), nameof(RunLine_Postfix));
                    harmony.Patch(cutsceneRunLineMethod, postfix: new HarmonyMethod(postfix));
                    MelonLoader.MelonLogger.Msg("DialogueHandler: Patched CutsceneDialogue.RunLine");
                }

                // Also patch CharacterDialogue.RunLine
                var charRunLineMethod = AccessTools.Method("CharacterDialogue:RunLine");
                if (charRunLineMethod != null)
                {
                    var postfix = AccessTools.Method(typeof(DialogueHandler), nameof(RunLine_Postfix));
                    harmony.Patch(charRunLineMethod, postfix: new HarmonyMethod(postfix));
                    MelonLoader.MelonLogger.Msg("DialogueHandler: Patched CharacterDialogue.RunLine");
                }

                // Patch SpecialDialogueLine.Init for special dialogue screens
                var specialInitMethod = AccessTools.Method("SpecialDialogueLine:Init");
                if (specialInitMethod != null)
                {
                    var postfix = AccessTools.Method(typeof(DialogueHandler), nameof(SpecialDialogueInit_Postfix));
                    harmony.Patch(specialInitMethod, postfix: new HarmonyMethod(postfix));
                    MelonLoader.MelonLogger.Msg("DialogueHandler: Patched SpecialDialogueLine.Init");
                }

                // Patch GameManager.RunOptions
                var runOptionsMethod = AccessTools.Method("GameManager:RunOptions");
                if (runOptionsMethod != null)
                {
                    var postfix = AccessTools.Method(typeof(DialogueHandler), nameof(RunOptions_Postfix));
                    harmony.Patch(runOptionsMethod, postfix: new HarmonyMethod(postfix));
                    MelonLoader.MelonLogger.Msg("DialogueHandler: Patched GameManager.RunOptions");
                }
                else
                {
                    MelonLoader.MelonLogger.Warning("DialogueHandler: Could not find GameManager.RunOptions");
                }

                // Patch MinigameDialogue.ShowScoreScreen for "Your Results" announcement
                var showScoreScreenMethod = AccessTools.Method("MinigameDialogue:ShowScoreScreen");
                if (showScoreScreenMethod != null)
                {
                    var postfix = AccessTools.Method(typeof(DialogueHandler), nameof(ShowScoreScreen_Postfix));
                    harmony.Patch(showScoreScreenMethod, postfix: new HarmonyMethod(postfix));
                    MelonLoader.MelonLogger.Msg("DialogueHandler: Patched MinigameDialogue.ShowScoreScreen");
                }
            }
            catch (System.Exception ex)
            {
                MelonLoader.MelonLogger.Error($"DialogueHandler: Error applying patches: {ex.Message}");
                MelonLoader.MelonLogger.Error($"Stack trace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Postfix for MinimalLineView.RunLine - announces dialogue lines.
        /// </summary>
        private static void RunLine_Postfix(object dialogueLine)
        {
            if (!_enabled) return;
            if (dialogueLine == null) return;

            // Store current dialogue line for R key repeat functionality
            _currentDialogueLine = dialogueLine;

            try
            {
                // Get character name and text WITHOUT character name
                var characterNameProp = AccessTools.Property(dialogueLine.GetType(), "CharacterName");
                var textWithoutCharNameProp = AccessTools.Property(dialogueLine.GetType(), "TextWithoutCharacterName");

                if (textWithoutCharNameProp == null)
                {
                    MelonLoader.MelonLogger.Warning("DialogueHandler: Could not find TextWithoutCharacterName property");
                    return;
                }

                string characterName = characterNameProp?.GetValue(dialogueLine) as string;
                var textObj = textWithoutCharNameProp.GetValue(dialogueLine);

                if (textObj == null)
                {
                    MelonLoader.MelonLogger.Warning("DialogueHandler: TextWithoutCharacterName object is null");
                    return;
                }

                // Text is a Yarn.Markup.MarkupParseResult, get its Text FIELD (not property!)
                var textField = AccessTools.Field(textObj.GetType(), "Text");
                if (textField == null)
                {
                    MelonLoader.MelonLogger.Warning($"DialogueHandler: Could not find Text field on {textObj.GetType().Name}");
                    return;
                }

                string text = textField.GetValue(textObj) as string;

                if (string.IsNullOrEmpty(text))
                {
                    DebugLogger.Log("DialogueHandler: Text is empty, skipping");
                    return;
                }

                // Remove rich text tags (e.g., <size=1.5em>, <color=red>, <br>, etc.)
                text = System.Text.RegularExpressions.Regex.Replace(text, @"<[^>]+>", "");
                text = text.Trim();

                if (string.IsNullOrEmpty(text))
                {
                    DebugLogger.Log("DialogueHandler: Text is empty after removing tags, skipping");
                    return;
                }

                // FILTER: Skip minigame control data (CharacterName = "Pop", "SpellA", "SpellB", etc.)
                // These are used by MinigameDialogue to control rhythm, not actual dialogue
                if (!string.IsNullOrEmpty(characterName) &&
                    (characterName == "Pop" || characterName.StartsWith("Spell")))
                {
                    DebugLogger.Log($"DialogueHandler: Skipping minigame control data: {characterName}: {text}");
                    return;
                }

                // Build announcement
                string announcement;
                if (!string.IsNullOrEmpty(characterName))
                {
                    announcement = $"{characterName}: {text}";
                }
                else
                {
                    announcement = text;
                }

                // Avoid announcing the same line twice
                if (announcement != _lastAnnouncedLine)
                {
                    _lastAnnouncedLine = announcement;
                    ScreenReader.Speak(announcement, true);
                    DebugLogger.Log($"DialogueHandler: Announced - {announcement}");
                }
                else
                {
                    DebugLogger.Log($"DialogueHandler: Skipped duplicate - {announcement}");
                }
            }
            catch (System.Exception ex)
            {
                MelonLoader.MelonLogger.Error($"DialogueHandler: Error in RunLine_Postfix: {ex.Message}");
                MelonLoader.MelonLogger.Error($"Stack trace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Postfix for SpecialDialogueLine.Init - announces special dialogue screens.
        /// </summary>
        private static void SpecialDialogueInit_Postfix(object __result)
        {
            if (!_enabled) return;
            if (__result == null) return;

            try
            {
                DebugLogger.Log("DialogueHandler: SpecialDialogueInit_Postfix called");

                // __result is the instantiated SpecialDialogueLine GameObject
                // We need to find TextMeshProUGUI components in it
                var getComponentsMethod = AccessTools.Method(__result.GetType(), "GetComponentsInChildren",
                    generics: new[] { typeof(TMPro.TextMeshProUGUI) });

                if (getComponentsMethod == null)
                {
                    MelonLoader.MelonLogger.Warning("DialogueHandler: Could not find GetComponentsInChildren method");
                    return;
                }

                var textComponents = getComponentsMethod.Invoke(__result, new object[] { true }) as TMPro.TextMeshProUGUI[];

                if (textComponents == null || textComponents.Length == 0)
                {
                    DebugLogger.Log("DialogueHandler: No TextMeshProUGUI components found in SpecialDialogueLine");
                    return;
                }

                // Collect all text from the components
                string fullText = "";
                foreach (var textComp in textComponents)
                {
                    if (textComp != null && !string.IsNullOrEmpty(textComp.text))
                    {
                        if (fullText.Length > 0)
                            fullText += " ";
                        fullText += textComp.text;
                    }
                }

                if (!string.IsNullOrEmpty(fullText))
                {
                    // Don't use duplicate check for special dialogues as they might be important
                    ScreenReader.Speak(fullText, true);
                    DebugLogger.Log($"DialogueHandler: Announced special dialogue - {fullText}");
                }
            }
            catch (System.Exception ex)
            {
                MelonLoader.MelonLogger.Error($"DialogueHandler: Error in SpecialDialogueInit_Postfix: {ex.Message}");
                MelonLoader.MelonLogger.Error($"Stack trace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Postfix for GameManager.RunOptions - announces dialogue choices.
        /// </summary>
        private static void RunOptions_Postfix(object[] dialogueOptions)
        {
            if (!_enabled) return;
            if (dialogueOptions == null || dialogueOptions.Length == 0) return;

            try
            {
                MelonLoader.MelonLogger.Msg("DialogueHandler: RunOptions_Postfix called");

                // Start a coroutine to wait for UI update
                MelonLoader.MelonCoroutines.Start(AnnounceOptionsWithDelay());
            }
            catch (System.Exception ex)
            {
                MelonLoader.MelonLogger.Error($"DialogueHandler: Error in RunOptions_Postfix: {ex.Message}");
                MelonLoader.MelonLogger.Error($"Stack trace: {ex.StackTrace}");
            }
        }

        private static IEnumerator AnnounceOptionsWithDelay()
        {
            // Wait for UI to update
            yield return new UnityEngine.WaitForSeconds(0.1f);

            string announcement = Loc.Get("dialogue_options");

            // Get the currently highlighted option
            string highlightedOption = DialogueOptionsHandler.GetCurrentHighlightedOption();
            if (!string.IsNullOrEmpty(highlightedOption))
            {
                announcement += ": " + highlightedOption;
            }

            ScreenReader.Speak(announcement, true);
            DebugLogger.Log($"DialogueHandler: Announced options - {announcement}");
        }

        /// <summary>
        /// Postfix for MinigameDialogue.ShowScoreScreen - announces results screen.
        /// </summary>
        private static void ShowScoreScreen_Postfix(object __instance)
        {
            if (!_enabled) return;

            try
            {
                // Get resultsObj, resultsTextObjs, and scoreRankTitles from the instance
                var resultsObjField = AccessTools.Field(__instance.GetType(), "resultsObj");
                var resultsTextObjsField = AccessTools.Field(__instance.GetType(), "resultsTextObjs");
                var scoreRankTitlesField = AccessTools.Field(__instance.GetType(), "scoreRankTitles");
                var percentageField = AccessTools.Field(__instance.GetType(), "percentage");

                if (resultsObjField != null && resultsTextObjsField != null && scoreRankTitlesField != null && percentageField != null)
                {
                    var resultsObj = resultsObjField.GetValue(__instance) as UnityEngine.GameObject;
                    var resultsTextObjs = resultsTextObjsField.GetValue(__instance) as UnityEngine.GameObject[];
                    var scoreRankTitles = scoreRankTitlesField.GetValue(__instance) as UnityEngine.GameObject[];
                    var percentage = (float)percentageField.GetValue(__instance);

                    // Start coroutine to announce results
                    MelonLoader.MelonCoroutines.Start(AnnounceResultsScreen(resultsObj, resultsTextObjs, scoreRankTitles, percentage));
                }
            }
            catch (System.Exception ex)
            {
                MelonLoader.MelonLogger.Error($"DialogueHandler: Error in ShowScoreScreen_Postfix: {ex.Message}");
            }
        }

        private static IEnumerator AnnounceResultsScreen(UnityEngine.GameObject resultsObj, UnityEngine.GameObject[] resultsTextObjs, UnityEngine.GameObject[] scoreRankTitles, float percentage)
        {
            // Wait a moment for the UI to be fully set up
            yield return new UnityEngine.WaitForSeconds(0.2f);

            // Announce the main results object text
            if (resultsObj != null)
            {
                try
                {
                    var tmpText = resultsObj.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                    if (tmpText != null && !string.IsNullOrEmpty(tmpText.text))
                    {
                        ScreenReader.Speak(tmpText.text, true);
                        DebugLogger.Log($"DialogueHandler: Announced results - {tmpText.text}");
                    }
                }
                catch (System.Exception ex)
                {
                    DebugLogger.Log($"DialogueHandler: Error announcing results object: {ex.Message}");
                }
            }

            // Announce each result text object as they appear
            if (resultsTextObjs != null)
            {
                foreach (var textObj in resultsTextObjs)
                {
                    yield return new UnityEngine.WaitForSeconds(0.5f); // Match the game's animation timing

                    if (textObj != null && textObj.activeInHierarchy)
                    {
                        try
                        {
                            var tmpText = textObj.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                            if (tmpText != null && !string.IsNullOrEmpty(tmpText.text))
                            {
                                ScreenReader.Speak(tmpText.text, true);
                                DebugLogger.Log($"DialogueHandler: Announced result text - {tmpText.text}");
                            }
                        }
                        catch (System.Exception ex)
                        {
                            DebugLogger.Log($"DialogueHandler: Error announcing result text: {ex.Message}");
                        }
                    }
                }
            }

            // Announce the score rank title based on percentage
            // Wait a bit for the rank title to appear (game shows it after resultsTextObjs)
            yield return new UnityEngine.WaitForSeconds(0.1f);

            if (scoreRankTitles != null && scoreRankTitles.Length >= 3)
            {
                try
                {
                    UnityEngine.GameObject rankTitle = null;

                    // Determine which rank title to announce based on percentage
                    // Match the game's logic from AnimateScoreScreen
                    if (percentage <= 90f && percentage > 60f)
                    {
                        rankTitle = scoreRankTitles[0];
                    }
                    else if (percentage > 90f)
                    {
                        rankTitle = scoreRankTitles[1];
                    }
                    else if (percentage <= 60f)
                    {
                        rankTitle = scoreRankTitles[2];
                    }

                    if (rankTitle != null)
                    {
                        var tmpText = rankTitle.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                        if (tmpText != null && !string.IsNullOrEmpty(tmpText.text))
                        {
                            ScreenReader.Speak(tmpText.text, true);
                            DebugLogger.Log($"DialogueHandler: Announced rank title - {tmpText.text}");
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    DebugLogger.Log($"DialogueHandler: Error announcing rank title: {ex.Message}");
                }
            }
        }
    }
}
