using System.IO;
using eg_unity_shared_tools.Code.Editor.Utilities;
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
        private static string _iconsRelativePath;
        private static string _iconsAbsolutePath;

        private IIconsToolTabPanel _settingsPanel;
        
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
                    _settingsPanel.DrawPanel();
                    break;
            }
            
            ToggleSetIconTabAccessibility();
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
                Debug.LogError($"Null or empty DefaultIconDirectory. This is not supported. Closing window");
                _window.Close();
                return;
            }

            _iconsRelativePath = string.IsNullOrWhiteSpace(settings.CustomIconDirectory)
                ? settings.DefaultIconDirectory
                : settings.CustomIconDirectory;

            _iconsAbsolutePath = FileUtils.BuildAbsolutePathInProject(_iconsRelativePath);

            ToggleSetIconTabAccessibility();

            InitializeTabPanels(settings);
        }
        
        private static IconToolSettings LoadToolSettings()
        {
            var settingsPath = Path.Combine(Application.dataPath,
                Constants.SettingRelativesPath, 
                Constants.SettingsFileName);

            var settings = JsonFileManager.LoadJson<IconToolSettings>(settingsPath);
            
            return settings;
        }

        private void InitializeTabPanels(IconToolSettings settings)
        {
            _settingsPanel = new SettingsPanel(settings, _iconsRelativePath, OnToolSettingsApplied);
        }

        private static bool ToggleSetIconTabAccessibility()
        {
            var blockSetIconTabAccess = !FileUtils.DirectoryExists(_iconsAbsolutePath) ||
                                        FileUtils.DirectoryIsEmpty(_iconsAbsolutePath);
            
            if (blockSetIconTabAccess)
            {
                if (_disabledTabs == null)
                {
                    _selectedTabIndex = (int)GameIconToolTabs.IMPORT_ICON;
                }
                
                _disabledTabs = new int[]
                {
                    (int)GameIconToolTabs.SET_ICON
                };
            }
            else
            {
                _disabledTabs = null;
            }

            return blockSetIconTabAccess;
        }
        
        //IDEA: ITabContent interface defining DrawTabContent
        // Then implement class IconToolSettingsTab : ITabContent
        // Study if it's better that or using an abstract class
        //Then create the three objects on initialization, or ask a factory to create them
        //Here, instead of implementing this draw methods, just call the DrawTabContent method from the interface
        
        //DOUBT: How to handle Initialization? That can be abstracted, since it's specific of each tab
        // I can skip the Factory and creat the tabs here, by type. Then I can initialize them.

        private void DrawSetIconTab()
        {
            GUILayout.Label("Set Icon Tab");
        }
        
        private void DrawImportIconTab()
        {
            GUILayout.Label("Import Icon Tab");
        }
        
        private void OnToolSettingsApplied(string newIconsRelativePath)
        {
            _iconsRelativePath = newIconsRelativePath;
            _iconsAbsolutePath = FileUtils.BuildAbsolutePathInProject(_iconsRelativePath);
            
            //TODO send relative path to Set_Icon and Import tabs
        }
    }
}