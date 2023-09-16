using System.IO;
using eg_unity_shared_tools.Utilities;
using eg_unity_shared_tools.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace eg_unity_shared_tools.GameIconConfigurationTool.Code.Editor.TabPanels
{
    public class SettingsPanel : IIconsToolTabPanel
    {
        private const string TextFieldTooltipId = "SettingsIconPathTextField";
        private const string TextFieldLabel = "Icons relative pat";
        private const string TextFieldTooltip = "The icons directory's relative path inside Assets folder. " +
                                                "Example: \"FirstPartyAssets/GameIcons\" = \"Assets/FirstPartyAssets/GameIcons\"";
            
        private readonly IconToolSettingsModel _settingsModel;
        private readonly TooltipCache _tooltipCache;
        private string _unnapliedFolderRelativePath = "";

        public SettingsPanel(IconToolSettingsModel settingsModel, TooltipCache tooltipCache)
        {
            _settingsModel = settingsModel;
            _tooltipCache = tooltipCache;
            _unnapliedFolderRelativePath = _settingsModel.IconsRelativePath;

            var textFieldGUIContent = new GUIContent(TextFieldLabel,
                TextFieldTooltip);
            _tooltipCache.RegisterTooltip(TextFieldTooltipId, textFieldGUIContent);
        }
            
        public void DrawPanel()
        {
            GUIUtils.VerticalLayout(false,
                DrawPathEditTextField,
                DrawApplySettingsButton,
                DrawResetSettingsButton);
        }

        private void DrawPathEditTextField()
        {
            var tooltip = _tooltipCache.GetTooltip(TextFieldTooltipId);
            
            GUIUtils.DrawSpace(10);
            _unnapliedFolderRelativePath = GUIUtils.DrawTextField(tooltip, _unnapliedFolderRelativePath);
        }

        private void DrawApplySettingsButton()
        {
            GUIUtils.DrawSpace(15);
            var blockInteraction = string.IsNullOrWhiteSpace(_unnapliedFolderRelativePath)
                             || _unnapliedFolderRelativePath == _settingsModel.IconsRelativePath;

            GUIUtils.DrawButton("Apply Changes", ApplySettings, blockInteraction);
        }
        
        private void DrawResetSettingsButton()
        {
            GUIUtils.DrawSpace(10);
            var blockInteraction = !_settingsModel.UsingCustomSettings;
            GUIUtils.DrawButton("Reset to defaults", ResetToDefaults, blockInteraction);
        }

        private void ResetToDefaults()
        {
            var oldIconsPath = _settingsModel.IconsAbsolutePath;
            _settingsModel.ResetToDefaultSettings();

            ResetSettings();
            
            void ResetSettings()
            {
                var newIconsPath = FileUtils.BuildAbsolutePathInProject(_settingsModel.IconsRelativePath);

                var filesTransfered = false;
                
                if (oldIconsPath != _settingsModel.IconsAbsolutePath)
                {
                    filesTransfered = TryTransferFiles(oldIconsPath, newIconsPath);   
                }
                
                if (filesTransfered)
                {
                    FileUtils.DeleteUnityDirectory(oldIconsPath, true);
                    AssetDatabase.Refresh();
                }

                _unnapliedFolderRelativePath = _settingsModel.IconsRelativePath;
                ClearTextFieldFocus();
            }
        }

        private void ApplySettings()
        {
            var oldIconsRelativePath = _settingsModel.IconsRelativePath;
            var oldIconsPath = FileUtils.BuildAbsolutePathInProject(oldIconsRelativePath);
            
            var newIconsPath = FileUtils.BuildAbsolutePathInProject(_unnapliedFolderRelativePath); //new test
            var filesTransfered = TryTransferFiles(oldIconsPath, newIconsPath);
            
            if (filesTransfered)
            {
                FileUtils.DeleteUnityDirectory(oldIconsPath, true);
                AssetDatabase.Refresh();
            }

            _settingsModel.SetNewIconsRelativePath(_unnapliedFolderRelativePath);
            _settingsModel.SaveSettings();
            
            ClearTextFieldFocus();
        }

        private bool TryTransferFiles(string oldIconsPath, string newIconsPath)
        {
            if (!FileUtils.DirectoryExists(oldIconsPath))
            {
                return false; 
            }

            if (!FileUtils.DirectoryExists(newIconsPath))
            {
                Directory.CreateDirectory(newIconsPath);
            }
                
            (var hasSubdirectories, var subdirectories) = FileUtils.DirectoryHasSubDirectories(oldIconsPath);
            
            if (hasSubdirectories)
            {
                DirectoryInfo directoryInfo = null;
                foreach (var oldIconDirectory in subdirectories)
                {
                    directoryInfo = new DirectoryInfo(oldIconDirectory);
                    var iconsFolderName = directoryInfo.Name;
                    var destinationPath = Path.Combine(newIconsPath, iconsFolderName);

                    Directory.Move(oldIconDirectory, destinationPath);
                }
            }

            return true;
        }
        
        private static void ClearTextFieldFocus()
        {
            GUI.FocusControl(null);
        }
    }
}