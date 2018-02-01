using UnityEngine;

namespace UniteCore
{
    public class LightingSystemController : MonoBehaviour
    {
        // Variables
        #region Variables

        public int initialPreset = 1;

        #endregion

        // Override
        #region Override

        void Awake()
        {
            SetLightingSystem(initialPreset);
        }

        #endregion

        // Public
        #region Public

        public void SetLightingSystem(int preset)
        {
            foreach (Transform child in transform)
            {
                if (child.parent == transform)
                    child.gameObject.SetActive(child.name == "Preset " + preset);
            }
        }

        #endregion

    }
}