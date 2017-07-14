using System.Collections.Generic;
using UnityEngine;

public class LanguageManager
{
    // Variables
    #region Variables

    private Dictionary<string, string> localizationDictionary = new Dictionary<string, string>();

    #endregion

    // Public
    #region Public

    public void SetLanguage(Language language)
    {
        localizationDictionary.Clear();

        Dictionary<string, string> dictionary = GameManager.Instance.dataManager.GetLocalizationJSon(language);

        foreach (Localizator comp in Object.FindObjectsOfType<Localizator>())
        {
            if (comp)
            {
                if (dictionary.ContainsKey(comp.key))
                    localizationDictionary.Add(comp.key, dictionary[comp.key]);
                else if (comp.key == "")
                {
                    Debug.LogError("Localization key is empty:  " + "GameObject name is " +
                        comp.gameObject.name + ", Parent name is " + comp.transform.parent.name);
                }
                else
                    Debug.LogError("Localization key " + comp.key + " Was not found:  " + "GameObject name is " +
                        comp.gameObject.name + ", Parent name is " + comp.transform.parent.name);
            }
        }
    }

    public string GetLocalization(string key)
    {
        if (localizationDictionary.ContainsKey(key))
            return localizationDictionary[key];

        return key + " No localization found";
    }

    #endregion

}
