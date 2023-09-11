using UnityEditor;
using UnityEngine;

namespace eg_unity_shared_tools.Utilities.Editor
{
    public class CustomEditorStyles
    {
        public static GUIStyle CreateTextFieldStyle()
        {
            return CreateCustomStyle(EditorStyles.textField, Color.white);
        }

        public static GUIStyle CreateCustomStyle(GUIStyle style, Color textColor)
        {
            var boxStyle = new GUIStyle(style);
            boxStyle.normal.textColor = textColor;
            return boxStyle;
        }
    }
}