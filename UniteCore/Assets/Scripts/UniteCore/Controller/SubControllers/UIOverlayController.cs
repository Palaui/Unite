using Unite;
using UnityEngine;
using UnityEngine.UI;

namespace UniteCore
{
    public class UIOverlayController : MonoBehaviour
    {
        // Variables
        #region Variables

        private CanvasGroup canvasGroup;
        private Image background;
        private Image splashImage;

        #endregion

        // Override
        #region Override

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            background = transform.Find("Background").GetComponent<Image>();
            splashImage = background.transform.Find("Splash").GetComponent<Image>();
            gameObject.SetActive(false);
        }

        #endregion

        // Public
        #region Public

        public void ActivateSplash(Texture2D tex)
        {
            gameObject.SetActive(true);
            if (tex)
                splashImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one / 2);
            else
                splashImage.sprite = Sprite.Create(Texture2D.blackTexture, new Rect(0, 0, 1, 1), Vector2.one / 2);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public void DeactivateFading(float fadeTime = 1)
        {
            DynamicListener.Timeline(1, (float alpha) => { canvasGroup.alpha = Mathf.Lerp(0, 1, 1 - alpha); },
                () => { gameObject.SetActive(false); });
        }

        #endregion

    }
}