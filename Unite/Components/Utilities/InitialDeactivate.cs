using System.Collections;
using UnityEngine;

namespace Unite
{
    class InitialDeactivate : MonoBehaviour
    {
        // Variables
        #region Variables

        public bool deactivateImmediate = false;

        #endregion

        // Override
        #region Override

        void Awake()
        {
            if (deactivateImmediate)
                gameObject.SetActive(false);
        }

        void Start()
        {
            if (Time.fixedUnscaledTime < 1 && !deactivateImmediate)
                StartCoroutine(Deactivate());
        }

        #endregion

        // Evemts
        #region Evemts

        private IEnumerator Deactivate()
        {
            yield return new WaitForSeconds(0.6f - GetComponentsInParent<InitialDeactivate>(true).Length * 0.1f);
            gameObject.SetActive(false);
        }

        #endregion
    }
}
