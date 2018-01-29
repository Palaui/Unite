using System;
using System.Collections;
using UnityEngine;

namespace Unite
{
    public class Localization
    {
        public static IEnumerator Translate(string targetLang, string sourceText, System.Action<string> result)
        {
            yield return Translate("auto", targetLang, sourceText, result);
        }

        public static IEnumerator Translate(string sourceLang, string targetLang, string sourceText, Action<string> result)
        {
            string url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl="
                + sourceLang + "&tl=" + targetLang + "&dt=t&q=" + WWW.EscapeURL(sourceText);

            WWW www = new WWW(url);
            yield return www;

            if (www.isDone)
            {
                if (string.IsNullOrEmpty(www.error))
                {
                    JSon json = JSon.GetFromString(www.text);
                    string translatedText = json.GetNodeKeys()[0];
                    result(translatedText);
                }
            }
        }
    }
}