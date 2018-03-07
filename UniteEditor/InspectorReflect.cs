using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;
using System;
using Unite;

namespace UniteEditor
{
    public class InspectorReflect
    {
        // Public
        #region Public

        public void DrawMethodsAsEnum(MonoBehaviour script, bool addCallingButton, ref string str)
        {
            string[] methods = Reflect.GetMethodsArray(script.GetType(), new string[] { });

            string comparer = str;
            int index;

            if (methods.Length == 0)
            {
                Debug.LogError("DrawMethodsAsEnum: Methods array is empty, try BuildMethodsArray first");
                return;
            }

            try { index = methods.Select((v, i) => new { Name = v, Index = i }).First(x => x.Name == comparer).Index; }
            catch { index = 0; }
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            str = methods[EditorGUILayout.Popup(index, methods)];
            if (addCallingButton)
            {
                if (GUILayout.Button("Call " + str))
                {
                    script.Invoke(str, 0);
                    EditorUtility.SetDirty(script);
                }
            }
        }

        public void DrawFieldsAsEnum(MonoBehaviour script, ref string str)
        {
            string[] fields = Reflect.GetFieldsArray(script.GetType(), new string[] { });

            string comparer = str;
            int index;

            if (fields.Length == 0)
            {
                Debug.LogError("DrawMethodsAsEnum: Fields array is empty, try BuildFieldsArray first");
                return;
            }

            try { index = fields.Select((v, i) => new { Name = v, Index = i }).First(x => x.Name == comparer).Index; }
            catch { index = 0; }
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            str = fields[EditorGUILayout.Popup(index, fields)];
        }

        #endregion

    }
}
