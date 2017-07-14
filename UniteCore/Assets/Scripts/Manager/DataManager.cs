using UnityEngine;
using System.Collections.Generic;
using Unite;

internal class DataManager
{
    List<JSon> LanguageJSons = new List<JSon>();

    internal DataManager()
    {
        LoadLanguageData();
    }

    public void LoadLanguageData()
    {
        for (int i = 0; i < System.Enum.GetValues(typeof(Language)).Length; i++)
        {
            string path = "Scriptable/Lan_" + (Language)System.Enum.ToObject(typeof(Language), i);
            STextAsset sAsset = Resources.Load(path) as STextAsset;
            if (sAsset)
                LanguageJSons.Add(new JSon(sAsset.asset));
        }
    }

    public Dictionary<string, string> GetLocalizationJSon(Language language)
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        foreach (JSon jSon in LanguageJSons)
        {
            if (jSon.GetValue("localization") == language.ToString())
            {
                foreach (KeyValuePair<string, string> entry in jSon.GetNodeKeyValues("literals"))
                    dictionary.Add(entry.Key, entry.Value);
            }
        }
        return dictionary;
    }

    public bool LanguageExist(Language language)
    {
        foreach (JSon jSon in LanguageJSons)
        {
            if (jSon.GetValue("localization") == language.ToString())
                return true;
        }
        return false;
    }
}
