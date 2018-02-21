
namespace UniteCore
{
    public class EventManager
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

        // Public
        #region Public

        public void ChangeLanguage(Language language)
        {
            if (changeLanguage != null)
                changeLanguage(language);
        }

        public void ChangeColorScheme(string colorScheme)
        {
            if (changeColorScheme != null)
                changeColorScheme(colorScheme);
        }

        public void ChangeLightScheme(string lightScheme)
        {
            if (changeLightScheme != null)
                changeLightScheme(lightScheme);
        }

        #endregion
    }
}