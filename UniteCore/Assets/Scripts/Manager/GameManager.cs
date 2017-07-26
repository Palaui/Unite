using Unite;
using UnityEngine;
using UnityEngine.SceneManagement;

// Enums
#region Enums

// Add all scenes of the game
public enum GameScene
{
    IntroScene,
    MainScene
}

// Add all languages available
public enum Language { EN, ES, DE }

#endregion

public class GameManager : Singleton<GameManager>
{
    // Variables
    #region Variables

    protected static GameManager GM = null;

    // Guarantee this will be always a singleton only - can't use the constructor!
    protected GameManager() { }

    internal DataManager dataManager;
    internal EventManager eventManager;
    internal LanguageManager languageManager;

    internal SceneController sceneController = null;
    internal UIController uiController = null;

    internal GameScene Scene { get; private set; }

    private Language currentLanguage;

    #endregion

    // Properties
    #region Properties

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

    #endregion

    // First Call
    #region First Call

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void BeforeSceneLoad()
    {
        GM = Instance;
        GM.gameObject.SetActive(true);
    }

    #endregion

    // Override
    #region Override

    void Awake()
    {
        dataManager = new DataManager();
        eventManager = new EventManager();
        languageManager = new LanguageManager();
    }

    Expression ex;
    void Start()
    {
        CurrentLanguage = Language.ES;
        DynamicGraphicsModule.Activate();
        DynamicGraphicsModule.BeginDrawFPS();

        ex = new Expression("-(1 / 4) * x ^ 2");
        ex.GetGraphicPoints(new Vector2(-200, 200), 10);
        //Expression ex = new Expression("x * 2 / 3");
    }

    void OnGUI()
    {
        ex.Draw(new Vector2(Screen.width / 2, Screen.height * 0.75f), new Vector2(0.25f, 10));
    }

    #endregion

    // Public
    #region Public

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

    public void ChangeToScene(GameScene newScene)
    {
        sceneController.SetControllerState(ControllerState.Exit);
        Scene = newScene;
        SceneManager.LoadScene(Scene.ToString());
    }

    #endregion

}
