using System.IO;
using UnityEditor;
using UnityEngine;

namespace UniteEditor
{
    public static class AssetCreator
    {
        public static void CreateTextAsset(string resourcesPath, string extension)
        {
            AssetDatabase.CreateAsset(new TextAsset(), "Assets/Resources/" + resourcesPath + "." + extension);
            File.WriteAllText(Application.dataPath + "/Resources/" + resourcesPath, " ");
        }
    }
}
