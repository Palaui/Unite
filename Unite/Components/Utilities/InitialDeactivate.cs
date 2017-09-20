using UnityEngine;

namespace Unite
{
    public class InitialDeactivate : MonoBehaviour
    {
        // Variables
        #region Variables

        public bool hasBeenDeativated = false;

        #endregion

        // Override
        #region Override

        void Start()
        {
            if (!hasBeenDeativated)
                Deactivate();
        }

        #endregion

        // Private
        #region Private

        private void Deactivate()
        {
            foreach (InitialDeactivate script in GetComponentsInChildren<InitialDeactivate>())
            {
                if (script.transform.parent == transform)
                    script.Deactivate();
            }

            gameObject.SetActive(false);
            hasBeenDeativated = true;
        }

        #endregion

    }
}
