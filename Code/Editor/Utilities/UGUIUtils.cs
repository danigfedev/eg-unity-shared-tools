using System;
using UnityEditor;
using UnityEngine;

namespace eg_unity_shared_tools.Code.Editor.Utilities
{
    public static class UGUIUtils
    {
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

        private static readonly string[] TabStyles = 
        {
            "ButtonLeft",
            "ButtonMid",
            "ButtonRight"
        };
        
        public static int DrawTabs(int selectedTabIndex, params string[] tabNames)
        {
            GUILayout.BeginHorizontal();

            for(int tabIndex =0; tabIndex < tabNames.Length; tabIndex++)
            {
                var style = tabIndex == 0 ? TabStyles[0] : tabIndex == tabNames.Length - 1 ? TabStyles[2] : TabStyles[1];
                
                if (GUILayout.Toggle(selectedTabIndex == tabIndex, tabNames[tabIndex], style))
                {
                    selectedTabIndex = tabIndex;
                }
            }
            
            GUILayout.EndHorizontal();

            return selectedTabIndex;
        }
    }
}