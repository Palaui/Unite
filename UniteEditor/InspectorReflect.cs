using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;
using System;

namespace UniteEditor
{
    public class InspectorReflect
    {
        // Variables
        #region Variables

        private string[] fields;
        private string[] methods;

        #endregion

        // Public
        #region Public

        public void BuildMethodsArray(Type type, string[] ignoredList)
        {
            if (ignoredList != null)
                methods = GetMethodsArray(type, ignoredList);
        }

        public void BuildFieldsArray(Type type, string[] ignoredList, Type fieldsType)
        {
            if (ignoredList != null)
                fields = GetFieldsArray(type, ignoredList, fieldsType);
        }

        public void DrawMethodsAsEnum(MonoBehaviour script, bool addCallingButton, ref string str)
        {
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

        // Public Static
        #region Public Static

        public static string[] GetMethodsArray(Type type, string[] ignoredList)
        {
            if (ignoredList != null)
            {
                string[] methodsArray = type
                    .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .Where(x => x.DeclaringType == type)
                    .Where(x => !ignoredList.Any(n => n == x.Name))
                    .Where(x => x.GetParameters().Length == 0)
                    .Select(x => x.Name)
                    .ToArray();

                return methodsArray;
            }

            Debug.Log("The list of ignored methods cannot be null, pass an empty array instead");
            return null;
        }

        public static MethodInfo[] GetAllMethods(Type type)
        {
            return type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        }

        public static string[] GetFieldsArray(Type type, string[] ignoredList, Type fieldsType)
        {
            if (ignoredList != null)
            {
                string[] fieldsArray = type
                    .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .Where(x => x.DeclaringType == type)
                    .Where(x => !ignoredList.Any(n => n == x.Name))
                    .Where(x => x.FieldType == fieldsType)
                    .Select(x => x.Name)
                    .ToArray();

                return fieldsArray;
            }

            Debug.Log("The list of ignored fields cannot be null, pass an empty array instead");
            return null;
        }

        public static FieldInfo[] GetAllFields(Type type)
        {
            return type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        }

        #endregion
    }
}
