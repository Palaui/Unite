using System;
using System.Collections.Generic;
using System.Reflection;
using Unite;
using UnityEditor;
using UnityEngine;

namespace UniteEditor
{
    public class GameObjectEditionWindow : EditorWindow
    {
        public enum SearchType { SearchByName, SearchByTag }
        public enum UseType { UseAll, UseSelectedAndChildren, UseOnlySelected }
        public enum ChangeType { ChangeObject, AssureComponent }
        public SearchType searchType;
        public UseType useType;
        public ChangeType changeType;

        private List<GameObject> affectedGos = new List<GameObject>();
        private GameObject goToReplace;
        private string compToAdd;
        private string comparer = "";
        private bool byTag = false;

        private static Texture2D tex;

        [MenuItem("Window/GameObjects Editor")]
        public static void Init()
        {
            GameObjectEditionWindow window = (GameObjectEditionWindow)GetWindow(typeof(GameObjectEditionWindow));
            window.titleContent = new GUIContent("GameObjects Editor");

            tex = ImageUtility.BitmapToTexture2D(Properties.Resources.WindowBackground);
            window.Show();
        }

        void OnGUI()
        {
            EditorGUI.DrawPreviewTexture(new Rect(0, 0, 5000, 5000), tex);

            GUILayout.Label("Find and Replace with GameObjects", EditorStyles.boldLabel);
            comparer = EditorGUILayout.TextField("Search for: ", comparer);
            searchType = (SearchType)EditorGUILayout.EnumPopup("Search by: ", searchType);
            useType = (UseType)EditorGUILayout.EnumPopup("Use: ", useType);
            changeType = (ChangeType)EditorGUILayout.EnumPopup("Changing type: ", changeType);

            FindMatchingGameObjects();

            switch (changeType)
            {
                case ChangeType.ChangeObject:
                    goToReplace = EditorGUILayout.ObjectField("Replace with: ", goToReplace, typeof(GameObject), true) as GameObject;
                    if (GUILayout.Button("Find and Replace GameObjects"))
                    {
                        byTag = (searchType == SearchType.SearchByTag);
                        ChangeGameObjects();
                    }
                    break;
                case ChangeType.AssureComponent:
                    compToAdd = EditorGUILayout.TextField("Component to Add: ", compToAdd);
                    if (GUILayout.Button("Assure Components"))
                        AssureComponents(compToAdd);
                    break;
                default:
                    break;
            }

            GUILayout.Space(10);

            switch (changeType)
            {
                case ChangeType.ChangeObject:
                    if (affectedGos.Count > 0 && comparer != "")
                    {
                        GUILayout.BeginVertical();
                        string replaceText = "<select a prefab>";
                        if (goToReplace != null)
                            replaceText = goToReplace.name;

                        GUILayout.Label("Replace these with " + replaceText, EditorStyles.boldLabel);
                        GUILayout.Space(5);
                        foreach (GameObject go in affectedGos)
                        {
                            if ((!byTag && go.name.Contains(comparer)) || (byTag && go.tag.Contains(comparer)))
                                GUILayout.Label("\t" + go.name);
                        }
                        GUILayout.EndVertical();
                    }
                    break;
                case ChangeType.AssureComponent:
                    if (affectedGos.Count > 0 && comparer != "")
                    {
                        GUILayout.BeginVertical();
                        GUILayout.Label("Add this component to matching GameObjects", EditorStyles.boldLabel);
                        GUILayout.Space(5);
                        foreach (GameObject go in affectedGos)
                        {
                            if ((!byTag && go.name.Contains(comparer)) || (byTag && go.tag.Contains(comparer)))
                                GUILayout.Label("\t" + go.name);
                        }
                        GUILayout.EndVertical();
                    }
                    break;
                default:
                    break;
            }
        }

        private void FindMatchingGameObjects()
        {
            affectedGos.Clear();
            switch (useType)
            {
                case UseType.UseAll:
                    foreach (GameObject go in FindObjectsOfType<GameObject>())
                    {
                        if (go)
                        {
                            if ((!byTag && go.name.Contains(comparer)) || (byTag && go.tag.Contains(comparer)))
                                affectedGos.Add(go);
                        }
                    }
                    break;
                case UseType.UseSelectedAndChildren:
                    {
                        foreach (GameObject go in Selection.gameObjects)
                        {
                            if ((!byTag && go.name.Contains(comparer)) || (byTag && go.tag.Contains(comparer)))
                            {
                                if (!affectedGos.Contains(go))
                                    affectedGos.Add(go);

                                foreach (Transform child in go.transform)
                                {
                                    if (!affectedGos.Contains(child.gameObject))
                                        affectedGos.Add(child.gameObject);
                                }
                            }
                        }
                    }
                    break;
                case UseType.UseOnlySelected:
                    foreach (GameObject go in Selection.gameObjects)
                    {
                        if ((!byTag && go.name.Contains(comparer)) || (byTag && go.tag.Contains(comparer)))
                        {
                            if (!affectedGos.Contains(go))
                                affectedGos.Add(go);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void ChangeGameObjects()
        {
            foreach (GameObject go in affectedGos)
            {
                GameObject prefab = (GameObject)PrefabUtility.InstantiatePrefab(goToReplace);
                prefab.transform.SetParent(go.transform.parent);
                prefab.transform.position = go.transform.position;
                prefab.transform.rotation = go.transform.rotation;
                prefab.name = prefab.name.Replace("(Clone)", "");

                foreach (Transform child in go.transform)
                {
                    if (child.parent == go.transform)
                        child.SetParent(prefab.transform);
                }
            }

            // Destroy the reference GameObjects
            for (int i = affectedGos.Count - 1; i >= 0; i--)
            {
                DestroyImmediate(affectedGos[i]);
                affectedGos.RemoveAt(i);
            }
        }

        private void AssureComponents(string comp)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    foreach (string compName in comp.Split(' '))
                    {
                        if (compName == type.Name)
                        {
                            foreach (GameObject go in affectedGos)
                            {
                                if (!go.GetComponent(type))
                                    go.AddComponent(type);
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
}
