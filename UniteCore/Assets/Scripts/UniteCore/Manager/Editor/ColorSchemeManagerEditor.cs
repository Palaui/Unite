using UnityEditor;
using UnityEngine;

namespace UniteCore
{
    [CustomEditor(typeof(ColorSchemeManager))]
    public class ColorSchemeManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ColorSchemeManager script = (ColorSchemeManager)target;

            DrawDefaultInspector();

            GUILayout.Space(8);
            if (GUILayout.Button("Generate New Palette"))
                script.EditorGenerate();
            GUILayout.Space(8);
            if (GUILayout.Button("Load Palette"))
                script.EditorLoad();
        }
    }
}