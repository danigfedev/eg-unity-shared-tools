using System.IO;
using eg_unity_shared_tools.Utilities;
using UnityEditor;
using UnityEngine;

namespace eg_unity_shared_tools.GameIconConfigurationTool.Code.Editor
{
    public class IconSelectionPanel : IIconsToolTabPanel
    {
        private IconToolSettingsModel _settingsModel;
        private int _selectedOptionIndex;
        
        public IconSelectionPanel(IconToolSettingsModel settingsModel)
        {
            _settingsModel = settingsModel;
        }
        
        public void DrawPanel()
        {
            GUILayout.Label("Icon Selection Tab");
            
            //TODO: use a dirty flag that only changes when icons are loaded/added/removed
            
            (var existingIcons, var subdirectories) = CheckExistingIcons();
            
            if (!existingIcons)
            {
                // show Import button
                
                //Import process:
                // You import the whole folder, with its name
                // if that folder exists abort and tell user to rename it
                // check folder validity (count number of images in folder, and maybe check names?)
            }
            else
            {
                var iconNames = GetDirectoryNames(subdirectories);
                _selectedOptionIndex = EditorGUILayout.Popup(_selectedOptionIndex, iconNames);
                //Show import new button in horizontal layout
                //Show selected icon preview
                //Show apply button
            }
        }

        private (bool, string[]) CheckExistingIcons()
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

            return (existingIcons, subdirectories);
        }

        private string[] GetDirectoryNames(string[] directoryPaths)
        {
            var iconsCount = directoryPaths.Length;
            var directoryNames = new string[iconsCount];

            for (int directoryIndex = 0; directoryIndex < iconsCount; directoryIndex++)
            {
                var splitPath = directoryPaths[directoryIndex].Split(Path.DirectorySeparatorChar);
                var directoryName = splitPath[splitPath.Length - 1];
                directoryNames[directoryIndex] = directoryName;
            }

            return directoryNames;
        }
    }
}