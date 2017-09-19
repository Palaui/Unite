using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Unite
{
    public class AdaptButtonPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        // Variables
        #region Variables

        public Outline outline;
        public Color outlineColor = Color.white;
        public Image image;
        public Color imageColor = Color.white;
        public Text text;
        public Color textColor = Color.white;

        private Color outlineBaseColor;
        private Color imageBaseColor;
        private Color textBaseColor;

        private bool isValid;

        #endregion

        // Override
        #region Override

        void Start()
        {
            if (GetComponent<Button>() || GetComponent<Image>() || GetComponent<RawImage>())
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
            {
                Debug.LogError("AdaptButtonPress: " + gameObject.name + " does not contain a UI asociated. Disabling");
                enabled = false;
            }
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
