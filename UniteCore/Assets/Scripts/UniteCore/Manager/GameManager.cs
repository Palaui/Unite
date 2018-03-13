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
        private DebugSystemController debugSystem = null;

        private Language currentLanguage;
        private string currentColorScheme;
        private string currentLightScheme;

        #endregion

        // Properties
        #region Properties

        public static GameManager Instance { get { Assure(); return instance; } }

        public static DataManager DataManager { get { Assure(); return instance.dataManager; } }
        public static EventManager EventManager { get { Assure(); return instance.eventManager; } }

        public static SceneController SceneController { get { Assure(); return instance.sceneController; } }
        public static UIController UIController { get { Assure(); return instance.uiController; } }
        public static LightingSystemController LightingController { get { Assure(); return instance.lightingController; } }
        public static DebugSystemController DebugSystem { get { Assure(); return instance.debugSystem; } }


        public static Language CurrentLanguage
        {
            get { Assure(); return instance.currentLanguage; }
            set
            {
                if (DataManager.LanguageExist(value))
                {
                    Instance.currentLanguage = value;
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
                    Instance.currentColorScheme = value;
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
                    Instance.currentLightScheme = value;
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
            uiController.Assure();
            lightingController = GetComponentInChildren<LightingSystemController>();
            debugSystem = GetComponentInChildren<DebugSystemController>();
        }

        void Start()
        {
            if (Application.isPlaying)
            {
                Protocol.StartProtocol<DeviceConnectivityProtocol>();
                Protocol.StartProtocol<InternetConnectivityProtocol>();

                CurrentLanguage = Language.EN;
                CurrentColorScheme = "Unite";

                DynamicGraphics.Activate(14, 26);
                if (Application.isEditor)
                    DynamicGraphics.BeginDrawFPS();

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

        // Protected Static
        #region Protected Static

        protected static void Assure()
        {
            if (!instance)
                FindObjectOfType<GameManager>().Awake();
        }

        #endregion

    }
}