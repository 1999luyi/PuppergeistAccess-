using UnityEngine;
using UnityEngine.EventSystems;

namespace PuppergeistAccess
{
    /// <summary>
    /// Debug tool to monitor EventSystem state.
    /// </summary>
    public static class EventSystemDebugger
    {
        private static bool _enabled = false;
        private static GameObject _lastReportedObject = null;

        public static bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        public static void Update()
        {
            if (!_enabled) return;

            try
            {
                // Log any arrow key presses
                if (Input.GetKeyDown(KeyCode.UpArrow))
                    DebugLogger.Log("EventSystemDebugger: UpArrow pressed");
                if (Input.GetKeyDown(KeyCode.DownArrow))
                    DebugLogger.Log("EventSystemDebugger: DownArrow pressed");
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                    DebugLogger.Log("EventSystemDebugger: LeftArrow pressed");
                if (Input.GetKeyDown(KeyCode.RightArrow))
                    DebugLogger.Log("EventSystemDebugger: RightArrow pressed");
                if (Input.GetKeyDown(KeyCode.Tab))
                    DebugLogger.Log("EventSystemDebugger: Tab pressed");

                if (EventSystem.current == null)
                {
                    if (_lastReportedObject != null)
                    {
                        DebugLogger.Log("EventSystemDebugger: EventSystem.current is NULL");
                        _lastReportedObject = null;
                    }
                    return;
                }

                GameObject selected = EventSystem.current.currentSelectedGameObject;

                if (selected != _lastReportedObject)
                {
                    _lastReportedObject = selected;

                    if (selected == null)
                    {
                        DebugLogger.Log("EventSystemDebugger: No object selected (currentSelectedGameObject is NULL)");
                    }
                    else
                    {
                        DebugLogger.Log($"EventSystemDebugger: Selected object: {selected.name}, Active: {selected.activeInHierarchy}, Path: {GetGameObjectPath(selected)}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                DebugLogger.Log($"EventSystemDebugger: Error - {ex.Message}");
            }
        }

        private static string GetGameObjectPath(GameObject obj)
        {
            if (obj == null) return "";

            string path = obj.name;
            Transform parent = obj.transform.parent;

            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }

            return path;
        }
    }
}
