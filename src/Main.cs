using MelonLoader;
using UnityEngine;
using HarmonyLib;

namespace PuppergeistAccess
{
    /// <summary>
    /// Main entry point for PuppergeistAccess mod.
    /// </summary>
    public class Main : MelonMod
    {
        private static HarmonyLib.Harmony _harmony;

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("PuppergeistAccess initializing...");

            try
            {
                // Initialize Tolk for screen reader support
                LoggerInstance.Msg("Loading Tolk library...");
                bool tolkLoaded = false;
                try
                {
                    tolkLoaded = Tolk.Tolk_Load();
                }
                catch (System.Exception ex)
                {
                    LoggerInstance.Warning($"Exception loading Tolk: {ex.Message}");
                }

                if (!tolkLoaded)
                {
                    LoggerInstance.Warning("Failed to load Tolk library - it may already be loaded by the game");
                    // Continue anyway - the game might have its own Tolk instance
                }
                else
                {
                    LoggerInstance.Msg("Detecting screen reader...");
                    string screenReaderName = Tolk.DetectScreenReader();
                    if (string.IsNullOrEmpty(screenReaderName))
                    {
                        LoggerInstance.Warning("No screen reader detected");
                    }
                    else
                    {
                        LoggerInstance.Msg($"Screen reader detected: {screenReaderName}");
                    }
                }

                // Initialize localization
                LoggerInstance.Msg("Initializing localization...");
                Loc.Initialize();

                // Initialize Harmony
                LoggerInstance.Msg("Initializing Harmony...");
                _harmony = new HarmonyLib.Harmony("com.puppergeist.access");

                // Initialize handlers
                LoggerInstance.Msg("Initializing DialogueHandler...");
                DialogueHandler.Initialize(_harmony);

                LoggerInstance.Msg("Initializing MenuHandler...");
                MenuHandler.Initialize(_harmony);

                LoggerInstance.Msg("Initializing InteractionHandler...");
                InteractionHandler.Initialize(_harmony);

                LoggerInstance.Msg("Initializing ImageHandler...");
                ImageHandler.Initialize();

                // Announce mod loaded
                LoggerInstance.Msg("Announcing mod loaded...");
                ScreenReader.Speak(Loc.Get("mod_loaded"), true);

                LoggerInstance.Msg("PuppergeistAccess initialized successfully");
            }
            catch (System.Exception ex)
            {
                LoggerInstance.Error($"Failed to initialize: {ex.Message}");
                LoggerInstance.Error($"Stack trace: {ex.StackTrace}");
            }
        }

        public override void OnUpdate()
        {
            InputHandler.Update();
            MenuHandler.Update();
            ImageHandler.Update();
            DialogueOptionsHandler.Update();
            DialogueHandler.Update();
            EventSystemDebugger.Update();
            InteractionHandler.Update();
        }

        public override void OnDeinitializeMelon()
        {
            Tolk.Tolk_Unload();
            LoggerInstance.Msg("PuppergeistAccess unloaded");
        }
    }
}
