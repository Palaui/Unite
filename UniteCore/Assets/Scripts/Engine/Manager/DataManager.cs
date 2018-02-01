using UnityEngine;
using System.Collections.Generic;
using Unite;
using UnityEngine.UI;

namespace UniteCore
{
    public class DataManager
    {
        // Variables
        #region Variables

        // Language and color
        public Dictionary<string, string> localizationDictionary = new Dictionary<string, string>();
        public Dictionary<string, string> colorSchemeDictionary = new Dictionary<string, string>();

        private List<JSon> localizationJSons = new List<JSon>();
        private List<JSon> colorSchemeJSons = new List<JSon>();

        private JSon currentLanguageJSon;

        // Volume
        private float volume = 1;
        private float unMutedVolume = 1;
        private bool isVolumeMuted = false;

        #endregion

        // Properties
        #region Properties

        public JSon CurrentLanguageJSon { get { return currentLanguageJSon; } }

        public float Volume { get { return volume; } }

        public float UnMutedVolume
        {
            get { return unMutedVolume; }
            set
            {
                volume = value;
                unMutedVolume = value;
                isVolumeMuted = false;
            }
        }

        public bool IsVolumeMuted
        {
            get { return isVolumeMuted; }
            set
            {
                isVolumeMuted = value;
                if (isVolumeMuted)
                    volume = 0;
                else
                    volume = unMutedVolume;
            }
        }

        #endregion

        // Public
        #region Public

        // Langauge
        #region Language

        public void LoadLanguageData()
        {
            TextAsset[] assets = Resources.LoadAll<TextAsset>("Engine/Data/Languages");
            foreach (TextAsset asset in assets)
            {
                if (asset)
                    localizationJSons.Add(new JSon(asset));
            }
        }

        public void SetLanguage(Language language)
        {
            localizationDictionary.Clear();
            foreach (JSon json in localizationJSons)
            {
                if (json.GetValue("localization") == language.ToString())
                {
                    AddLoaclizationValues(json["literals"]);
                    currentLanguageJSon = json;
                }
            }
        }

        private void AddLoaclizationValues(JSon json)
        {
            foreach (KeyValuePair<string, string> entry in json.GetKeyValueValues())
                localizationDictionary.Add(entry.Key, entry.Value);
            foreach (JSon node in json.GetNodeValues())
                AddLoaclizationValues(json.GetNode(node.ID));
        }

        public bool LanguageExist(Language language)
        {
            foreach (JSon json in localizationJSons)
            {
                if (json.GetValue("localization") == language.ToString())
                    return true;
            }
            return false;
        }

        public string GetLocalizationOf(string key)
        {
            if (localizationDictionary.ContainsKey(key))
                return localizationDictionary[key].Replace("\\n", "\n");

            return key + " No localization found";
        }

        #endregion

        // Color Schemes
        #region Color Schemes

        public void LoadColorSchemeData()
        {
            TextAsset[] assets = Resources.LoadAll<TextAsset>("Engine/Data/ColorSchemes");
            foreach (TextAsset asset in assets)
            {
                if (asset)
                    colorSchemeJSons.Add(new JSon(asset));
            }
        }

        public void SetColorScheme(string colorScheme)
        {
            colorSchemeDictionary.Clear();
            foreach (JSon json in colorSchemeJSons)
            {
                if (json.GetValue("ID") == colorScheme)
                {
                    AddColorSchemeValues(json["Colors"]);
                    AddColorSchemeValues(json["ColorBlocks"]);
                }
            }
        }

        private void AddColorSchemeValues(JSon json)
        {
            foreach (KeyValuePair<string, string> entry in json.GetKeyValueValues())
                colorSchemeDictionary.Add(entry.Key, entry.Value);
            foreach (JSon node in json.GetNodeValues())
                AddColorSchemeValues(json.GetNode(node.ID));
        }

        public bool ColorSchemeExist(string colorScheme)
        {
            foreach (JSon jSon in colorSchemeJSons)
            {
                if (jSon.GetValue("ID") == colorScheme)
                    return true;
            }
            return false;
        }

        public string GetColorStringOf(string key)
        {
            if (colorSchemeDictionary.ContainsKey(key))
                return colorSchemeDictionary[key];

            return key + " No Color found";
        }

        public Color GetColorOf(int index)
        {
            if (colorSchemeDictionary.ContainsKey("Color " + index))
                return Ext.GetColorFromString(colorSchemeDictionary["Color " + index], false);

            return Color.black;
        }

        public ColorBlock GetColorBlockOf(int index)
        {
            if (colorSchemeDictionary.ContainsKey("ColorBlock " + index))
                return Ext.GetColorBlockFromString(colorSchemeDictionary["ColorBlock " + index], false);

            return new ColorBlock();
        }

        #endregion

        #endregion

    }
}