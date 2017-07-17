using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UniteEditor
{
    class ObjectEditionUtility
    {
        public class GameObjectEditionWindow : EditorWindow
        {
            public enum SearchType { SearchByName = 0, SearchByTag = 1 }
            public SearchType searchType;

            private GameObject replace;
            private string find = "";
            private bool byTag = false;

            [MenuItem("Edit/GameObjects Editor")]
            public static void Init()
            {
                GameObjectEditionWindow window = (GameObjectEditionWindow)GetWindow(typeof(GameObjectEditionWindow));
                window.titleContent = new GUIContent("GameObjects Editor");
                window.Show();
            }

            void OnGUI()
            {
                GUILayout.Label("Find and Replace with GameObjects", EditorStyles.boldLabel);
                GUILayout.Label("Note: Undo is not currently supported. Save your scene first!");
                find = EditorGUILayout.TextField("Search for: ", find);
                searchType = (SearchType)EditorGUILayout.EnumPopup("Search by: ", searchType);
                replace = EditorGUILayout.ObjectField("Replace with: ", replace, typeof(GameObject), true) as GameObject;
                if (GUILayout.Button("Find and Replace GameObjects"))
                {
                    byTag = searchType == SearchType.SearchByTag;
                    DoFindReplaceObjects(find, replace, byTag);
                }
                GUILayout.Space(10);
                if (Selection.transforms.Length > 0 && find != "")
                {
                    GUILayout.BeginVertical();
                    string replaceText = "<select a prefab>";
                    if (replace != null)
                        replaceText = replace.name;

                    GUILayout.Label("Replace these with " + replaceText, EditorStyles.boldLabel);
                    foreach (Transform t in Selection.transforms)
                    {
                        if ((!byTag && t.name.Contains(find)) || (byTag && t.tag.Contains(find)))
                            GUILayout.Label(t.gameObject.name);
                    }
                    GUILayout.EndVertical();
                }
            }

            private void DoFindReplaceObjects(string find, GameObject replace, bool byTag)
            {
                List<GameObject> selectAfterwards = new List<GameObject>();
                if (Selection.gameObjects.Length > 0)
                {
                    Undo.RecordObjects(Selection.gameObjects, "Find Replace Objects");
                    foreach (GameObject selectedGameObject in Selection.gameObjects)
                    {
                        if ((!byTag && selectedGameObject.name.Contains(find)) || (byTag && selectedGameObject.tag.Contains(find)))
                        {
                            GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(replace);
                            newObj.transform.SetParent(selectedGameObject.transform.parent);
                            newObj.transform.position = selectedGameObject.transform.position;
                            newObj.transform.rotation = selectedGameObject.transform.rotation;
                            newObj.name = newObj.name.Replace("(Clone)", "");
                            selectAfterwards.Add(newObj);
                            DestroyImmediate(selectedGameObject);
                        }
                    }
                }
                else
                    Debug.LogWarning("You must select one or more objects.");

                Selection.activeGameObject = null;
                Selection.objects = selectAfterwards.ToArray();
            }
        }
    }
}
