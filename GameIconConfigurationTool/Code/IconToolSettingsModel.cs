using System;
using System.IO;
using eg_unity_shared_tools.GameIconConfigurationTool.Code.DataContracts;
using eg_unity_shared_tools.Utilities;
using UnityEditor;
using UnityEngine;

namespace eg_unity_shared_tools.GameIconConfigurationTool.Code
{
    public class IconToolSettingsModel
    {
        public event Action OnSettingsApplied;
        public string IconsRelativePath => _toolSettings.IconsDirectory;
        public string IconsAbsolutePath => FileUtils.BuildAbsolutePathInProject(IconsRelativePath);

        private IconToolSettings _toolSettings;
        
        private readonly string _defaultSettingsPath = Path.Combine(Application.dataPath,
            Constants.DefaultSettingsRelativePath, 
            Constants.DefaultSettingsFileName);
        
        private readonly string _customSettingsPath = Path.Combine(Application.dataPath,
            Constants.CustomSettingsRelativePath, 
            Constants.CustomSettingsFileName);
        
        private string _currentSettingsPath;

        public void SetNewIconsRelativePath(string newPath)
        {
            _toolSettings.IconsDirectory = newPath;
        }
        
        public void LoadSettings()
        {
            if (FileUtils.FileExists(_customSettingsPath))
            {
                _currentSettingsPath = _customSettingsPath;
                _toolSettings = JsonFileManager.LoadJson<IconToolSettings>(_customSettingsPath);
            }
            else if (FileUtils.FileExists(_defaultSettingsPath))
            {
                _currentSettingsPath = _defaultSettingsPath;
                _toolSettings = JsonFileManager.LoadJson<IconToolSettings>(_defaultSettingsPath);
            }
            else
            {
                throw new FileNotFoundException($"No settings file found at {_currentSettingsPath}");
            }
        }
        
        public void SaveSettings()
        {
            try
            {
                FileUtils.TryCreateDirectory(_customSettingsPath);
                JsonFileManager.SaveJson(_toolSettings, _customSettingsPath);
                AssetDatabase.Refresh();
            
                OnSettingsApplied?.Invoke();
            }
            catch (Exception exception)
            {
                Debug.LogError(exception.Message);
            }
        }
    }
}