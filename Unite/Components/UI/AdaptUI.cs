using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Unite
{
    public class AdaptUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        // Enums
        #region Enums

        private enum AdaptCondition { Hover, Press, HoverAndPress }

        #endregion

        // Variables
        #region Variables

        [SerializeField]
        private AdaptCondition adaptCondition;

        public List<Outline> outlines;
        public List<Color> outlineColors;
        public List<Image> images;
        public List<Color> imageColors;
        public List<Text> texts;
        public List<Color> textColors;

        private List<Color> outlineBaseColors = new List<Color>();
        private List<Color> imageBaseColors = new List<Color>();
        private List<Color> textBaseColors = new List<Color>();

        #endregion

        // Override
        #region Override

        void Start()
        {
            if (outlineColors.Count != outlines.Count)
            {
                Debug.LogError("AdaptUI " + name + " There are not the same number of outlines than outline colors, Disabling");
                enabled = false;
                return;
            }
            if (imageColors.Count != images.Count)
            {
                Debug.LogError("AdaptUI " + name + " There are not the same number of images than image colors, Disabling");
                enabled = false;
                return;
            }
            if (textColors.Count != texts.Count)
            {
                Debug.LogError("AdaptUI " + name + " There are not the same number of texts than text colors, Disabling");
                enabled = false;
                return;
            }

            foreach (Outline outline in outlines)
                outlineBaseColors.Add(outline.effectColor);
            foreach (Image image in images)
                imageBaseColors.Add(image.color);
            foreach (Text text in texts)
                textBaseColors.Add(text.color);
        }

        void OnEnable()
        {
            Deactivate();
        }

        #endregion

        // Private
        #region Private

        private void Activate()
        {
            for (int i = 0; i < outlines.Count; i++)
                outlines[i].effectColor = outlineColors[i];
            for (int i = 0; i < images.Count; i++)
                images[i].color = imageColors[i];
            for (int i = 0; i < texts.Count; i++)
                texts[i].color = textColors[i];
        }

        private void Deactivate()
        {
            for (int i = 0; i < outlines.Count; i++)
                outlines[i].effectColor = outlineBaseColors[i];
            for (int i = 0; i < images.Count; i++)
                images[i].color = imageBaseColors[i];
            for (int i = 0; i < texts.Count; i++)
                texts[i].color = textBaseColors[i];
        }

        #endregion

        // Events
        #region Events

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (adaptCondition != AdaptCondition.Press)
                Activate();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (adaptCondition != AdaptCondition.Press)
                Deactivate();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (adaptCondition == AdaptCondition.Press)
                Activate();
            if (adaptCondition == AdaptCondition.Hover)
                Deactivate();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (adaptCondition == AdaptCondition.Press)
                Deactivate();
            if (adaptCondition == AdaptCondition.Hover)
                Activate();
        }

        #endregion

    }
}
