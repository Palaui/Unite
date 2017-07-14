using UnityEngine;
using UnityEngine.UI;

public class Localizator : MonoBehaviour
{
    public string key;
    private Text textComp;
    private bool applicationIsQuitting;

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
