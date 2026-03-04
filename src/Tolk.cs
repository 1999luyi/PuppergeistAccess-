using System;
using System.Runtime.InteropServices;

namespace PuppergeistAccess
{
    /// <summary>
    /// Tolk library wrapper for screen reader support.
    /// </summary>
    public static class Tolk
    {
        [DllImport("Tolk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Tolk_Load();

        [DllImport("Tolk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Tolk_Unload();

        [DllImport("Tolk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Tolk_DetectScreenReader();

        [DllImport("Tolk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Tolk_HasSpeech();

        [DllImport("Tolk.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern bool Tolk_Output(string text, bool interrupt);

        [DllImport("Tolk.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern bool Tolk_Speak(string text, bool interrupt);

        [DllImport("Tolk.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Tolk_Silence();

        /// <summary>
        /// Safe wrapper for Tolk_DetectScreenReader that handles string marshaling.
        /// </summary>
        public static string DetectScreenReader()
        {
            try
            {
                IntPtr ptr = Tolk_DetectScreenReader();
                if (ptr == IntPtr.Zero)
                    return string.Empty;
                return Marshal.PtrToStringUni(ptr);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
