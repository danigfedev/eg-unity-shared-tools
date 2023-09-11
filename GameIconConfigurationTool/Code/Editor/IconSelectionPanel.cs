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
        private const string AssetsDirectoryKey = "Assets";
        private const string IconFileKey = "icon.png";
        
        private IconToolSettingsModel _settingsModel;
        private int _selectedOptionIndex;
        private string[] _iconSubdirectoryPaths;
        private string[] _iconSubdirectoryNames;
        private GUIStyle _boldLabelStyle;
        
        public IconSelectionPanel(IconToolSettingsModel settingsModel)
        {
            _settingsModel = settingsModel;

            _boldLabelStyle = new GUIStyle(EditorStyles.label);
            _boldLabelStyle.fontStyle = FontStyle.Bold;
        }
        
        public void DrawPanel()
        {
            //TODO: use a dirty flag that only changes when icons are loaded/added/removed
            
            if (!CheckExistingIcons())
            {
                UGUIUtils.DrawButton(ImportFirstIconButtonLabel, ImportIcon);
            }
            else
            {
                SetIconNames(); //TODO maybe move this into CheckExistingIcons. This will be involved in teh DirtyFlag, for sure

                DrawIconPreviewSection();
                DrawIconSelectionSection();
                DrawSetIconsButtonSection();
            }
        }

        private void DrawIconPreviewSection()
        {
            var iconRelativePath = Path.Combine(AssetsDirectoryKey, _settingsModel.IconsRelativePath,
                _iconSubdirectoryNames[_selectedOptionIndex], IconFileKey);
            var iconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(iconRelativePath);
            if (iconTexture == null)
            {
                Debug.Log($"Icon texture is null. Path:{iconRelativePath}");
                return;
            }

            UGUIUtils.HorizontalLayout(true, DrawPreview);

            void DrawPreview()
            {
                UGUIUtils.VerticalLayout(false,
                    () => GUILayout.Space(15),
                    () => UGUIUtils.HorizontalLayout(true,
                        () => GUILayout.Label("Icon preview:", _boldLabelStyle)),
                    () => UGUIUtils.HorizontalLayout(true,
                        () => GUILayout.Box(iconTexture, GUILayout.Width(128), GUILayout.Height(128))));
            }
        }
        
        private void DrawIconSelectionSection()
        {
            UGUIUtils.VerticalLayout(false,
                () => GUILayout.Space(15),
                () => GUILayout.Label("Icon selection:", _boldLabelStyle),
                () => GUILayout.Space(5),
                DrawIconSelection);
            
            void DrawIconSelection()
            {
                UGUIUtils.HorizontalLayout(false,
                    DrawIconSelectionDropdown,
                    () => UGUIUtils.DrawButton(ImportNewIconButtonLabel, ImportIcon));
            
                void DrawIconSelectionDropdown()
                {
                    _selectedOptionIndex = EditorGUILayout.Popup(_selectedOptionIndex, _iconSubdirectoryNames);
                }
            }
        }
        
        private void DrawSetIconsButtonSection()
        {
            GUILayout.Space(15);
            UGUIUtils.DrawButton("Set Game Icons", SetGameIcons);
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

        private void SetGameIcons()
        {
            
        }
    }
}