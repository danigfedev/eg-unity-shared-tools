using System.IO;
using System.Linq;
using eg_unity_shared_tools.Utilities;
using eg_unity_shared_tools.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace eg_unity_shared_tools.GameIconConfigurationTool.Code.Editor
{
    public class IconSelectionPanel : IIconsToolTabPanel
    {
        private const string ImportFirstIconButtonLabel = "Import an icon";
        private const string ImportNewIconButtonLabel = "Import new icon";
        private const string DirectoryBrowserLabel = "Select a folder";
        
        private IconToolSettingsModel _settingsModel;
        private int _selectedOptionIndex;
        private string[] _iconSubdirectoryPaths;
        private string[] _iconSubdirectoryNames;
        
        public IconSelectionPanel(IconToolSettingsModel settingsModel)
        {
            _settingsModel = settingsModel;
        }
        
        public void DrawPanel()
        {
            GUILayout.Label("Icon Selection Tab");
            
            //TODO: use a dirty flag that only changes when icons are loaded/added/removed
            
            if (!CheckExistingIcons())
            {
                UGUIUtils.DrawButton(ImportFirstIconButtonLabel, ImportIcon);
            }
            else
            {
                SetIconNames(); //TODO maybe move this into CheckExistingIcons. This will be involved in teh DirtyFlag, for sure
                _selectedOptionIndex = EditorGUILayout.Popup(_selectedOptionIndex, _iconSubdirectoryNames);
                
                //Show import new button in horizontal layout
                //Show selected icon preview
                //Show apply button
            }
        }

        private bool CheckExistingIcons()
        {
            bool existingIcons;
            string[] subdirectories = null;

            if (FileUtils.DirectoryExists(_settingsModel.IconsAbsolutePath))
            {
                bool hasSubdirectories;
                (hasSubdirectories, subdirectories) = FileUtils.DirectoryHasSubDirectories(_settingsModel.IconsAbsolutePath);
                existingIcons = hasSubdirectories;
            }
            else
            {
                existingIcons = false;
            }

            _iconSubdirectoryPaths = subdirectories;

            return existingIcons;
        }

        private void SetIconNames()
        {
            var iconsCount = _iconSubdirectoryPaths.Length;
            var iconNames = new string[iconsCount];

            for (int directoryIndex = 0; directoryIndex < iconsCount; directoryIndex++)
            {
                var splitPath = _iconSubdirectoryPaths[directoryIndex].Split(Path.DirectorySeparatorChar);
                var directoryName = splitPath[splitPath.Length - 1];
                iconNames[directoryIndex] = directoryName;
            }

            _iconSubdirectoryNames = iconNames;
        }
        
        private void ImportIcon()
        {
            var selectedDirectory = EditorUtility.OpenFolderPanel(DirectoryBrowserLabel, "", "");
            
            var selectedDirectoryInfo = new DirectoryInfo(selectedDirectory);
            
            if (_iconSubdirectoryNames != null && _iconSubdirectoryNames.Contains(selectedDirectoryInfo.Name))
            {
                Debug.LogWarning($"Error importing {selectedDirectory}. There´s already an icon folder with the same name.");
                return;
            }

            if (!CheckSelectedDirectoryValidity())
            {
                Debug.LogWarning($"Error importing {selectedDirectory}. The contents of the directory are not valid.");
                return;
            }
            
            FileUtils.CopyDirectory(selectedDirectory, _settingsModel.IconsAbsolutePath);

            AssetDatabase.Refresh();
        }

        private bool CheckSelectedDirectoryValidity()
        {
            //TODO: define validity rules (count number of images in folder, and maybe check names?)
            
            //Directory will have three images (presumably png files):
            // - icon.png (1024 x 1024)
            // - background.png (1024 x 1024)
            // - foreground.png (1024 x 1024)
            return true;
        }
    }
}