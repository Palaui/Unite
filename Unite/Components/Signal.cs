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
                key = value;
                Radar.AddElement(key, gameObject);
            }
        }


        #endregion

        // Override
        #region Override

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void BeforeSceneLoad()
        {
            foreach (Signal signal in FindObjectsOfType<Signal>())
                Radar.AddElement(signal.key, signal.gameObject);
        }

        void OnEnable()
        {
            Radar.AddElement(key, gameObject);
        }

        #endregion

    }
}
