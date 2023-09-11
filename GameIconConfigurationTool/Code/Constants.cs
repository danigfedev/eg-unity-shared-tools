using System.IO;
using SharedConstants = eg_unity_shared_tools.Utilities.Constants;

namespace eg_unity_shared_tools.GameIconConfigurationTool.Code
{
    public class Constants
    {
        public const string DefaultSettingsRelativePath = "eg-unity-shared-tools/GameIconConfigurationTool/Settings";
        public static readonly string CustomSettingsRelativePath = Path.Combine(SharedConstants.CustomSettingRelativesPath, "GameIconConfigurationTool");
        public const string DefaultSettingsFileName = "default_settings.json";
        public const string CustomSettingsFileName = "custom_settings.json";
    }
}