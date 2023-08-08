using UnityEngine;
using UnityEditor;
using System.IO;
using eg_unity_shared_tools.Code.Editor.Utilities;

namespace eg_unity_shared_tools.Code.Editor
{
    public class AssetBundleExplorerTool : EditorWindow
    {
        private string assetBundleFilePath = "";
        private string _assetBundleContentList = "";
        private int _numLines;
        private Vector2 scrollPosition;
        private int _scrollHeight = 250;

        [MenuItem("EspidiGames/Tools/AssetBundle Explorer Tool")]
        public static void ShowWindow()
        {
            var window = GetWindow<AssetBundleExplorerTool>("AssetBundle Tool");
            window.minSize = new Vector2(300, 300);
        }

        private void OnGUI()
        {
            GUILayout.Label("Select an asset bundle file", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            assetBundleFilePath = EditorGUILayout.TextField(assetBundleFilePath);
            
            UGUIUtils.DrawButton("...", OpenFileBrowser, GUILayout.Width(25));
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();

            UGUIUtils.DrawButton("List Bundle contents", ListAssetBundleContents);
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(_scrollHeight));
            
            EditorGUILayout.TextArea(_assetBundleContentList,
                GUILayout.Height(Mathf.Max(_scrollHeight, EditorGUIUtility.singleLineHeight * _numLines)));
            
            EditorGUILayout.EndScrollView();

            UGUIUtils.DrawButton("Export contents to txt", ExportContentListToFile);
            
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
                    _assetBundleContentList = "Error al cargar el Asset Bundle.";
                }
            }
            else
            {
                _assetBundleContentList = "Ruta de archivo no válida o no seleccionada.";
            }
        }
        
        private void ExportContentListToFile()
        {
            if (!string.IsNullOrEmpty(_assetBundleContentList))
            {
                string exportFilePath = EditorUtility.SaveFilePanel("Exportar lista de contenidos", "",
                    "AssetBundleContentList", "txt");

                if (!string.IsNullOrEmpty(exportFilePath))
                {
                    File.WriteAllText(exportFilePath, _assetBundleContentList);
                    Debug.Log("Lista de contenidos exportada correctamente a: " + exportFilePath);
                }
            }
        }
    }

}