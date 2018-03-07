using Unite;
using UnityEngine;

namespace UniteCore
{
    public class UIController : MonoBehaviour
    {
        // Variables
        #region Variables

        protected UIOverlayController uiOverlayController = null;
        protected UIDebugController uiDebugController = null;

        protected bool hasBeenInitialized = false;
        protected bool applicationFocus = true;

        #endregion

        // Properties
        #region Properties

        public UIOverlayController UIOverlayController { get { Assure(); return uiOverlayController; } }
        public UIDebugController UIDebugController { get { Assure(); return uiDebugController; } }

        public bool ApplicationFocus { get { return applicationFocus; } }

        #endregion

        // Override
        #region Override

        void Awake()
        {
            if (!hasBeenInitialized)
                Assure();
        }

        void OnApplicationFocus(bool hasFocus)
        {
            OnFocus(hasFocus);
        }

        #endregion

        // Protected
        #region Protected

        /// <summary> Makes sure this class is initialized, even iun editor. </summary>
        protected internal virtual void Assure()
        {
            if (!hasBeenInitialized)
            {
                hasBeenInitialized = true;

                // SubControllers
                uiOverlayController = transform.Find("OverlayCanvas").GetComponent<UIOverlayController>();
                uiDebugController = GetComponentInChildren<UIDebugController>();
            }
        }

        /// <summary> Detects whenever this application is the focused application in a computer. </summary>
        protected void OnFocus(bool hasFocus)
        {
            if (hasFocus)
                DynamicListener.CallDelayed(true, () => { applicationFocus = true; });
            else
                applicationFocus = false;
        }

        #endregion

    }
}