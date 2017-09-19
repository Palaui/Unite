using UnityEngine;
using UnityEngine.UI;

namespace Unite
{
    public class Toggler : MonoBehaviour
    {
        // Variables
        #region Variables

        public Image spriteImage;
        public Sprite baseSpriteImage;
        public Sprite endSpriteImage;

        public Image colorImage;
        public Sprite baseColorImage;
        public Sprite endolorImage;

        public Outline outline;
        public Color baseOutline;
        public Color endOutline;

        public Text label;
        public Color baseLabelColor;
        public Color endLabelColor;
        public string baseLabel;
        public string endLabel;

        public bool beginToggled;
        public bool resetOnEnable;

        private bool toggleState;

        #endregion

        // Override
        #region Override

        void Start()
        {
            if (!GetComponent<Button>())
            {
                Debug.LogError("Toggler: " + gameObject.name + " does not contain a button. Disabling");
                enabled = false;
                return;
            }

            GetComponent<Button>().onClick.AddListener(PlayToggle);
            if (!resetOnEnable)
            {
                toggleState = beginToggled;
                ApplyToggle();
            }
        }

        void OnEnable()
        {
            if (resetOnEnable)
            {
                toggleState = beginToggled;
                ApplyToggle();
            }
        }

        #endregion

        // Public
        #region Public

        public void PlayToggle()
        {
            toggleState = !toggleState;
            ApplyToggle();
        }

        #endregion

        // Private
        #region Private

        private void ApplyToggle()
        {
            if (spriteImage)
            {
                if (!toggleState)
                    spriteImage.sprite = baseSpriteImage;
                else
                    spriteImage.sprite = endSpriteImage;
            }

            if (label)
            {
                if (!toggleState)
                    label.text = baseLabel;
                else
                    label.text = endLabel;
            }
        }

        #endregion

    }
}
