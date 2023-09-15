using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace eg_unity_shared_tools.Utilities.Editor
{
    public static class UGUIUtils
    {
        private static readonly string[] TabStyles = 
        {
            "ButtonLeft",
            "ButtonMid",
            "ButtonRight"
        };
        
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

        public static void DrawSpace(float pixels) => EditorGUILayout.Space(pixels);

        public static void DrawButton(string label, Action callback,
            params GUILayoutOption[] options)
        {
            DrawButton(label, callback, false, options);
        }

        public static void DrawButton(string label, Action callback, bool disableInteraction,
            params GUILayoutOption[] options)
        {
            EditorGUI.BeginDisabledGroup(disableInteraction);
            if (GUILayout.Button(label, options))
            {
                callback?.Invoke();
            }

            EditorGUI.EndDisabledGroup();
        }

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

        public static void DrawDirectoryBrowserButton(string browserTitle,
            string baseDirectory, string defaultName, string buttonLabel, Action<string> callback, params GUILayoutOption[] options)
        {
            DrawDirectoryBrowserButton(browserTitle, baseDirectory, defaultName, buttonLabel, callback, false, options);
        }
        
        public static void DrawDirectoryBrowserButton(string browserTitle, string baseDirectory, string defaultName, string buttonLabel, Action<string> callback, bool disableButton, params GUILayoutOption[] options)
        {
            DrawButton(buttonLabel, OpenBrowser, disableButton, options);
            
            void OpenBrowser()
            {
                var path = OpenDirectoryBrowser(browserTitle, baseDirectory, defaultName);
                callback?.Invoke(path);
            }
        }

        public static void DrawFileBrowserButton(string browserTitle, string baseDirectory, string extension, string buttonLabel, Action<string> callback, params GUILayoutOption[] options)
        {
            DrawFileBrowserButton(browserTitle, baseDirectory, extension, buttonLabel, callback, false, options);
        }

        public static void DrawFileBrowserButton(string browserTitle, string baseDirectory, string extension, string buttonLabel, Action<string> callback, bool disableButton, params GUILayoutOption[] options)
        {
            DrawButton(buttonLabel, OpenBrowser, disableButton, options);
            
            void OpenBrowser()
            {
                var path = OpenFileBrowser(browserTitle, baseDirectory, extension);
                callback?.Invoke(path);
            }
        }
        
        public static string OpenDirectoryBrowser(string browserTitle, string baseDirectory, string defaultName)
        {
            return EditorUtility.OpenFolderPanel(browserTitle, baseDirectory, defaultName);
        }

        public static string OpenFileBrowser(string browserTitle, string baseDirectory, string extension)
        {
            return EditorUtility.OpenFilePanel(browserTitle, baseDirectory, extension);
        }
    }
}