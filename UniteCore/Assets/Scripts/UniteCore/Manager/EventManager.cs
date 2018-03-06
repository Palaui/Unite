using UnityEngine;

namespace UniteCore
{
    public class EventManager : MonoBehaviour
    {
        // Event Declaration
        #region Event Delcaration

        public delegate void OnChangeLanguage(Language language);
        public event OnChangeLanguage changeLanguage;

        public delegate void OnChangeColorScheme(string colorScheme);
        public event OnChangeColorScheme changeColorScheme;

        public delegate void OnChangeLightScheme(string lightScheme);
        public event OnChangeLightScheme changeLightScheme;

        #endregion

        // Protected Internal
        #region Protected Internal

        protected internal void ChangeLanguage(Language language)
        {
            if (changeLanguage != null)
                changeLanguage(language);
        }

        protected internal void ChangeColorScheme(string colorScheme)
        {
            if (changeColorScheme != null)
                changeColorScheme(colorScheme);
        }

        protected internal void ChangeLightScheme(string lightScheme)
        {
            if (changeLightScheme != null)
                changeLightScheme(lightScheme);
        }

        #endregion

    }
}