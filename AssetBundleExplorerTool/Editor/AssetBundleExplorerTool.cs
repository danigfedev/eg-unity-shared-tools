using System.IO;
using System.Text;
using eg_unity_shared_tools.Utilities;
using eg_unity_shared_tools.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace eg_unity_shared_tools.AssetBundleExplorerTool.Editor
{
    public class AssetBundleExplorerTool : EditorWindow
    {
        private static Vector2 minSize = new Vector2(300, 320);
        
        private string _assetBundleFilePath = "";
        private string _assetBundleContentList = "";
        private int _numLines;
        private Vector2 scrollPosition;
        private int _scrollHeight = 250;
        private GUIStyle boxStyle;

        [MenuItem(Constants.BaseMenu + Constants.ToolsMenu + "AssetBundle Explorer Tool")]
        public static void ShowWindow()
        {
            WindowFactory.CreateWindow<AssetBundleExplorerTool>("AssetBundle Explorer", minSize);
        }
        
        private void OnEnable()
        {
            boxStyle = CustomEditorStyles.CreateTextFieldStyle();
        }

        private void OnGUI()
        {
            DrawAssetBundleFilePickerSection();
            DrawLoadBundleButton();
            DrawBundleContentsSection();
            DrawSaveButtonSection();

            Repaint();
        }

        private void DrawAssetBundleFilePickerSection()
        {
            GUIUtils.DrawSpace(10);
            GUIUtils.DrawLabel("Select an asset bundle file", EditorStyles.boldLabel);
            GUIUtils.HorizontalLayout(false, DrawTextField, DrawFilePickerButton);

            void DrawTextField()
            {
                _assetBundleFilePath = EditorGUILayout.TextField(_assetBundleFilePath);
            }

            void DrawFilePickerButton()
            {
                GUIUtils.DrawFileBrowserButton("Pick an Asset Bundle", "", "",
                    "...", SetAssetBundlePath, GUILayout.Width(25));
            }
        }
        
        private void DrawLoadBundleButton()
        {
            var disableLoadButtonInteraction = string.IsNullOrWhiteSpace(_assetBundleFilePath);
            GUIUtils.DrawSpace(10);
            GUIUtils.DrawButton("Load Asset Bundle", ListAssetBundleContents, disableLoadButtonInteraction);
        }

        private void DrawBundleContentsSection()
        {
            GUIUtils.DrawSpace(10);
            GUIUtils.DrawLabel("Asset bundle contents:", EditorStyles.boldLabel);
            GUIUtils.VerticalLayout(boxStyle, false, DrawScrollView);

            void DrawScrollView()
            {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
                GUILayout.Label(_assetBundleContentList, GUILayout.ExpandHeight(true));
                EditorGUILayout.EndScrollView();
            }
        }
        
        private void DrawSaveButtonSection()
        {
            var disableExportButtonInteraction = string.IsNullOrWhiteSpace(_assetBundleContentList);
            GUIUtils.DrawSpace(10);
            GUIUtils.DrawButton("Export contents to txt", ExportContentListToFile, disableExportButtonInteraction);
        }
        
        private void SetAssetBundlePath(string filePath)
        {
            _assetBundleFilePath = filePath;
        }

        private void ListAssetBundleContents()
        {
            _numLines = 0;
            
            if (!string.IsNullOrEmpty(_assetBundleFilePath) && File.Exists(_assetBundleFilePath))
            {
                AssetBundle assetBundle = AssetBundle.LoadFromFile(_assetBundleFilePath);
                if (assetBundle != null)
                {
                    string[] assetNames = assetBundle.GetAllAssetNames();
                    _assetBundleContentList = string.Join("\n", assetNames);
                    _numLines = assetNames.Length;
                    
                    assetBundle.Unload(false);
                }
                else
                {
                    _assetBundleContentList = "Error loading the Asset Bundle.";
                }
            }
            else
            {
                _assetBundleContentList = "Invalid or empty filepath";
            }
        }
        
        private void ExportContentListToFile()
        {
            if (!string.IsNullOrEmpty(_assetBundleContentList))
            {
                string exportFilePath = EditorUtility.SaveFilePanel("Export bundle contents", "",
                    "AssetBundleContentList", "txt");

                if (!string.IsNullOrEmpty(exportFilePath))
                {
                    var output = BuildOutput();
                    File.WriteAllText(exportFilePath, output);
                }
            }
        }

        private string BuildOutput()
        {
            var bundleName = Path.GetFileNameWithoutExtension(_assetBundleFilePath);
            var stringBuilder = new StringBuilder();
                    
            stringBuilder.AppendLine($"List of contents of bundle: {bundleName}");
            stringBuilder.AppendLine($"Bundle path: {_assetBundleFilePath}");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("====================================================================================");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(_assetBundleContentList);

            return stringBuilder.ToString();
        }
    }
}