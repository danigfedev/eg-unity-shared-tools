using System;
using eg_unity_shared_tools.Code.Editor.GameIconConfigurationTool;
using UnityEditor;
using UnityEngine;

namespace eg_unity_shared_tools.Code.Editor.Utilities
{
    public static class WindowFactory
    {
        public static TToolWindow CreateWindow<TToolWindow>(string windowTitle, Vector2 minSize = default) where TToolWindow : EditorWindow
        {

            return CreateWindow<TToolWindow>(windowTitle, false, minSize);
        }

        public static TToolWindow CreateModalWindow<TToolWindow>(string windowTitle, Vector2 minSize = default) where TToolWindow : EditorWindow
        {
            return CreateWindow<TToolWindow>(windowTitle, true, minSize);
        }

        public static TToolWindow CreateWindowWithTabs<TToolWindow>(string windowTitle, Type tabsEnum, Vector2 minSize = default) where TToolWindow : EditorWindow
        {
            var window = CreateWindow<TToolWindow>(windowTitle, false, minSize);
            
            if (window is WindowWithTabs windowWithTabs)
            {
                windowWithTabs.SetWindowTabs(typeof(GameIconToolTabs));
            }

            return window;
        }

        private static TToolWindow CreateWindow<TToolWindow>(string windowTitle, 
            bool isUtility = false,
            Vector2 minSize = default) where TToolWindow : EditorWindow
        {
            if (minSize == default)
            {
                minSize = new Vector2(Constants.DefaultWindowMinWidth, Constants.DefaultWindowMinHeight);
            }
            
            var window = EditorWindow.GetWindow<TToolWindow>(isUtility, windowTitle);
            window.minSize = minSize;
            
            return window;
        }
    }
}