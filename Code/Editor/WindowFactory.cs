using UnityEditor;
using UnityEngine;

namespace eg_unity_shared_tools.Code.Editor
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