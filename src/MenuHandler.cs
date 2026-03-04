using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PuppergeistAccess
{
    /// <summary>
    /// Handles menu navigation announcements (pause menu, settings).
    /// </summary>
    public static class MenuHandler
    {
        private static bool _enabled = true;
        private static GameObject _lastSelectedObject = null;
        private static float _lastAnnouncementTime = 0f;
        private static float _lastSliderValue = -1f;
        private static int _lastDropdownValue = -1;

        /// <summary>
        /// Enables or disables menu announcements.
        /// </summary>
        public static bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        /// <summary>
        /// Updates menu navigation. Call from Main.OnUpdate().
        /// </summary>
        public static void Update()
        {
            if (!_enabled) return;

            // Get currently selected UI object
            GameObject selected = EventSystem.current?.currentSelectedGameObject;

            // If selection changed, announce it immediately
            if (selected != null && selected != _lastSelectedObject)
            {
                DebugLogger.Log($"MenuHandler: Selection changed from '{(_lastSelectedObject != null ? _lastSelectedObject.name : "null")}' to '{selected.name}'");

                _lastSelectedObject = selected;
                _lastAnnouncementTime = Time.time;
                _lastSliderValue = -1f; // Reset slider value tracking
                _lastDropdownValue = -1; // Reset dropdown value tracking
                AnnounceUIElement(selected);
            }
            // If same object is selected, check for value changes (sliders, dropdowns)
            else if (selected != null && selected == _lastSelectedObject)
            {
                CheckValueChanges(selected);
            }
        }

        /// <summary>
        /// Announces a UI element (button, slider, dropdown, etc.).
        /// </summary>
        private static void AnnounceUIElement(GameObject obj)
        {
            if (obj == null) return;

            try
            {
                string announcement = "";

                // Try to get text from TextMeshProUGUI
                var tmpText = obj.GetComponentInChildren<TextMeshProUGUI>();
                if (tmpText != null && !string.IsNullOrEmpty(tmpText.text))
                {
                    announcement = tmpText.text;
                }
                else
                {
                    // Try regular Text component
                    var text = obj.GetComponentInChildren<Text>();
                    if (text != null && !string.IsNullOrEmpty(text.text))
                    {
                        announcement = text.text;
                    }
                    else
                    {
                        // Use object name as fallback
                        announcement = obj.name;
                    }
                }

                // Add control type information
                string controlType = GetControlType(obj);
                if (!string.IsNullOrEmpty(controlType))
                {
                    announcement += ", " + controlType;
                }

                // Add current value for interactive controls
                string value = GetControlValue(obj);
                if (!string.IsNullOrEmpty(value))
                {
                    announcement += ", " + value;
                }

                ScreenReader.Speak(announcement, true);
                DebugLogger.Log($"MenuHandler: Announced - {announcement}");
            }
            catch (System.Exception ex)
            {
                DebugLogger.Log($"MenuHandler: Error announcing element: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks for value changes in interactive controls (sliders, dropdowns).
        /// </summary>
        private static void CheckValueChanges(GameObject obj)
        {
            if (obj == null) return;

            try
            {
                // Check slider value changes
                var slider = obj.GetComponent<Slider>();
                if (slider != null)
                {
                    float currentValue = slider.value;
                    if (_lastSliderValue >= 0 && !Mathf.Approximately(currentValue, _lastSliderValue))
                    {
                        _lastSliderValue = currentValue;
                        int percentage = Mathf.RoundToInt(currentValue * 100f);
                        ScreenReader.Speak($"{percentage}%", true);
                        DebugLogger.Log($"MenuHandler: Slider value changed to {percentage}%");
                    }
                    else if (_lastSliderValue < 0)
                    {
                        _lastSliderValue = currentValue;
                    }
                    return;
                }

                // Check dropdown value changes
                var tmpDropdown = obj.GetComponent<TMP_Dropdown>();
                if (tmpDropdown != null)
                {
                    int currentValue = tmpDropdown.value;
                    if (_lastDropdownValue >= 0 && currentValue != _lastDropdownValue)
                    {
                        _lastDropdownValue = currentValue;
                        string optionText = tmpDropdown.options[currentValue].text;
                        ScreenReader.Speak(optionText, true);
                        DebugLogger.Log($"MenuHandler: Dropdown value changed to {optionText}");
                    }
                    else if (_lastDropdownValue < 0)
                    {
                        _lastDropdownValue = currentValue;
                    }
                    return;
                }

                var dropdown = obj.GetComponent<Dropdown>();
                if (dropdown != null)
                {
                    int currentValue = dropdown.value;
                    if (_lastDropdownValue >= 0 && currentValue != _lastDropdownValue)
                    {
                        _lastDropdownValue = currentValue;
                        string optionText = dropdown.options[currentValue].text;
                        ScreenReader.Speak(optionText, true);
                        DebugLogger.Log($"MenuHandler: Dropdown value changed to {optionText}");
                    }
                    else if (_lastDropdownValue < 0)
                    {
                        _lastDropdownValue = currentValue;
                    }
                    return;
                }
            }
            catch (System.Exception ex)
            {
                DebugLogger.Log($"MenuHandler: Error checking value changes: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the control type (button, slider, dropdown, etc.).
        /// </summary>
        private static string GetControlType(GameObject obj)
        {
            if (obj.GetComponent<Button>() != null)
                return Loc.Get("control_button");

            if (obj.GetComponent<Slider>() != null)
                return Loc.Get("control_slider");

            if (obj.GetComponent<TMP_Dropdown>() != null || obj.GetComponent<Dropdown>() != null)
                return Loc.Get("control_dropdown");

            if (obj.GetComponent<Toggle>() != null)
                return Loc.Get("control_toggle");

            return "";
        }

        /// <summary>
        /// Gets the current value of a control.
        /// </summary>
        private static string GetControlValue(GameObject obj)
        {
            try
            {
                // Slider
                var slider = obj.GetComponent<Slider>();
                if (slider != null)
                {
                    int percentage = Mathf.RoundToInt(slider.value * 100f);
                    return $"{percentage}%";
                }

                // TMP_Dropdown
                var tmpDropdown = obj.GetComponent<TMP_Dropdown>();
                if (tmpDropdown != null)
                {
                    return tmpDropdown.options[tmpDropdown.value].text;
                }

                // Regular Dropdown
                var dropdown = obj.GetComponent<Dropdown>();
                if (dropdown != null)
                {
                    return dropdown.options[dropdown.value].text;
                }

                // Toggle
                var toggle = obj.GetComponent<Toggle>();
                if (toggle != null)
                {
                    return toggle.isOn ? Loc.Get("toggle_on") : Loc.Get("toggle_off");
                }
            }
            catch (System.Exception ex)
            {
                DebugLogger.Log($"MenuHandler: Error getting control value: {ex.Message}");
            }

            return "";
        }

        /// <summary>
        /// Applies Harmony patches for menu handling.
        /// </summary>
        public static void Initialize(HarmonyLib.Harmony harmony)
        {
            MelonLoader.MelonLogger.Msg("MenuHandler: Applying patches...");

            try
            {
                // Patch PauseMenu.Paused setter to announce menu open/close
                var pausedProperty = AccessTools.Property(typeof(PauseMenu), "Paused");
                if (pausedProperty != null)
                {
                    var setter = pausedProperty.GetSetMethod();
                    if (setter != null)
                    {
                        var postfix = AccessTools.Method(typeof(MenuHandler), nameof(PausedSetter_Postfix));
                        harmony.Patch(setter, postfix: new HarmonyMethod(postfix));
                        MelonLoader.MelonLogger.Msg("MenuHandler: Patched PauseMenu.Paused setter");
                    }
                }
            }
            catch (System.Exception ex)
            {
                MelonLoader.MelonLogger.Error($"MenuHandler: Error applying patches: {ex.Message}");
                MelonLoader.MelonLogger.Error($"Stack trace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Postfix for PauseMenu.Paused setter - announces menu open/close.
        /// </summary>
        private static void PausedSetter_Postfix(bool value)
        {
            if (!_enabled) return;

            try
            {
                if (value)
                {
                    ScreenReader.Speak(Loc.Get("pause_menu_opened"), true);
                    _lastSelectedObject = null; // Reset so first item gets announced
                }
                else
                {
                    ScreenReader.Speak(Loc.Get("pause_menu_closed"), true);
                }
            }
            catch (System.Exception ex)
            {
                DebugLogger.Log($"MenuHandler: Error in PausedSetter_Postfix: {ex.Message}");
            }
        }
    }
}
