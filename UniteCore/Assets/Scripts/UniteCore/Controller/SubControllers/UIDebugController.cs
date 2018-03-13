using System.Reflection;
using Unite;
using UnityEngine;
using UnityEngine.UI;

namespace UniteCore
{
    public class UIDebugController : MonoBehaviour
    {
        // Variables
        #region Variables

        private JSon jsonDebugConsole;

        private Transform verticalContent;
        private InputField inputField;
        private bool isInitialized = false;

        #endregion

        // Override
        #region Override

        void Start()
        {
            if (!isInitialized)
            {
                jsonDebugConsole = (new JSon(Resources.Load("UniteCore/Data/DebugConsole") as TextAsset));

                verticalContent = transform.Find("DeveloperAutoBackground");
                inputField = GetComponentInChildren<InputField>();
                isInitialized = true;
            }
        }

        #endregion

        // Public
        #region Public

        /// <summary> Called when console is updated.. </summary>
        /// <param name="str"> Current console text. </param>
        public void CallConsoleUpdate(string str)
        {
            ConsoleUpdate(str);
        }

        /// <summary> Called when console command is sent. </summary>
        /// <param name="str"> Current console text. </param>
        public void CallConsoleInput(string str)
        {
            string[] strs = ConsoleInput(str);

            foreach (UIDebugController comp in GetComponents<UIDebugController>())
                comp.CheckConsoleInput(strs);
        }

        /// <summary> 
        /// Fills the console with the button information (Should contain the previous words plus the button word. 
        /// </summary>
        /// <param name="str"> String that will fill the console. </param>
        public void ConsoleButtonPress(string str)
        {
            inputField.text = str + " ";
            if (!Input.touchSupported || Application.platform == RuntimePlatform.WebGLPlayer)
            {
                inputField.ActivateInputField();
                inputField.Select();
            }
        }

        /// <summary> Called from a button, acts as an accepted console input. </summary>
        public void Apply()
        {
            CallConsoleInput(inputField.text);
        }

        #endregion

        // Protected
        #region Protected

        protected void CheckConsoleInput(string[] strs)
        {
            if (strs.Length == 1)
            {
                MethodInfo info = Reflect.GetMethodByName(GetType(), strs[0]);
                if (info != null)
                    info.Invoke(this, new object[] { });
            }
            else if (strs.Length == 2)
            {
                MethodInfo info = Reflect.GetMethodByName(GetType(), strs[0]);
                if (info != null)
                    info.Invoke(this, new object[] { strs[1] });
            }
        }

        #endregion

        // Private
        #region Private

        /// <summary> Creates a button for the console that will be able to introduce a new word to it. </summary>
        /// <param name="previous"> Previous words introduced. </param>
        /// <param name="key"> The name of the button and the order word that should display. </param>
        private void CreateAutoButton(string previous, string key)
        {
            if (key == "List")
                return;

            GameObject go = Instantiate(Resources.Load("UniteCore/Prefabs/DeveloperAutoButton")) as GameObject;
            go.name = key;
            go.transform.SetParent(verticalContent);
            go.transform.localScale = Vector3.one;
            go.GetComponentInChildren<Text>().text = go.name;

            if (previous == "")
                go.GetComponent<Button>().onClick.AddListener(delegate { ConsoleButtonPress(go.name); });
            else
                go.GetComponent<Button>().onClick.AddListener(delegate { ConsoleButtonPress(previous + "" + go.name); });
        }

        // Specific
        #region Specific

        private void ToggleSceneState(string str)
        {
            GameManager.SceneController.ToggleSceneLoadState(str);
        }

        private void SetColorScheme(string str)
        {
            GameManager.CurrentColorScheme = str;
        }

        private void SetLightingScheme(string str)
        {
            GameManager.CurrentLightScheme = str;
        }

        #endregion

        // Calculus
        #region Calculus

        private void ConsoleUpdate(string str)
        {
            JSon json = jsonDebugConsole;
            string previous = "";

            // Initialize and parse
            if (!isInitialized)
                Start();

            while (str.Contains("  "))
                str = str.Replace("  ", " ");

            string[] words = str.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

            // Remove auto buttons
            while (verticalContent.childCount > 0)
            {
                foreach (Transform child in verticalContent)
                    DestroyImmediate(child.gameObject);
            }

            // Detect route key words
            foreach (string word in words)
            {
                if (json.ContainsNode(word))
                {
                    previous += word + " ";
                    json = json[word];
                    continue;
                }

                break;
            }

            // Decide auto buttons to display
            if (json.ContainsValue("List") && json.ContainsValue("Method"))
            {
                foreach (string key in jsonDebugConsole["List"].GetValue(json.GetValue("List")).Split(new char[] { ' ' },
                    System.StringSplitOptions.RemoveEmptyEntries))
                {
                    CreateAutoButton(previous, key);
                }

                return;
            }

            foreach (string key in json.GetNodeKeys())
                CreateAutoButton(previous, key);
        }

        private string[] ConsoleInput(string str)
        {
            JSon json = jsonDebugConsole;

            str = str.Trim();
            while (str.Contains("  "))
                str = str.Replace("  ", " ");
            string[] words = str.Split(' ');
            str = "";

            foreach (string word in words)
            {
                if (json.ContainsNode(word))
                {
                    json = json[word];
                    continue;
                }

                str = word;
                break;
            }

            if (json.ContainsValue("Method"))
            {
                if (json.ContainsValue("List"))
                {
                    if (str != "")
                        return new string[] { json.GetValue("Method"), str };
                    else
                        return new string[] { };
                }

                return new string[] { json.GetValue("Method") };
            }

            return new string[] { };
        }

        #endregion

        #endregion

    }
}