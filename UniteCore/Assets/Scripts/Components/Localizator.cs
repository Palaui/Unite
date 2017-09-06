using UnityEngine;
using UnityEngine.UI;

public class Localizator : MonoBehaviour
{
    // Variables
    #region Variables

    public string key;

    private Text textComp;
    private bool applicationIsQuitting;

    #endregion

    // Override
    #region Override

    void Awake()
    {
        GameManager.Instance.eventManager.changeLanguage += OnChangeLanguage;
        textComp = GetComponent<Text>();
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

    #endregion

    // Events
    #region Events

    private void OnChangeLanguage(Language language)
    {
        if (!applicationIsQuitting)
            textComp.text = GameManager.Instance.languageManager.GetLocalization(key);
    }

    #endregion

}
