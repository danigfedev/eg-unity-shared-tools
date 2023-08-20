using System;
using System.IO;
using eg_unity_shared_tools.Code.Editor.Utilities;
using UnityEditor;
using UnityEngine;

namespace eg_unity_shared_tools.Code.Editor.GameIconConfigurationTool
{
    public class SettingsPanel : IIconsToolTabPanel
    {
        private IconToolSettings _settings;
        private Action<string> _applySettingsCallback;
        
        private string _settingsFilePath;
        private string _iconsFolderRelativePath;
        private string _unnapliedFolderRelativePath = "";

        public SettingsPanel(IconToolSettings settings, string iconsRelativePath, Action<string> applySettingsCallback)
        {
            _settings = settings;
            _iconsFolderRelativePath = iconsRelativePath;
            _applySettingsCallback = applySettingsCallback;
            _unnapliedFolderRelativePath = _iconsFolderRelativePath;
            
            _settingsFilePath = Path.Combine(Application.dataPath,
                Constants.SettingRelativesPath, 
                Constants.SettingsFileName);
        }
            
        public void DrawPanel()
        {
            // GUILayout.Label("Settings Tab");
            var hasChanges = false;
            
            _unnapliedFolderRelativePath = EditorGUILayout.TextField("Icons path", _unnapliedFolderRelativePath);
            
            hasChanges = _unnapliedFolderRelativePath != _iconsFolderRelativePath;

            GUI.enabled = hasChanges;
            UGUIUtils.DrawButton("Apply Changes", ApplySettings);
            GUI.enabled = true;
            
            //TODO dra a reset to default button? Where?
        }

        public void UpdateIconsRelativePath()
        {
            throw new System.NotImplementedException();
        }

        private void ApplySettings()
        {
            var oldIconsRelativePath = _iconsFolderRelativePath;
            
            _iconsFolderRelativePath = _unnapliedFolderRelativePath;
            _settings.CustomIconDirectory = _iconsFolderRelativePath;

            var oldIconsPath = FileUtils.BuildAbsolutePathInProject(oldIconsRelativePath);
            var newIconsPath = FileUtils.BuildAbsolutePathInProject(_iconsFolderRelativePath);

            //TODO Add more file utils
            if (!FileUtils.DirectoryExists(newIconsPath))
            {
                Directory.CreateDirectory(newIconsPath);
            }
            
            string[] directories = null;
            if (!FileUtils.DirectoryIsEmpty(oldIconsPath, ref directories))
            {
                DirectoryInfo directoryInfo = null;
                foreach (var oldIconDirectory in directories)
                {
                    directoryInfo = new DirectoryInfo(oldIconDirectory);
                    var iconsFolderName = directoryInfo.Name;
                    var destinationPath = Path.Combine(newIconsPath, iconsFolderName);
                    
                    Directory.Move(oldIconDirectory, destinationPath);
                }
            }

            JsonFileManager.SaveJson(_settings, _settingsFilePath);

            //Delete old path if saved successfully
            Directory.Delete(oldIconsPath);

            _applySettingsCallback?.Invoke(_iconsFolderRelativePath);
        }
    }
}