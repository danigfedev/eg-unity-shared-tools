using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using eg_unity_shared_tools.Code.Editor.Utilities;

namespace eg_unity_shared_tools.Code.Editor
{
    public class AssetBundleExplorerTool : EditorWindow
    {
        private static Vector2 minSize = new Vector2(300, 320);
        
        private string assetBundleFilePath = "";
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
            EditorGUILayout.Space(10);
            
            GUILayout.Label("Select an asset bundle file", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            assetBundleFilePath = EditorGUILayout.TextField(assetBundleFilePath);
            UGUIUtils.DrawButton("...", OpenFileBrowser, GUILayout.Width(25));
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(10);

            var shouldDisableLoadButton = string.IsNullOrWhiteSpace(assetBundleFilePath);
            UGUIUtils.DrawLockableButton("Load Asset Bundle", ListAssetBundleContents, shouldDisableLoadButton);
            
            EditorGUILayout.BeginVertical(boxStyle);
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
            GUILayout.Label(_assetBundleContentList, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            var shouldDisableExportButton = string.IsNullOrWhiteSpace(_assetBundleContentList);
            UGUIUtils.DrawLockableButton("Export contents to txt", ExportContentListToFile, shouldDisableExportButton);
            
            Repaint();
        }

        private void OpenFileBrowser() // TODO This could be defined in utils class too
        {
            assetBundleFilePath = EditorUtility.OpenFilePanel("Pick an Asset Bundle", "", "");
        }
        
        private void ListAssetBundleContents()
        {
            _numLines = 0;
            
            if (!string.IsNullOrEmpty(assetBundleFilePath) && File.Exists(assetBundleFilePath))
            {
                AssetBundle assetBundle = AssetBundle.LoadFromFile(assetBundleFilePath);
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
            var bundleName = Path.GetFileNameWithoutExtension(assetBundleFilePath);
            var stringBuilder = new StringBuilder();
                    
            stringBuilder.AppendLine($"List of contents of bundle: {bundleName}");
            stringBuilder.AppendLine($"Bundle path: {assetBundleFilePath}");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("====================================================================================");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(_assetBundleContentList);

            return stringBuilder.ToString();
        }
    }

}