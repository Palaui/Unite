using UnityEditor;
using UnityEngine;

namespace UniteCore
{
    [CustomEditor(typeof(LightingSchemeManager))]
    public class LightingSchemeManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            LightingSchemeManager script = (LightingSchemeManager)target;

            DrawDefaultInspector();

            GUILayout.Space(8);
            if (GUILayout.Button("Generate New Scheme"))
                script.Generate();
            GUILayout.Space(8);
            if (GUILayout.Button("Load Scheme"))
                script.Load();
            GUILayout.Space(8);
            if (GUILayout.Button("Display Scheme"))
                script.Display();
        }
    }
}