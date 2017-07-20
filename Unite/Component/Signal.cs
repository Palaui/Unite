using UnityEngine;

namespace Unite
{
    class Signal : MonoBehaviour
    {
        // Variables
        #region Variables

        public string key = "";

        #endregion

        // Override
        #region Overide

        void OnEnable()
        {
            Radar.AddElement(key, gameObject);
        }

        void OnDisable()
        {
            Radar.RemoveElement(key);
        }

        #endregion
    }
}
