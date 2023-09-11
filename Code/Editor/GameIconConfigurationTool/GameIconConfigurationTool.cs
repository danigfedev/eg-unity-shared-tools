using System;
using eg_unity_shared_tools.Code.Editor.Utilities;
using UnityEditor;
using UnityEngine;
using SharedConstants = eg_unity_shared_tools.Code.Constants;

namespace eg_unity_shared_tools.Code.Editor.GameIconConfigurationTool
{
    public class GameIconConfigurationTool : WindowWithTabs
    {
        private static GameIconConfigurationTool _window;

        private static IconToolSettingsModel _settingsModel;
        private IIconsToolTabPanel _settingsPanel;

        [MenuItem(SharedConstants.BaseMenu + SharedConstants.ToolsMenu + nameof(GameIconConfigurationTool))]
        public static void ShowWindow()
        {
            _window = WindowFactory.CreateWindowWithTabs<GameIconConfigurationTool>("Game Tool Icon", typeof(GameIconToolTabs));
        }

        private void OnEnable()
        {
            _settingsModel = new IconToolSettingsModel();
            _settingsModel.OnSettingsApplied += OnToolSettingsApplied;
            
            InitializeWindow();
        }

        private void OnDisable()
        {
            _settingsModel.OnSettingsApplied -= OnToolSettingsApplied;
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
            try
            {
                _settingsModel.LoadSettings();
            }
            catch (Exception exception)
            {
                Debug.LogError($"Loading settings failed with message: {exception.Message}");
                _window.Close();
            }

            if (string.IsNullOrWhiteSpace(_settingsModel.IconsRelativePath))
            {
                Debug.LogError($"Null or empty icons directory. This is not supported. Closing window");
                _window.Close();
                return;
            }

            ToggleSetIconTabAccessibility();

            InitializeTabPanels();
        }

        private void InitializeTabPanels()
        {
            _settingsPanel = new SettingsPanel(_settingsModel);
        }

        private static bool ToggleSetIconTabAccessibility()
        {
            var blockSetIconTabAccess = !FileUtils.DirectoryExists(_settingsModel.IconsAbsolutePath) ||
                                        FileUtils.DirectoryIsEmpty(_settingsModel.IconsAbsolutePath);
            
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
        
        private void OnToolSettingsApplied()
        {
            
        }
    }
}