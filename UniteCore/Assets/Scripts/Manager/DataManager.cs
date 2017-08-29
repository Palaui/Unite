using UnityEngine;
using System.Collections.Generic;
using Unite;
using System;

internal class DataManager
{
    // Variables
    #region Variables

    private Dictionary<string, string> localizationDictionary = new Dictionary<string, string>();
    private List<JSon> localizationJSons = new List<JSon>();

    #endregion

    // Override
    #region Override

    internal DataManager()
    {
        LoadLanguageData();
    }

    #endregion

    // Public
    #region Public

    public void LoadLanguageData()
    {
        for (int i = 0; i < Enum.GetValues(typeof(Language)).Length; i++)
        {
            string path = "Scriptable/Languages/Lan_" + (Language)System.Enum.ToObject(typeof(Language), i);
            STextAsset sAsset = Resources.Load(path) as STextAsset;
            if (sAsset)
                localizationJSons.Add(new JSon(sAsset.asset));
        }
    }

    public Dictionary<string, string> GetLocalizationJSon(Language language)
    {
        localizationDictionary.Clear();
        foreach (JSon json in localizationJSons)
        {
            if (json.GetValue("localization") == language.ToString())
                AddValues(json["literals"]);
        }

        return localizationDictionary;
    }

    private void AddValues(JSon json)
    {
        foreach (KeyValuePair<string, string> entry in json.GetKeyValueValues())
            localizationDictionary.Add(entry.Key, entry.Value);
        foreach (JSon node in json.GetNodeValues())
            AddValues(json.GetNode(node.ID));
    }

    public bool LanguageExist(Language language)
    {
        foreach (JSon jSon in localizationJSons)
        {
            if (jSon.GetValue("localization") == language.ToString())
                return true;
        }
        return false;
    }

    #endregion

}
