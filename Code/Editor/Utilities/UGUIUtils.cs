using System;
using UnityEditor;
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

        public static void DrawLockableButton(string label, Action callback, bool shouldDisableButton, params GUILayoutOption[] options)
        {
            EditorGUI.BeginDisabledGroup(shouldDisableButton);
            DrawButton(label, callback, options);
            EditorGUI.EndDisabledGroup();
        }
    }
}