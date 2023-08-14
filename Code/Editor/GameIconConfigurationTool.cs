using eg_unity_shared_tools.Code.Editor.Utilities;
using UnityEditor;

namespace eg_unity_shared_tools.Code.Editor
{
    public class GameIconConfigurationTool: EditorWindow
    {
        private int _selectedTabIndex = 0;

        private string[] _tabNames =
        {
            "Set Project's Icon",
            "Import Icon",
            "Settings"
        };
        
        [MenuItem(Constants.BaseMenu + Constants.ToolsMenu + nameof(GameIconConfigurationTool))]
        public static void ShowWindow()
        {
            var window = WindowFactory.CreateWindow<GameIconConfigurationTool>("title");
        }

        private void OnGUI()
        {
            _selectedTabIndex = UGUIUtils.DrawTabs(_selectedTabIndex, _tabNames);
        }
    }
}