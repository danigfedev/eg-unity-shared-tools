using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;

namespace eg_unity_shared_tools.Utilities.Editor
{
    public class WindowWithTabs : EditorWindow
    {
        protected static int _selectedTabIndex = 0;
        protected static int[] _disabledTabs = null;
        private static string[] _tabNames;
        
        protected virtual void OnGUI()
        {
            _selectedTabIndex = GUIUtils.DrawTabs(_selectedTabIndex, _tabNames, _disabledTabs);
        }

        public void SetWindowTabs(Type tabs)
        {
            if (!tabs.IsEnum)
            {
                throw new ArgumentException("The specified type is not an enum.");
            }
            
            var enumValues = Enum.GetValues(tabs);
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            var tabNames = new List<string>();
            
            foreach (object value in enumValues)
            {
                var enumString = value.ToString().ToLower().Replace("_", " ");
                var tabName =textInfo.ToTitleCase(enumString);
                tabNames.Add(tabName);
            }

            _tabNames = tabNames.ToArray();
        }
    }
}