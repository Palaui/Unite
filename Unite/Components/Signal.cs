using UnityEngine;

namespace Unite
{
    [ExecuteInEditMode]
    class Signal : MonoBehaviour
    {
        // Variables
        #region Variables

        [SerializeField]
        private string key = "";

        #endregion

        // Properties
        #region Properties

        public string Key
        {
            get { return key; }
            set
            {
                Radar.RemoveElement(key);
                key = value;
                Radar.AddElement(key, gameObject);
            }
        }


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
