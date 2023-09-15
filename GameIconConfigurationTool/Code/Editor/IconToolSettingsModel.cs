using System;
using System.IO;
using eg_unity_shared_tools.GameIconConfigurationTool.Code.DataContracts;
using eg_unity_shared_tools.Utilities;
using UnityEditor;
using UnityEngine;

namespace eg_unity_shared_tools.GameIconConfigurationTool.Code.Editor
{
    public class IconToolSettingsModel
    {
        public event Action OnSettingsApplied;
        public string IconsRelativePath => _toolSettings.IconsDirectory;
        public string IconsAbsolutePath => FileUtils.BuildAbsolutePathInProject(IconsRelativePath);
        public bool UsingCustomSettings { get; private set; }

        private IconToolSettings _toolSettings;
        private string _currentSettingsPath;
        
        private readonly string _defaultSettingsPath = Path.Combine(Application.dataPath,
            Constants.DefaultSettingsRelativePath, 
            Constants.DefaultSettingsFileName);
        
        private readonly string _customSettingsPath = Path.Combine(Application.dataPath,
            Constants.CustomSettingsRelativePath, 
            Constants.CustomSettingsFileName);

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
                UsingCustomSettings = true;
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

                UsingCustomSettings = true;
                
                OnSettingsApplied?.Invoke();
            }
            catch (Exception exception)
            {
                Debug.LogError(exception.Message);
            }
        }
        
        public void ResetToDefaultSettings()
        {
            _currentSettingsPath = _defaultSettingsPath;
            _toolSettings = JsonFileManager.LoadJson<IconToolSettings>(_defaultSettingsPath);
            
            File.Delete(_customSettingsPath);
            
            UsingCustomSettings = false;
        }
    }
}