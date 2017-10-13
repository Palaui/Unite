using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Unite
{
    public class ButtonExtension : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // Variables
        #region Variables

        public Texture2D tex;
        public Vector2 place;
        public bool mantainOnPress = false;

        #endregion

        // Override
        #region Override

        void Start()
        {
            if (!GetComponent<Button>())
            {
                Debug.LogError("ButtonExtension: " + name + " does not contain a button. Disabling");
                enabled = false;
                return;
            }

            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        void OnDisable()
        {
            Mouse.ApplyDefaultCursor();
        }

        #endregion

        // Public
        #region Public

        public void OnClick()
        {
            if (!mantainOnPress)
                Mouse.ApplyDefaultCursor();
        }

        #endregion

        // Events
        #region Events

        public void OnPointerEnter(PointerEventData eventData)
        {
            Cursor.SetCursor(tex, place, CursorMode.Auto);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Mouse.ApplyDefaultCursor();
        }

        #endregion

    }
}
