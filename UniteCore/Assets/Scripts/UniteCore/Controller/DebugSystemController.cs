using UnityEngine;
using UnityEngine.UI;

namespace UniteCore
{
    public class DebugSystemController : MonoBehaviour
    {
        // Variables
        #region Variables

        private InputField inputField;
        private bool isConsoleEnabled;

        #endregion

        // Override
        #region Override

        void Awake()
        {
            inputField = GameManager.UIController.UIDebugController.GetComponentInChildren<InputField>();
        }

        void Start()
        {
            isConsoleEnabled = false;
            SetDebugVisibility(false);
        }

        void Update()
        {
            if (Application.isEditor || Debug.isDebugBuild)
            {
                // Desktop activation
                if (Input.GetKeyDown(KeyCode.F7))
                {
                    isConsoleEnabled = !isConsoleEnabled;
                    SetDebugVisibility(isConsoleEnabled);
                }

                // Touch activation
                if (Input.touchCount == 4)
                {
                    if (Input.touches[0].position.x < 400 && Input.touches[0].position.y < 400 &&
                        Input.touches[1].position.x < 400 && Input.touches[1].position.y > Screen.height - 400 &&
                        Input.touches[2].position.x > Screen.width - 400 && Input.touches[2].position.y > Screen.height - 400 &&
                        Input.touches[3].position.x > Screen.width - 400 && Input.touches[3].position.y < 400)
                    {
                        isConsoleEnabled = !isConsoleEnabled;
                        SetDebugVisibility(isConsoleEnabled);
                    }
                }
            }
        }

        #endregion

        // Properties
        #region Properties

        public bool ConsoleEnabled
        {
            get { return isConsoleEnabled; }
            internal set
            {
                if (value != isConsoleEnabled)
                    SetDebugVisibility(value);
            }
        }

        #endregion

        // Public
        #region Public

        public void CloseDebug()
        {
            isConsoleEnabled = false;
            SetDebugVisibility(false);
        }

        /// <summary> Gives an order to the SceneController to load or unload certain scene </summary>
        /// <param name="sceneName"> Scene name. </param>
        /// <param name="shouldBeLoaded"> If scene should be laoded or unloaded. </param>
        public void SetSceneLoadState(string sceneName, bool shouldBeLoaded)
        {
            if (shouldBeLoaded)
                GameManager.SceneController.LoadScene(sceneName);
            else
                GameManager.SceneController.UnloadScene(sceneName);
        }

        #endregion

        // Private
        #region Private

        /// <summary> Enables or Disables the debug system </summary>
        /// <param name="shouldBeVisible"></param>
        private void SetDebugVisibility(bool shouldBeVisible)
        {
            GameManager.UIController.UIDebugController.gameObject.SetActive(shouldBeVisible);

            if (shouldBeVisible)
            {
                GameManager.UIController.UIDebugController.CallConsoleUpdate("");
                inputField.text = "";
                inputField.ActivateInputField();
                inputField.Select();
            }
        }

        #endregion

    }
}