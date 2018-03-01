using Unite;
using UnityEngine;

namespace UniteCore
{
    // Enums
    #region Enums

    public enum Language { EN, ES, DE }

    #endregion

    [ExecuteInEditMode]
    public class GameManager : MonoBehaviour
    {
        // Variables
        #region Variables

        private static GameManager instance;

        private DataManager dataManager;
        private EventManager eventManager;

        private SceneController sceneController = null;
        private UIController uiController = null;
        private LightingSystemController lightingController = null;

        private Language currentLanguage;
        private string currentColorScheme;
        private string currentLightScheme;

        #endregion

        // Properties
        #region Properties

        public static GameManager Instance { get { Assure(); return instance; } }

        public static DataManager DataManager { get { return instance.dataManager; } }
        public static EventManager EventManager { get { return instance.eventManager; } }

        public static SceneController SceneController { get { Assure(); return instance.sceneController; } }
        public static UIController UIController { get { Assure(); return instance.uiController; } }
        public static LightingSystemController LightingController { get { Assure(); return instance.lightingController; } }

        public static Language CurrentLanguage
        {
            get { Assure(); return instance.currentLanguage; }
            set
            {
                if (DataManager.LanguageExist(value))
                {
                    Assure();
                    instance.currentLanguage = value;
                    DataManager.SetLanguage(value);
                    EventManager.ChangeLanguage(value);
                }
            }
        }

        public static string CurrentColorScheme
        {
            get { Assure(); return instance.currentColorScheme; }
            set
            {
                if (DataManager.ColorSchemeExist(value))
                {
                    Assure();
                    instance.currentColorScheme = value;
                    DataManager.SetColorScheme(value);
                    EventManager.ChangeColorScheme(value);
                }
            }
        }

        public static string CurrentLightScheme
        {
            get { Assure(); return instance.currentLightScheme; }
            set
            {
                JSon json = DataManager.LightSchemeExist(value);
                if (json)
                {
                    Assure();
                    instance.currentLightScheme = value;
                    LightingController.ChangeLightScheme(LightBlendType.OutIn, LightChangeType.Immediate, json);
                }
            }
        }

        #endregion

        // Override
        #region Override

        void Awake()
        {
            instance = this;

            // Inititalize managers
            if (Application.isPlaying)
            {
                dataManager = GetComponent<DataManager>();
                eventManager = GetComponent<EventManager>();
            }

            // Initialize controllers
            sceneController = GetComponentInChildren<SceneController>();
            uiController = GetComponentInChildren<UIController>();
            lightingController = GetComponentInChildren<LightingSystemController>();
        }

        void Start()
        {
            if (Application.isPlaying)
            {
                Protocol.StartProtocol<DeviceConnectivityProtocol>();
                Protocol.StartProtocol<InternetConnectivityProtocol>();

                CurrentLanguage = Language.EN;
                CurrentColorScheme = "Unite";

                DynamicListener.CallDelayed(2, false, () => { CurrentLightScheme = "UniteCore"; });
                DynamicListener.CallDelayed(3.5f, false,
                    () => SetCurrentLightScheme(LightBlendType.OutIn, LightChangeType.Driven, "Test"));

                DynamicGraphicsModule.Activate(14, 26);
                if (Application.isEditor)
                    DynamicGraphicsModule.BeginDrawFPS();

                Protocol.StartProtocol<StartUpProtocol>();
            }
        }

        #endregion

        // Public Static
        #region Public Static

        public static void SetCurrentLightScheme(LightBlendType blendType, LightChangeType changeType, string scheme)
        {
            JSon json = DataManager.LightSchemeExist(scheme);
            if (json)
            {
                Assure();
                instance.currentLightScheme = scheme;
                LightingController.ChangeLightScheme(blendType, changeType, json);
            }
        }

        #endregion

        // Internal Static
        #region Internal Static

        internal static void Assure()
        {
            if (!instance)
                FindObjectOfType<GameManager>().Awake();
        }

        #endregion

    }
}