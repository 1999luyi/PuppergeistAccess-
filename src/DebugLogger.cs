using MelonLoader;

namespace PuppergeistAccess
{
    /// <summary>
    /// Debug logging utility. Only active when debug mode is enabled (F12).
    /// Zero overhead when disabled.
    /// </summary>
    public static class DebugLogger
    {
        private static bool _debugEnabled = true; // Default to enabled

        /// <summary>
        /// Enables or disables debug mode.
        /// </summary>
        public static bool DebugEnabled
        {
            get => _debugEnabled;
            set
            {
                _debugEnabled = value;
                string message = value ? Loc.Get("debug_enabled") : Loc.Get("debug_disabled");
                ScreenReader.Speak(message, true);
                MelonLogger.Msg(message);
            }
        }

        /// <summary>
        /// Logs a debug message. Only outputs if debug mode is enabled.
        /// </summary>
        public static void Log(string message)
        {
            if (_debugEnabled)
            {
                MelonLogger.Msg($"[DEBUG] {message}");
            }
        }

        /// <summary>
        /// Logs a debug message with formatting. Only outputs if debug mode is enabled.
        /// </summary>
        public static void Log(string format, params object[] args)
        {
            if (_debugEnabled)
            {
                MelonLogger.Msg($"[DEBUG] {string.Format(format, args)}");
            }
        }
    }
}
