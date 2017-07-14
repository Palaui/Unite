using UnityEngine;
using UnityEngine.UI;

public class LocalizationComponent : MonoBehaviour
{
    public string key;
    Text textComp;
    bool applicationIsQuitting;

    void Awake()
    {
        GameManager.Instance.eventManager.changeLanguage += OnChangeLanguage;

        textComp = GetComponent<Text>();
    }

    private void OnChangeLanguage(Language language)
    {
        textComp.text = GameManager.Instance.languageManager.GetLocalization(key);
    }

    void OnDestroy()
    {
        if (!applicationIsQuitting)
            GameManager.Instance.eventManager.changeLanguage -= OnChangeLanguage;
    }


    void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }
}
