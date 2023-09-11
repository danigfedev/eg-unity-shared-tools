using System.IO;
using RepoConstants = eg_unity_shared_tools.Code.Constants;

namespace eg_unity_shared_tools.Code.Editor.GameIconConfigurationTool
{
    public class Constants
    {
        public const string DefaultSettingsRelativePath = "eg-unity-shared-tools/Code/Editor/GameIconConfigurationTool/Settings";
        public static readonly string CustomSettingsRelativePath = Path.Combine(RepoConstants.CustomSettingRelativesPath, "GameIconConfigurationTool");
        public const string DefaultSettingsFileName = "default_settings.json";
        public const string CustomSettingsFileName = "custom_settings.json";
    }
}