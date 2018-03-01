using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UniteCore
{
    public class ColorScheme : MonoBehaviour
    {
        // Variables
        #region Variables

        public List<Text> texts = new List<Text>();
        public List<Image> images = new List<Image>();
        public List<Button> buttons = new List<Button>();

        public int colorIndex;

        #endregion

        // Override
        #region Override

        void Awake()
        {
            GameManager.EventManager.changeColorScheme += OnChangeColorScheme;
        }

        void OnEnable()
        {
            foreach (Text text in texts)
                text.color = GameManager.DataManager.GetColorOf(colorIndex);
            foreach (Image image in images)
                image.color = GameManager.DataManager.GetColorOf(colorIndex);
            foreach (Button button in buttons)
                button.colors = GameManager.DataManager.GetColorBlockOf(colorIndex);
        }

        #endregion

        // Events
        #region Events

        private void OnChangeColorScheme(string colorScheme)
        {
            OnEnable();
        }

        #endregion

    }
}