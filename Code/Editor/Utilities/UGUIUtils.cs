using System;
using UnityEngine;

namespace eg_unity_shared_tools.Code.Editor.Utilities
{
    public static class UGUIUtils
    {
        // DrawButton<T>(string label, T callback) => Explore this for methods with parameters
        public static void DrawButton(string label, Action callback, params GUILayoutOption[] options)
        {
            if (GUILayout.Button(label, options))
            {
                callback?.Invoke();
            }
        }
    }
}