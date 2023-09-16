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

        private TooltipCache _tooltipCache;
        private IconToolSettingsModel _settingsModel;
        private IIconsToolTabPanel _settingsPanel;
        private IIconsToolTabPanel _iconSelectionPanel;

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
            _tooltipCache = new TooltipCache();
            _settingsModel = new IconToolSettingsModel();
            
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
            _settingsPanel = new SettingsPanel(_settingsModel, _tooltipCache);
            _iconSelectionPanel = new IconSelectionPanel(_settingsModel);
        }
    }
}