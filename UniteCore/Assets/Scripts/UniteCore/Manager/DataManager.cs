using UnityEngine;
using System.Collections.Generic;
using Unite;
using UnityEngine.UI;

namespace UniteCore
{
    public class DataManager : MonoBehaviour
    {
        // Variables
        #region Variables

        // Language and color
        public Dictionary<string, string> localizationDictionary = new Dictionary<string, string>();
        public Dictionary<string, string> colorSchemeDictionary = new Dictionary<string, string>();

        private List<JSon> localizationJSons = new List<JSon>();
        private List<JSon> colorSchemeJSons = new List<JSon>();
        private List<JSon> lightSchemeJSons = new List<JSon>();

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

        // Override
        #region Override

        void Awake()
        {
            Inititalize();
        }

        #endregion

        // Public
        #region Public

        public virtual void Inititalize()
        {
            LoadLanguageData();
            LoadColorSchemeData();
            LoadLightSchemeData();
        }

        // Langauge
        #region Language

        public string GetLocalizationOf(string key)
        {
            if (localizationDictionary.ContainsKey(key))
                return localizationDictionary[key].Replace("\\n", "\n");

            return key + " No localization found";
        }

        #endregion

        // Color Schemes
        #region Color Schemes

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

        // Internal
        #region Internal

        // Langauge
        #region Language

        internal void SetLanguage(Language language)
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

        internal bool LanguageExist(Language language)
        {
            foreach (JSon json in localizationJSons)
            {
                if (json.GetValue("localization") == language.ToString())
                    return true;
            }
            return false;
        }

        #endregion

        // Color Schemes
        #region Color Schemes

        internal void SetColorScheme(string colorScheme)
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

        internal bool ColorSchemeExist(string colorScheme)
        {
            foreach (JSon jSon in colorSchemeJSons)
            {
                if (jSon.GetValue("ID") == colorScheme)
                    return true;
            }
            return false;
        }

        #endregion

        // Light Schemes
        #region Light Schemes

        internal JSon LightSchemeExist(string lightScheme)
        {
            foreach (JSon jSon in lightSchemeJSons)
            {
                if (jSon.GetValue("ID") == lightScheme)
                    return jSon;
            }
            return null;
        }

        #endregion

        #endregion

        // Protected
        #region Protected

        // Langauge
        #region Language

        protected void LoadLanguageData()
        {
            TextAsset[] assets = Resources.LoadAll<TextAsset>("UniteCore/Data/Languages");
            foreach (TextAsset asset in assets)
            {
                if (asset)
                    localizationJSons.Add(new JSon(asset));
            }
        }

        protected void AddLoaclizationValues(JSon json)
        {
            foreach (KeyValuePair<string, string> entry in json.GetKeyValueValues())
                localizationDictionary.Add(entry.Key, entry.Value);
            foreach (JSon node in json.GetNodeValues())
                AddLoaclizationValues(json.GetNode(node.ID));
        }

        #endregion

        // Color Schemes
        #region Color Schemes

        protected void LoadColorSchemeData()
        {
            TextAsset[] assets = Resources.LoadAll<TextAsset>("UniteCore/Data/ColorSchemes");
            foreach (TextAsset asset in assets)
            {
                if (asset)
                    colorSchemeJSons.Add(new JSon(asset));
            }
        }

        protected void AddColorSchemeValues(JSon json)
        {
            foreach (KeyValuePair<string, string> entry in json.GetKeyValueValues())
                colorSchemeDictionary.Add(entry.Key, entry.Value);
            foreach (JSon node in json.GetNodeValues())
                AddColorSchemeValues(json.GetNode(node.ID));
        }

        #endregion

        // Light Schemes
        #region Light Schemes

        protected void LoadLightSchemeData()
        {
            TextAsset[] assets = Resources.LoadAll<TextAsset>("UniteCore/Data/LightingSchemes");
            foreach (TextAsset asset in assets)
            {
                if (asset)
                    lightSchemeJSons.Add(new JSon(asset));
            }
        }

        #endregion

        #endregion

    }
}