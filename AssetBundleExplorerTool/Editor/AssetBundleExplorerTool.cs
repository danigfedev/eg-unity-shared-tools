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
            UGUIUtils.DrawSpace(10);

            GUILayout.Label("Select an asset bundle file", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            _assetBundleFilePath = EditorGUILayout.TextField(_assetBundleFilePath);
            UGUIUtils.DrawFileBrowserButton("Pick an Asset Bundle", "", "",
                "...", SetAssetBundlePath, GUILayout.Width(25));

            EditorGUILayout.EndHorizontal();
            
            UGUIUtils.DrawSpace(10);

            var disableLoadButtonInteraction = string.IsNullOrWhiteSpace(_assetBundleFilePath);
            UGUIUtils.DrawButton("Load Asset Bundle", ListAssetBundleContents, disableLoadButtonInteraction);
            
            EditorGUILayout.BeginVertical(boxStyle);
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
            GUILayout.Label(_assetBundleContentList, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.EndVertical();

            UGUIUtils.DrawSpace(10);

            var disableExportButtonInteraction = string.IsNullOrWhiteSpace(_assetBundleContentList);
            UGUIUtils.DrawButton("Export contents to txt", ExportContentListToFile, disableExportButtonInteraction);
            
            Repaint();
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