using System.Collections.Generic;
using UnityEngine;

namespace PuppergeistAccess
{
    /// <summary>
    /// Central localization for the accessibility mod.
    /// Automatically detects game language.
    /// </summary>
    public static class Loc
    {
        #region Fields

        private static bool _initialized = false;
        private static string _currentLang = "en";

        // Dictionaries for each supported language
        private static readonly Dictionary<string, string> _english = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> _chinese = new Dictionary<string, string>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes localization. Call once at mod startup.
        /// </summary>
        public static void Initialize()
        {
            InitializeStrings();
            RefreshLanguage();
            _initialized = true;
        }

        /// <summary>
        /// Updates language based on game setting.
        /// Call when player changes language.
        /// </summary>
        public static void RefreshLanguage()
        {
            string gameLang = GetGameLanguage();

            // Map game language to supported languages
            switch (gameLang.ToLower())
            {
                case "zh":
                case "zh-cn":
                case "chinese":
                case "chinesesimplified":
                    _currentLang = "zh";
                    break;
                default:
                    _currentLang = "en";
                    break;
            }
        }

        /// <summary>
        /// Gets a localized string.
        /// </summary>
        public static string Get(string key)
        {
            if (!_initialized) Initialize();

            var dict = GetCurrentDictionary();

            // Try current language
            if (dict.TryGetValue(key, out string value))
                return value;

            // Fallback: English
            if (_english.TryGetValue(key, out string engValue))
                return engValue;

            // Last fallback: Key itself (helps with debugging)
            return key;
        }

        /// <summary>
        /// Gets a localized string with placeholders.
        /// Uses {0}, {1}, {2} etc. as placeholders.
        /// </summary>
        public static string Get(string key, params object[] args)
        {
            string template = Get(key);
            try
            {
                return string.Format(template, args);
            }
            catch
            {
                return template;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Reads the current game language.
        /// </summary>
        private static string GetGameLanguage()
        {
            // Try Unity's Application.systemLanguage first
            try
            {
                SystemLanguage lang = Application.systemLanguage;
                return lang.ToString();
            }
            catch
            {
                // Fallback to English if detection fails
                return "en";
            }
        }

        private static Dictionary<string, string> GetCurrentDictionary()
        {
            switch (_currentLang)
            {
                case "zh": return _chinese;
                default: return _english;
            }
        }

        /// <summary>
        /// Helper method: Adds a string in all languages.
        /// </summary>
        private static void Add(string key, string english, string chinese)
        {
            _english[key] = english;
            _chinese[key] = chinese;
        }

        /// <summary>
        /// Define all translations here.
        /// </summary>
        private static void InitializeStrings()
        {
            // Core mod messages
            Add("mod_loaded", "PuppergeistAccess mod loaded", "PuppergeistAccess mod loaded");
            Add("help_title", "PuppergeistAccess Help", "PuppergeistAccess Help");
            Add("debug_enabled", "Debug mode enabled", "Debug mode enabled");
            Add("debug_disabled", "Debug mode disabled", "Debug mode disabled");

            // Dialogue
            Add("dialogue_options", "Dialogue options", "Dialogue options");
            Add("option", "Option", "Option");

            // Menu navigation
            Add("pause_menu_opened", "Pause menu opened", "Pause menu opened");
            Add("pause_menu_closed", "Pause menu closed", "Pause menu closed");

            // Control types
            Add("control_button", "button", "button");
            Add("control_slider", "slider", "slider");
            Add("control_dropdown", "dropdown", "dropdown");
            Add("control_toggle", "toggle", "toggle");

            // Control states
            Add("toggle_on", "on", "on");
            Add("toggle_off", "off", "off");
        }

        #endregion
    }
}
