using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    public class InitialDeactivate : MonoBehaviour
    {
        // Variables
        #region Variables

        private static List<GameObject> gos = new List<GameObject>();
        private static bool hasBeenInitialized = false;

        public bool deactivateImmediate = false;

        #endregion

        // Override
        #region Override

        void Awake()
        {
            gos.Add(gameObject);
        }

        void Start()
        {
            if (!hasBeenInitialized)
            {
                foreach (GameObject go in gos)
                    go.SetActive(false);

                hasBeenInitialized = true;
            }
        }

        #endregion

    }
}
