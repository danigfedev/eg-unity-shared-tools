using UnityEditor;
using UnityEngine;

namespace eg_unity_shared_tools.Code.Editor
{
    public enum GameIconToolTabs
    {
        SET_ICON = 0,
        IMPORT_ICON,
        SETTINGS
    }
    
    public class GameIconConfigurationTool : WindowWithTabs
    {
        [MenuItem(Constants.BaseMenu + Constants.ToolsMenu + nameof(GameIconConfigurationTool))]
        public static void ShowWindow()
        {
            WindowFactory.CreateWindowWithTabs<GameIconConfigurationTool>("Game Tool Icon", typeof(GameIconToolTabs));
            
            //Maybe define rules here to set the default tab. For example:
            // If Icons folder not found, open Settings tab directly
            // If Icons folder exists, but no Icons impoerted, open Import tab directly

            // _selectedTabIndex = (int) GameIconToolTabs.SETTINGS;
        }

        protected override void OnGUI()
        {
            base.OnGUI();
            
            switch (_selectedTabIndex)
            {
                case (int)GameIconToolTabs.SET_ICON:
                    DrawSetIconTab();
                    break;
                case (int)GameIconToolTabs.IMPORT_ICON:
                    DrawImportIconTab();
                    break;
                case (int)GameIconToolTabs.SETTINGS:
                    DrawSettingsTab();
                    break;
            }
        }

        private void DrawSetIconTab()
        {
            GUILayout.Label("Set Icon Tab");
        }
        
        private void DrawImportIconTab()
        {
            GUILayout.Label("Import Icon Tab");
        }
        
        private void DrawSettingsTab()
        {
            GUILayout.Label("Settings Tab");
        }
    }
}