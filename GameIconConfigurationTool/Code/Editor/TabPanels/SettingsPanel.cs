using System.IO;
using eg_unity_shared_tools.Utilities;
using eg_unity_shared_tools.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace eg_unity_shared_tools.GameIconConfigurationTool.Code.Editor.TabPanels
{
    public class SettingsPanel : IIconsToolTabPanel
    {
        private IconToolSettingsModel _settingsModel;
        private string _unnapliedFolderRelativePath = "";
        private GUIContent _textFieldGUIContent;

        public SettingsPanel(IconToolSettingsModel settingsModel)
        {
            _settingsModel = settingsModel;
            _unnapliedFolderRelativePath = _settingsModel.IconsRelativePath;
            _textFieldGUIContent = new GUIContent("Icons relative path",
                "The icons directory's relative path inside Assets folder. " +
                "/n Example: \"FirstPartyAssets/GameIcons\" = \"Assets/FirstPartyAssets/GameIcons\"");
        }
            
        public void DrawPanel()
        {
            UGUIUtils.VerticalLayout(false,
                DrawPathEditTextField,
                DrawApplySettingsButton,
                DrawResetSettingsButton);
        }

        private void DrawPathEditTextField()
        {
            UGUIUtils.DrawSpace(10);
            _unnapliedFolderRelativePath =
                EditorGUILayout.TextField(_textFieldGUIContent, _unnapliedFolderRelativePath);
        }

        private void DrawApplySettingsButton()
        {
            UGUIUtils.DrawSpace(15);
            var blockInteraction = string.IsNullOrWhiteSpace(_unnapliedFolderRelativePath)
                             || _unnapliedFolderRelativePath == _settingsModel.IconsRelativePath;

            UGUIUtils.DrawButton("Apply Changes", ApplySettings, blockInteraction);
        }
        
        private void DrawResetSettingsButton()
        {
            UGUIUtils.DrawSpace(10);
            var blockInteraction = !_settingsModel.UsingCustomSettings;
            UGUIUtils.DrawButton("Reset to defaults", ResetToDefaults, blockInteraction);
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