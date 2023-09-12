using System.IO;
using eg_unity_shared_tools.Utilities;
using eg_unity_shared_tools.Utilities.Editor;
using UnityEditor;

namespace eg_unity_shared_tools.GameIconConfigurationTool.Code.Editor
{
    public class SettingsPanel : IIconsToolTabPanel
    {
        private IconToolSettingsModel _settingsModel;
        private string _unnapliedFolderRelativePath = "";

        public SettingsPanel(IconToolSettingsModel settingsModel)
        {
            _settingsModel = settingsModel;
            _unnapliedFolderRelativePath = _settingsModel.IconsRelativePath;
        }
            
        public void DrawPanel()
        {
            _unnapliedFolderRelativePath = EditorGUILayout.TextField("Icons path", _unnapliedFolderRelativePath);
            
            var hasChanges = !string.IsNullOrWhiteSpace(_unnapliedFolderRelativePath)
                         && _unnapliedFolderRelativePath != _settingsModel.IconsRelativePath;
            
            UGUIUtils.DrawButton("Apply Changes", ApplySettings, hasChanges);
            
            //TODO add a reset to default button? Where?
        }

        private void ApplySettings()
        {
            var oldIconsRelativePath = _settingsModel.IconsRelativePath;
            
            _settingsModel.SetNewIconsRelativePath(_unnapliedFolderRelativePath);

            var oldIconsPath = FileUtils.BuildAbsolutePathInProject(oldIconsRelativePath);

            var filesTransfered = TryTransferFiles(oldIconsPath);
            
            _settingsModel.SaveSettings();
            
            if (filesTransfered)
            {
                FileUtils.DeleteUnityDirectory(oldIconsPath, true);
                AssetDatabase.Refresh();
            }
        }

        private bool TryTransferFiles(string oldIconsPath)
        {
            if (!FileUtils.DirectoryExists(oldIconsPath))
            {
                return false; 
            }

            if (!FileUtils.DirectoryExists(_settingsModel.IconsAbsolutePath))
            {
                Directory.CreateDirectory(_settingsModel.IconsAbsolutePath);
            }
                
            (var hasSubdirectories, var subdirectories) = FileUtils.DirectoryHasSubDirectories(oldIconsPath);
            
            if (hasSubdirectories)
            {
                DirectoryInfo directoryInfo = null;
                foreach (var oldIconDirectory in subdirectories)
                {
                    directoryInfo = new DirectoryInfo(oldIconDirectory);
                    var iconsFolderName = directoryInfo.Name;
                    var destinationPath = Path.Combine(_settingsModel.IconsAbsolutePath, iconsFolderName);

                    Directory.Move(oldIconDirectory, destinationPath);
                }
            }

            return true;
        }
    }
}