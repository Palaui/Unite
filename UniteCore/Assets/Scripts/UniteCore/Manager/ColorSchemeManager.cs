using System.Collections.Generic;
using System.IO;
using Unite;
using UnityEngine;
using UnityEngine.UI;

namespace UniteCore
{
    public class ColorSchemeManager : MonoBehaviour
    {
        // Variables
        #region Variables

        [Header("Creation of a new palette")]
        public List<Color> colorPalette;
        public List<ColorBlock> colorBlockPalette;
        public string paletteName;

        #endregion

        // Public
        #region Public

        public void EditorGenerate() { if (!Application.isPlaying) Generate(); }

        public void EditorLoad() { if (!Application.isPlaying) Load(); }

        #endregion

        // Internal
        #region Internal

        /// <summary> Generate a new palette, should be called using the editor tool at (Core -> ColorScheme) </summary>
        internal void Generate()
        {
            if (paletteName == "")
            {
                Debug.LogError("ColorSchemeManager Generate: PaletteName can not be empty, Aborting");
                return;
            }

            TextAsset textAsset = Resources.Load("UniteCore/Data/ColorSchemes/" + paletteName + ".json") as TextAsset;

#if UNITY_EDITOR

            if (!textAsset)
            {
                File.WriteAllText(Application.dataPath + "/Resources/UniteCore/Data/ColorSchemes/" + paletteName + ".json", " ");
                UnityEditor.AssetDatabase.Refresh();
            }

#endif

            JSon schemeJson = new JSon("Assets/Resources/UniteCore/Data/ColorSchemes/" + paletteName);
            schemeJson.RemoveAll();
            schemeJson.AddValue("ID", paletteName);
            schemeJson.AddNode("Colors");
            schemeJson.AddNode("ColorBlocks");

            for (int i = 0; i < colorPalette.Count; i++)
            {
                string color = colorPalette[i].r.ToString("F3") + ", " + colorPalette[i].g.ToString("F3") + ", " +
                    colorPalette[i].b.ToString("F3") + ", " + colorPalette[i].a.ToString("F3");
                schemeJson.AddValue("Colors", "Color " + (i + 1).ToString(), color);
            }
            for (int i = 0; i < colorBlockPalette.Count; i++)
            {
                string color = colorBlockPalette[i].normalColor.r.ToString("F3") + ", " +
                    colorBlockPalette[i].normalColor.g.ToString("F3") + ", " +
                    colorBlockPalette[i].normalColor.b.ToString("F3") + ", " +
                    colorBlockPalette[i].normalColor.a.ToString("F3") +
                    ", " + colorBlockPalette[i].highlightedColor.r.ToString("F3") + ", " +
                    colorBlockPalette[i].highlightedColor.g.ToString("F3") + ", " +
                    colorBlockPalette[i].highlightedColor.b.ToString("F3") + ", " +
                    colorBlockPalette[i].highlightedColor.a.ToString("F3") +
                    ", " + colorBlockPalette[i].pressedColor.r.ToString("F3") + ", " +
                    colorBlockPalette[i].pressedColor.g.ToString("F3") + ", " +
                    colorBlockPalette[i].pressedColor.b.ToString("F3") + ", " +
                    colorBlockPalette[i].pressedColor.a.ToString("F3") +
                    ", " + colorBlockPalette[i].disabledColor.r.ToString("F3") + ", " +
                    colorBlockPalette[i].disabledColor.g.ToString("F3") + ", " +
                    colorBlockPalette[i].disabledColor.b.ToString("F3") + ", " +
                    colorBlockPalette[i].disabledColor.a.ToString("F3");
                schemeJson.AddValue("ColorBlocks", "ColorBlock " + (i + 1).ToString(), color);
            }

            schemeJson.Rewrite();
        }

        /// <summary> Loads a palette to inspector, should be called using the editor tool at (Core -> ColorScheme) </summary>
        internal void Load()
        {
            JSon schemeJson = new JSon("Assets/Resources/UniteCore/Data/ColorSchemes/" + paletteName);

            colorPalette.Clear();
            colorBlockPalette.Clear();

            if (schemeJson != null)
            {
                foreach (string key in schemeJson["Colors"].GetValueKeys())
                    colorPalette.Add(schemeJson["Colors"].GetColorValue(key, false));
                foreach (string key in schemeJson["ColorBlocks"].GetValueKeys())
                    colorBlockPalette.Add(schemeJson["ColorBlocks"].GetColorBlockValue(key, false));
            }
        }

        #endregion

    }
}