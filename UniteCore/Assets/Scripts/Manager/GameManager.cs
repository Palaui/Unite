using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Add all scenes of the game
public enum GameScene
{
    IntroScene,
    MainScene
}

// Add all languages available
public enum Language { EN, ES, DE }

public class GameManager : Singleton<GameManager>
{
    protected static GameManager GM = null;

    // Guarantee this will be always a singleton only - can't use the constructor!
    protected GameManager() { }

    internal DataManager dataManager;
    internal EventManager eventManager;
    internal LanguageManager languageManager;

    internal SceneController sceneController = null;
    internal UIController uiController = null;

    internal GameScene Scene { get; private set; }

    Language currentLanguage;


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void BeforeSceneLoad()
    {
        GM = GameManager.Instance;
        GM.gameObject.SetActive(true);
    }

    // Awake is always called before any Start functions
    void Awake()
    {
        Debug.Log("Awake : " + GetType().Name);

        dataManager = new DataManager();
        eventManager = new EventManager();
        languageManager = new LanguageManager();
    }

    public void SetControllerToInitializeScene<T>(T component)
    {
        if (sceneController && uiController == null)
            CurrentLanguage = currentLanguage;

        if (component.GetType().BaseType == typeof(SceneController))
            sceneController = component as SceneController;

        if (component.GetType().BaseType == typeof(UIController))
            uiController = component as UIController;

        if (sceneController && uiController != null)
        {
            sceneController.SetControllerState(ControllerState.Initialize);
            sceneController.SetControllerState(ControllerState.InScene);
        }
    }

    // GM Start method ocurrs after Initialize controllers
    void Start()
    {
        Debug.Log("Start : " + GetType().Name);

        CurrentLanguage = Language.ES;
    }

    public void ChangeToScene(GameScene newScene)
    {
        Debug.Log("LoadNewScene :: " + newScene);

        sceneController.SetControllerState(ControllerState.Exit);

        Scene = newScene;

        SceneManager.LoadScene(Scene.ToString());
    }

    public Language CurrentLanguage
    {
        get { return currentLanguage; }
        set
        {
            currentLanguage = value;
            if (dataManager.LanguageExist(currentLanguage))
            {
                languageManager.SetLanguage(currentLanguage);
                eventManager.ChangeLanguage(currentLanguage);
            }
        }
    }
}
