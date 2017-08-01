using UnityEngine;
using UnityEditor;

namespace UniteEditor
{
    public class GroupUtility
    {
        [MenuItem("Edit/Group %g")]
        public static void Group()
        {
            if (Selection.transforms.Length > 0)
            {
                if (!IsGroup())
                {
                    GameObject group = new GameObject("Group");

                    Vector3 pivotPosition = Vector3.zero;
                    foreach (Transform tr in Selection.transforms)
                        pivotPosition += tr.transform.position;
                    pivotPosition /= Selection.transforms.Length;
                    group.transform.position = pivotPosition;

                    Undo.RegisterCreatedObjectUndo(group, "Group");
                    foreach (GameObject s in Selection.gameObjects)
                        Undo.SetTransformParent(s.transform, group.transform, "Group");

                    Selection.activeGameObject = group;
                }
                else
                {
                    while (Selection.transforms[0].childCount > 0)
                    {
                        foreach (Transform child in Selection.transforms[0])
                            Undo.SetTransformParent(child.transform, null, "UnGroup");
                    }
                    Undo.DestroyObjectImmediate(Selection.transforms[0].gameObject);
                }
            }
            else
                Debug.LogWarning("You must select one or more objects.");
        }

        private static bool IsGroup()
        {
            if (Selection.transforms.Length > 1)
                return false;

            if (Selection.transforms[0].GetComponents<Component>().Length <= 1)
                return true;

            return false;
        }
    }
}