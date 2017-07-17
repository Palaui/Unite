using UnityEngine;

namespace Unite
{
    class Signal : MonoBehaviour
    {
        // Override
        #region Overide

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
