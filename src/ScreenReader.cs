namespace PuppergeistAccess
{
    /// <summary>
    /// High-level screen reader interface.
    /// </summary>
    public static class ScreenReader
    {
        /// <summary>
        /// Speaks text through the screen reader.
        /// </summary>
        /// <param name="text">Text to speak</param>
        /// <param name="interrupt">If true, interrupts current speech</param>
        public static void Speak(string text, bool interrupt = false)
        {
            if (string.IsNullOrEmpty(text))
                return;

            Tolk.Tolk_Speak(text, interrupt);
        }

        /// <summary>
        /// Silences the screen reader.
        /// </summary>
        public static void Silence()
        {
            Tolk.Tolk_Silence();
        }

        /// <summary>
        /// Checks if screen reader speech is available.
        /// </summary>
        public static bool HasSpeech()
        {
            return Tolk.Tolk_HasSpeech();
        }
    }
}
