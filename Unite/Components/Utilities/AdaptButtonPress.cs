using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Unite
{
    class AdaptButtonPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        // Variables
        #region Variables

        public Outline outline;
        public Color outlineColor = Color.white;
        public Image image;
        public Color imageColor = Color.white;
        public Text text;
        public Color textColor = Color.white;

        private Button button;
        private Color outlineBaseColor;
        private Color imageBaseColor;
        private Color textBaseColor;

        private bool isValid;

        #endregion

        // Override
        #region Override

        void Start()
        {
            button = GetComponent<Button>();
            if (button)
            {
                if (outline)
                    outlineBaseColor = outline.effectColor;
                if (image)
                    imageBaseColor = image.color;
                if (text)
                    textBaseColor = text.color;
                isValid = true;
            }
            else
                enabled = false;
        }

        #endregion

        // Events
        #region Events

        public void OnPointerDown(PointerEventData eventData)
        {
            if (isValid)
            {
                if (outline)
                    outline.effectColor = outlineColor;
                if (image)
                    image.color = imageColor;
                if (text)
                    text.color = textColor;
            }

        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (isValid)
            {
                if (outline)
                    outline.effectColor = outlineBaseColor;
                if (image)
                    image.color = imageBaseColor;
                if (text)
                    text.color = textBaseColor;
            }
        }

        #endregion

    }
}
