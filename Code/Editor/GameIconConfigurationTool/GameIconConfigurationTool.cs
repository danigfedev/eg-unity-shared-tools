using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;
using SharedConstants = eg_unity_shared_tools.Code.Constants;

namespace eg_unity_shared_tools.Code.Editor.GameIconConfigurationTool
{
    public enum GameIconToolTabs
    {
        SET_ICON = 0,
        IMPORT_ICON,
        SETTINGS
    }
    
    public class GameIconConfigurationTool : WindowWithTabs
    {
        private static GameIconConfigurationTool _window;
        
        [MenuItem(SharedConstants.BaseMenu + SharedConstants.ToolsMenu + nameof(GameIconConfigurationTool))]
        public static void ShowWindow()
        {
            _window = WindowFactory.CreateWindowWithTabs<GameIconConfigurationTool>("Game Tool Icon", typeof(GameIconToolTabs));
        }

        private void OnEnable()
        {
            InitializeWindow();
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

        private void InitializeWindow()
        {
            var settings = LoadToolSettings();

            if (settings == null)
            {
                Debug.LogError($"NULL Tool settings. Unable to initialize {nameof(GameIconConfigurationTool)} tool. Closing window");
                _window.Close();
                return;
            }

            if (string.IsNullOrWhiteSpace(settings.DefaultIconDirectory))
            {
                Debug.LogError($"Null or empty DefaultIconDirectory. This is not supporting. Closing window");
                _window.Close();
                return;
            }

            var iconsRelativePath = string.IsNullOrWhiteSpace(settings.CustomIconDirectory)
                ? settings.DefaultIconDirectory
                : settings.CustomIconDirectory;

            var iconsPath = Path.Combine(Application.dataPath, iconsRelativePath);

            bool shouldOpenImportTab = !Directory.Exists(iconsPath) || Directory.GetDirectories(iconsPath).Length == 0;

            _selectedTabIndex = shouldOpenImportTab
                ? (int)GameIconToolTabs.IMPORT_ICON
                : (int)GameIconToolTabs.SET_ICON;

            if (shouldOpenImportTab)
            {
                _disabledTabs = new int[]
                {
                    (int)GameIconToolTabs.SET_ICON
                };
            }
        }
        
        private static IconToolSettings LoadToolSettings()
        {
            string filePath = Path.Combine(Application.dataPath,
                Constants.SettingRelativesPath, 
                Constants.SettingsFileName);
            
            if (!File.Exists(filePath))
            {
                return null;
            }

            //TODO write an utility that receives a filepath, reads it an parses the object.
            string jsonString = File.ReadAllText(filePath);
            var settings = JsonUtility.FromJson<IconToolSettings>(jsonString);

            return settings;
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