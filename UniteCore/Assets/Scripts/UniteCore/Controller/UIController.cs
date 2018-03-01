using Unite;
using UnityEngine;

namespace UniteCore
{
    public class UIController : MonoBehaviour
    {
        // Variables
        #region Variables

        private UIOverlayController uiOverlayController = null;

        private bool hasBeenInitialized = false;
        private bool applicationFocus = true;

        #endregion

        // Properties
        #region Properties

        public UIOverlayController OverlayToolsController { get { Awake(); return uiOverlayController; } }

        public bool ApplicationFocus { get { return applicationFocus; } }

        #endregion

        // Override
        #region Override

        protected virtual void Awake()
        {
            if (!hasBeenInitialized)
            {
                hasBeenInitialized = true;

                // SubControllers
                uiOverlayController = transform.Find("OverlayCanvas").GetComponent<UIOverlayController>();

                // Panels
            }
        }

        void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
                DynamicListener.CallDelayed(true, () => { applicationFocus = true; });
            else
                applicationFocus = false;
        }

        #endregion

    }
}