using UnityEngine;

namespace PuppergeistAccess
{
    /// <summary>
    /// Handles mod input (F1 for help, F12 for debug toggle).
    /// </summary>
    public static class InputHandler
    {
        private static bool _f1Pressed = false;
        private static bool _f12Pressed = false;
        private static bool _f11Pressed = false;

        /// <summary>
        /// Checks for mod key presses. Call from Update.
        /// </summary>
        public static void Update()
        {
            // F1: Help
            if (Input.GetKeyDown(KeyCode.F1))
            {
                if (!_f1Pressed)
                {
                    _f1Pressed = true;
                    ShowHelp();
                }
            }
            else if (Input.GetKeyUp(KeyCode.F1))
            {
                _f1Pressed = false;
            }

            // F12: Toggle debug mode
            if (Input.GetKeyDown(KeyCode.F12))
            {
                if (!_f12Pressed)
                {
                    _f12Pressed = true;
                    ToggleDebug();
                }
            }
            else if (Input.GetKeyUp(KeyCode.F12))
            {
                _f12Pressed = false;
            }

            // F11: Toggle EventSystem debugger
            if (Input.GetKeyDown(KeyCode.F11))
            {
                if (!_f11Pressed)
                {
                    _f11Pressed = true;
                    ToggleEventSystemDebugger();
                }
            }
            else if (Input.GetKeyUp(KeyCode.F11))
            {
                _f11Pressed = false;
            }
        }

        private static void ShowHelp()
        {
            string help = Loc.Get("help_title") + ". " +
                         "F1: Help. " +
                         "F12: Toggle debug mode. " +
                         "R: Repeat current dialogue.";
            ScreenReader.Speak(help, true);
        }

        private static void ToggleDebug()
        {
            DebugLogger.DebugEnabled = !DebugLogger.DebugEnabled;
        }

        private static void ToggleEventSystemDebugger()
        {
            EventSystemDebugger.Enabled = !EventSystemDebugger.Enabled;
            string status = EventSystemDebugger.Enabled ? "EventSystem debugger enabled" : "EventSystem debugger disabled";
            ScreenReader.Speak(status, true);
        }
    }
}
