using System;
using eg_unity_shared_tools.GameIconConfigurationTool.Code.Editor.TabPanels;
using eg_unity_shared_tools.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using SharedConstants = eg_unity_shared_tools.Utilities.Constants;

namespace eg_unity_shared_tools.GameIconConfigurationTool.Code.Editor
{
    public class GameIconConfigurationTool : WindowWithTabs
    {
        private static GameIconConfigurationTool _window;

        private static IconToolSettingsModel _settingsModel;
        private IIconsToolTabPanel _settingsPanel;
        private IIconsToolTabPanel _iconSelectionPanel;

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
                case (int)GameIconToolTabs.ICON_SELECTION:
                    _iconSelectionPanel.DrawPanel();
                    break;
                case (int)GameIconToolTabs.SETTINGS:
                    _settingsPanel.DrawPanel();
                    break;
            }
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
            
            InitializeTabPanels();
        }

        private void InitializeTabPanels()
        {
            _selectedTabIndex = (int)GameIconToolTabs.ICON_SELECTION;
            _settingsPanel = new SettingsPanel(_settingsModel);
            _iconSelectionPanel = new IconSelectionPanel(_settingsModel);
        }
        
        //IDEA: ITabContent interface defining DrawTabContent
        // Then implement class IconToolSettingsTab : ITabContent
        // Study if it's better that or using an abstract class
        //Then create the three objects on initialization, or ask a factory to create them
        //Here, instead of implementing this draw methods, just call the DrawTabContent method from the interface
        
        //DOUBT: How to handle Initialization? That can be abstracted, since it's specific of each tab
        // I can skip the Factory and creat the tabs here, by type. Then I can initialize them.
        
        private void DrawImportIconTab()
        {
            GUILayout.Label("Import Icon Tab");
        }
        
        private void OnToolSettingsApplied()
        {
            
        }
    }
}