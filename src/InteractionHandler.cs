using HarmonyLib;
using UnityEngine;

namespace PuppergeistAccess
{
    /// <summary>
    /// Handles interaction object announcements.
    /// </summary>
    public static class InteractionHandler
    {
        private static bool _enabled = true;
        private static object _lastInteractable = null;

        public static bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        /// <summary>
        /// Initializes Harmony patches for interaction handling.
        /// </summary>
        public static void Initialize(HarmonyLib.Harmony harmony)
        {
            try
            {
                // Patch Player.Update to detect when currentInteractable changes
                var playerType = AccessTools.TypeByName("Player");
                if (playerType != null)
                {
                    var updateMethod = AccessTools.Method(playerType, "Update");
                    if (updateMethod != null)
                    {
                        var postfix = AccessTools.Method(typeof(InteractionHandler), nameof(PlayerUpdate_Postfix));
                        harmony.Patch(updateMethod, postfix: new HarmonyMethod(postfix));
                        DebugLogger.Log("InteractionHandler: Patched Player.Update");
                    }
                }
            }
            catch (System.Exception ex)
            {
                DebugLogger.Log($"InteractionHandler: Error applying patches: {ex.Message}");
            }
        }

        /// <summary>
        /// Postfix for Player.Update - announces when currentInteractable changes.
        /// </summary>
        private static void PlayerUpdate_Postfix(object __instance)
        {
            if (!_enabled) return;

            try
            {
                // Get currentInteractable
                var currentInteractableField = AccessTools.Field(__instance.GetType(), "currentInteractable");
                if (currentInteractableField == null) return;

                object currentInteractable = currentInteractableField.GetValue(__instance);

                // Only announce if the interactable has changed
                if (currentInteractable != _lastInteractable)
                {
                    _lastInteractable = currentInteractable;

                    if (currentInteractable != null)
                    {
                        AnnounceInteractable(currentInteractable);
                    }
                }
            }
            catch (System.Exception ex)
            {
                DebugLogger.Log($"InteractionHandler: Error in PlayerUpdate_Postfix: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates interaction monitoring. Call from Main.OnUpdate().
        /// </summary>
        public static void Update()
        {
            // No longer needed - all functionality is in the patch
        }

        private static void AnnounceInteractable(object interactable)
        {
            try
            {
                // Get the name or keyword of the interactable
                var keywordProp = AccessTools.Property(interactable.GetType(), "Keyword");
                string keyword = keywordProp?.GetValue(interactable) as string;

                if (string.IsNullOrEmpty(keyword))
                {
                    // Fallback to GameObject name
                    var gameObject = (interactable as Component)?.gameObject;
                    keyword = gameObject?.name;
                }

                if (!string.IsNullOrEmpty(keyword))
                {
                    ScreenReader.Speak(keyword, true);
                    DebugLogger.Log($"InteractionHandler: Announced interactable - {keyword}");
                }
            }
            catch (System.Exception ex)
            {
                DebugLogger.Log($"InteractionHandler: Error announcing interactable: {ex.Message}");
            }
        }
    }
}
