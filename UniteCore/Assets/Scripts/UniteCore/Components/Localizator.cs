using UnityEngine;
using UnityEngine.UI;

namespace UniteCore
{
    public class Localizator : MonoBehaviour
    {
        // Variables
        #region Variables

        [SerializeField]
        private string parent;
        [SerializeField]
        private string key;

        private Text textComp;

        #endregion

        // Properties
        #region Properties

        public string Key
        {
            get { return key; }
            set { key = value; OnEnable(); }
        }

        public string Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        #endregion

        // Override
        #region Override

        void Awake()
        {
            GameManager.EventManager.changeLanguage += OnChangeLanguage;
            textComp = GetComponent<Text>();
        }

        void OnEnable()
        {
            textComp.text = GameManager.DataManager.GetLocalizationOf(key);
        }

        #endregion

        // Events
        #region Events

        private void OnChangeLanguage(Language language)
        {
            textComp.text = GameManager.DataManager.GetLocalizationOf(key);
        }

        #endregion

    }
}