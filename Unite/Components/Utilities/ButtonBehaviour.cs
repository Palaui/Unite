using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Unite
{
    public class ButtonBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // Variables
        #region Variables

        public Texture2D cursorTex;
        public Vector2 cursorImpactPlace;
        public bool mantainOnPress = false;

        private Texture2D cursor;

        #endregion

        // Override
        #region Override

        void Start()
        {
            if (GetComponent<Button>())
                GetComponent<Button>().onClick.AddListener(OnClick);
            else
            {
                Debug.LogError("Toggler: " + gameObject.name + " does not contain a button. Disabling");
                enabled = false;
                return;
            }

            cursor = cursorTex;
        }

        void OnDisable()
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        #endregion

        // Public
        #region Public

        public void OnClick()
        {
            if (!mantainOnPress)
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        #endregion

        // Events
        #region Events

        public void OnPointerEnter(PointerEventData eventData)
        {
            Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        #endregion

    }
}
