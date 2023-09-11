using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace eg_unity_shared_tools.Utilities.Editor
{
    public static class UGUIUtils
    {
        public static void HorizontalLayout(bool centerElements, params Action[] drawingMethods)
        {
            GUILayout.BeginHorizontal();
            if(centerElements) GUILayout.FlexibleSpace();
            
            foreach (var drawMethod in drawingMethods)
            {
                drawMethod?.Invoke();
            }
            
            if(centerElements) GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public static void VerticalLayout(bool centerElements, params Action[] drawingMethods)
        {
            GUILayout.BeginVertical();
            if(centerElements) GUILayout.FlexibleSpace();
            
            foreach (var drawMethod in drawingMethods)
            {
                drawMethod?.Invoke();
            }
            
            if(centerElements) GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }
        
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
        
        public static int DrawTabs(int selectedTabIndex, string[] tabNames, int[] disabledTabsIndexes = null)
        {
            GUILayout.BeginHorizontal();

            for(int tabIndex = 0; tabIndex < tabNames.Length; tabIndex++)
            {
                var style = tabIndex == 0 ? TabStyles[0] : tabIndex == tabNames.Length - 1 ? TabStyles[2] : TabStyles[1];
                
                if (disabledTabsIndexes != null && disabledTabsIndexes.Contains(tabIndex))
                {
                    EditorGUI.BeginDisabledGroup (true);
                    GUILayout.Toggle(selectedTabIndex == tabIndex, tabNames[tabIndex], style);
                    EditorGUI.EndDisabledGroup();
                    continue;
                }
                
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