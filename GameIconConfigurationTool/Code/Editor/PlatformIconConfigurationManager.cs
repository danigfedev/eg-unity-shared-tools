using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEngine;

namespace eg_unity_shared_tools.GameIconConfigurationTool.Code.Editor
{
    public class PlatformIconConfigurationManager
    {
        public static void SetIconsForBuildTargetGroup(Texture2D baseIcon, Texture2D backgroundIcon, Texture2D foregroundIcon, 
            Dictionary<BuildTargetGroup, bool> buildTargesWithStatusList)
        {
            foreach (var buildTargetGroupWithStatus in buildTargesWithStatusList)
            {
                var buildTargetGroup = buildTargetGroupWithStatus.Key;
                var shouldConfigureIcons = buildTargetGroupWithStatus.Value;
                
                switch (buildTargetGroup)
                {
                    case BuildTargetGroup.Standalone:
                        if (shouldConfigureIcons)
                            SetIcons(baseIcon, buildTargetGroup);
                        else
                        {
                            ResetStandaloneOverrideIcons(buildTargetGroup);
                        }
                        break;
                    case BuildTargetGroup.Android:
                        if (shouldConfigureIcons)
                        {
                            SetAndroidIcons(baseIcon, backgroundIcon, foregroundIcon);
                        }
                        else
                        {
                            SetAndroidIcons(null, null, null);
                        }

                        break;
                    case BuildTargetGroup.iOS:
                        var iosIcon = shouldConfigureIcons ? baseIcon : null;
                        SetIcons(iosIcon, buildTargetGroup);
                        break;
                }   
            }
            
            AssetDatabase.Refresh();
        }

        private static void SetIcons(Texture2D baseIcon, BuildTargetGroup buildTargetGroup)
        {
            var standaloneSizes = PlayerSettings.GetIconSizesForTargetGroup(buildTargetGroup);

            var standaloneIcons = PrepareIcons(baseIcon, standaloneSizes);

            PlayerSettings.SetIconsForTargetGroup(buildTargetGroup, standaloneIcons);
        }

        private static void SetAndroidIcons(Texture2D baseIcon, Texture2D backgroundIcon, Texture2D foregroundIcon)
        {
            SetAndroidAdaptiveIcons(AndroidPlatformIconKind.Legacy, baseIcon);
            SetAndroidAdaptiveIcons(AndroidPlatformIconKind.Round, baseIcon);
            SetAndroidAdaptiveIcons(AndroidPlatformIconKind.Adaptive, backgroundIcon, foregroundIcon);
        }

        private static void SetAndroidAdaptiveIcons(PlatformIconKind platformIconKind, params Texture2D[] iconTextures)
        {
            var platform = NamedBuildTarget.Android;

            PlatformIcon[] icons = PlayerSettings.GetPlatformIcons(platform, platformIconKind);
            
            //TODO Should resize the icon first?
            
            for (int i = 0; i < icons.Length; i++)
            {
                icons[i].SetTextures(iconTextures);
            }

            PlayerSettings.SetPlatformIcons(platform, platformIconKind, icons);
        }

        private static Texture2D[] PrepareIcons(Texture2D baseIcon, int[] standaloneSizes)
        {
            var iconCount = standaloneSizes.Length;
            var standaloneIcons = new Texture2D[iconCount];

            //TODO Should resize the icon first?
            
            for (int i = 0; i < iconCount; i++)
            {
                standaloneIcons[i] = baseIcon;
            }

            return standaloneIcons;
        }
        
        public static void SetDefaultIcon(Texture2D baseIcon)
        {
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new Texture2D[] { baseIcon });
        }
        
        private static void ResetStandaloneOverrideIcons(BuildTargetGroup buildTargetGroup)
        {
            PlayerSettings.SetIconsForTargetGroup(buildTargetGroup, new Texture2D[] { });
        }
    }
}