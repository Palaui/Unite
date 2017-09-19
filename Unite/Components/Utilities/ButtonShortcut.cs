using UnityEngine;
using UnityEngine.UI;

namespace Unite
{
    public class ButtonShortcut : MonoBehaviour
    {
        // Variables
        #region Variables

        public KeyCode key = KeyCode.None;

        #endregion

        // Override
        #region Override

        void Start()
        {
            if (!GetComponent<Button>())
                enabled = false;
        }

        void Update()
        {
            if (Input.GetKeyDown(key))
                GetComponent<Button>().onClick.Invoke();
        }

        #endregion

    }
}
