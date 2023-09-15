using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using eg_unity_shared_tools.Utilities;
using eg_unity_shared_tools.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace eg_unity_shared_tools.GameIconConfigurationTool.Code.Editor.TabPanels
{
    public class IconSelectionPanel : IIconsToolTabPanel
    {
        private const string ImportFirstIconButtonLabel = "Import an icon";
        private const string ImportNewIconButtonLabel = "Import new icon";
        private const string DirectoryBrowserLabel = "Select a folder";
        private const string AssetsDirectoryKey = "Assets";
        private const string IconFileKey = "icon.png";
        private const string BackgroundFileKey = "background.png";
        private const string ForegroundFileKey = "foreground.png";
        private const bool DefaultTargetToggleStatus = true;
        
        private IconToolSettingsModel _settingsModel;
        private int _selectedOptionIndex;
        private string[] _iconSubdirectoryPaths;
        private string[] _iconSubdirectoryNames;

        private Dictionary<BuildTargetGroup, bool> _buildTargetGroupToToggleStatusMap = new(); 
        private GUIStyle _boldLabelStyle;
        
        public IconSelectionPanel(IconToolSettingsModel settingsModel)
        {
            _settingsModel = settingsModel;

            _boldLabelStyle = new GUIStyle(EditorStyles.label);
            _boldLabelStyle.fontStyle = FontStyle.Bold;
            
            GetValidBuildTargetGroups();
        }
        
        public void DrawPanel()
        {
            //TODO: use a dirty flag that only changes when icons are loaded/added/removed
            
            if (!CheckExistingIcons())
            {
                DrawImportFirstIconSection();
            }
            else
            {
                SetIconNames(); //TODO: maybe move this into CheckExistingIcons. This will be involved in teh DirtyFlag, for sure

                DrawIconPreviewSection();
                DrawIconSelectionSection();
                DrawPlatformSelectionCheckboxes();
                DrawSetIconsButtonSection();
            }
        }

        private void DrawImportFirstIconSection()
        {
            GUIUtils.DrawSpace(10);
            GUIUtils.DrawDirectoryBrowserButton(DirectoryBrowserLabel, "", "",
                ImportFirstIconButtonLabel, ImportIcon);
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

            GUIUtils.DrawSpace(10);
            GUIUtils.HorizontalLayout(true, DrawPreview);

            void DrawPreview()
            {
                GUIUtils.VerticalLayout(false,
                    () => GUIUtils.DrawSpace(15),
                    () => GUIUtils.HorizontalLayout(true,
                        () => GUILayout.Label("Icon preview:", _boldLabelStyle)),
                    () => GUIUtils.HorizontalLayout(true,
                        () => GUILayout.Box(iconTexture, GUILayout.Width(128), GUILayout.Height(128))));
            }
        }
        
        private void DrawIconSelectionSection()
        {
            GUIUtils.VerticalLayout(false,
                () => GUIUtils.DrawSpace(15),
                () => GUILayout.Label("Icon selection:", _boldLabelStyle),
                () => GUIUtils.DrawSpace(5),
                DrawIconSelection);
            
            void DrawIconSelection()
            {
                GUIUtils.HorizontalLayout(false,
                    DrawIconSelectionDropdown,
                    () => GUIUtils.DrawDirectoryBrowserButton(DirectoryBrowserLabel, "", "",
                        ImportNewIconButtonLabel, ImportIcon));
            
                void DrawIconSelectionDropdown()
                {
                    _selectedOptionIndex = EditorGUILayout.Popup(_selectedOptionIndex, _iconSubdirectoryNames);
                }
            }
        }
        
        private void DrawPlatformSelectionCheckboxes()
        {
            GUIUtils.DrawSpace(15);

            GUIUtils.HorizontalLayout(false,
                () => GUILayout.Label("Platform Selection:", _boldLabelStyle),
                DrawPlatformSelectionToggles);

            void DrawPlatformSelectionToggles()
            {
                var keys = _buildTargetGroupToToggleStatusMap.Keys.ToList();
                foreach (var buildTargetGroup in keys)
                {
                    var togglePressed = _buildTargetGroupToToggleStatusMap[buildTargetGroup];
                    _buildTargetGroupToToggleStatusMap[buildTargetGroup] = GUILayout.Toggle(togglePressed, buildTargetGroup.ToString(), "Button");
                }
            }
        }
        
        private void DrawSetIconsButtonSection()
        {
            GUIUtils.DrawSpace(15);
            GUIUtils.DrawButton("Set Game Icons", SetGameIcons);
        }

        private void GetValidBuildTargetGroups()
        {
            var buildTargetGroupList = (BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup));
            var buildTargetList = (BuildTarget[])Enum.GetValues(typeof(BuildTarget));
            
            foreach (var buildTargetGroup in buildTargetGroupList)
            {
                if (buildTargetGroup == BuildTargetGroup.Unknown)
                {
                    continue;
                }
                
                foreach (var buildTarget in buildTargetList)
                {
                    var isPlatformSupported = BuildPipeline.IsBuildTargetSupported(buildTargetGroup, buildTarget)
                                              && !_buildTargetGroupToToggleStatusMap.TryGetValue(buildTargetGroup,
                                                  out _);
                    
                    if (isPlatformSupported)
                    {
                        _buildTargetGroupToToggleStatusMap.Add(buildTargetGroup, DefaultTargetToggleStatus);
                    }
                }
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
        
        private void ImportIcon(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath))
            {
                Debug.LogWarning("No directory provided. Aborting process");
                return;
            }
            
            var selectedDirectoryInfo = new DirectoryInfo(directoryPath);
            
            if (_iconSubdirectoryNames != null && _iconSubdirectoryNames.Contains(selectedDirectoryInfo.Name))
            {
                Debug.LogWarning($"Error importing {directoryPath}. There´s already an icon folder with the same name.");
                return;
            }
            
            if (!CheckSelectedDirectoryValidity(selectedDirectoryInfo))
            {
                Debug.LogWarning($"Error importing {directoryPath}. The contents of the directory are not valid.");
                return;
            }
            
            FileUtils.CopyDirectory(directoryPath, _settingsModel.IconsAbsolutePath);

            AssetDatabase.Refresh();
        }

        private bool CheckSelectedDirectoryValidity(DirectoryInfo iconDirectoryInfo)
        {
            //Directory will have three images:
            // - icon.png (1024 x 1024)
            // - background.png (1024 x 1024)
            // - foreground.png (1024 x 1024)
            
            var files = iconDirectoryInfo.GetFiles();

            if (files.Length != 3)
            {
                return false;
            }

            foreach (var file in files)
            {
                var isValid = file.Name.Contains(IconFileKey) 
                              || file.Name.Contains(BackgroundFileKey)
                              || file.Name.Contains(ForegroundFileKey);

                if (!isValid)
                {
                    return false;
                }
            }
            
            return true;
        }

        private void SetGameIcons()
        {
            var iconRelativePath = Path.Combine(AssetsDirectoryKey, _settingsModel.IconsRelativePath,
                _iconSubdirectoryNames[_selectedOptionIndex], IconFileKey);
            
            var backgroundRelativePath = Path.Combine(AssetsDirectoryKey, _settingsModel.IconsRelativePath,
                _iconSubdirectoryNames[_selectedOptionIndex], BackgroundFileKey);
            
            var foregroundRelativePath = Path.Combine(AssetsDirectoryKey, _settingsModel.IconsRelativePath,
                _iconSubdirectoryNames[_selectedOptionIndex], ForegroundFileKey);

            var baseIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconRelativePath);
            var backgroundIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(backgroundRelativePath);
            var foregroundIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(foregroundRelativePath);

            PlatformIconConfigurationManager.SetDefaultIcon(baseIcon);
            PlatformIconConfigurationManager.SetIconsForBuildTargetGroup(baseIcon,
                backgroundIcon, foregroundIcon, _buildTargetGroupToToggleStatusMap);
        }
    }
}