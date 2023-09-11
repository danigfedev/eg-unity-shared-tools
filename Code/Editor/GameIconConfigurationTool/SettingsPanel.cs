using System.IO;
using eg_unity_shared_tools.Code.Editor.Utilities;
using UnityEditor;
using UnityEngine;

namespace eg_unity_shared_tools.Code.Editor.GameIconConfigurationTool
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
            var hasChanges = false;
            
            _unnapliedFolderRelativePath = EditorGUILayout.TextField("Icons path", _unnapliedFolderRelativePath);
            
            hasChanges = _unnapliedFolderRelativePath != _settingsModel.IconsRelativePath;

            GUI.enabled = hasChanges;
            UGUIUtils.DrawButton("Apply Changes", ApplySettings);
            GUI.enabled = true;
            
            //TODO add a reset to default button? Where?
        }

        public void UpdateIconsRelativePath()
        {
            throw new System.NotImplementedException();
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
                Directory.Delete(oldIconsPath);   
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
                
            string[] directories = null;
            if (!FileUtils.DirectoryIsEmpty(oldIconsPath, ref directories))
            {
                DirectoryInfo directoryInfo = null;
                foreach (var oldIconDirectory in directories)
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