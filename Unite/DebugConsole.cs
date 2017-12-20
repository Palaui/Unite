using UnityEngine;
using System.Collections;

namespace Unite
{
    public class DebugConsole : MonoBehaviour
    {
        // Variables
        #region Varbles

        private static DebugConsole instance = null;

        public GameObject DebugGui = null;
        public Vector3 defaultGuiPosition = new Vector3(0.01F, 0.98F, 0F);
        public Vector3 defaultGuiScale = new Vector3(0.5F, 0.5F, 1F);
        public Color normal = Color.green;
        public Color warning = Color.yellow;
        public Color error = Color.red;
        public int maxMessages = 30;
        public float lineSpacing = 0.02F;
        public ArrayList messages = new ArrayList();
        public ArrayList guis = new ArrayList();
        public ArrayList colors = new ArrayList();
        public bool draggable = false;
        public bool visible = true;
        public bool pixelCorrect = false;

        protected float screenHeight = -1;
        protected bool guisCreated = false;
        private bool connectedToMouse = false;

        #endregion

        // Properties
        #region Properties

        public static DebugConsole Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType(typeof(DebugConsole)) as DebugConsole;
                    if (instance == null)
                    {
                        GameObject console = new GameObject();
                        console.AddComponent<DebugConsole>();
                        console.name = "DebugConsoleController";
                        instance = FindObjectOfType(typeof(DebugConsole)) as DebugConsole;
                        Instance.InitGuis();
                    }
                }

                return instance;
            }
        }

        public static bool IsDraggable
        {
            get { return Instance.draggable; }
            set { Instance.draggable = value; }
        }

        public static bool IsVisible
        {
            get { return Instance.visible; }
            set
            {
                Instance.visible = value;
                if (value == true)
                    Instance.Display();
                else if (value == false)
                    Instance.ClearScreen();
            }
        }

        #endregion

        // Override
        #region Override

        void Awake()
        {
            instance = this;
            InitGuis();
        }

        void Update()
        {
            if (visible == true && screenHeight != Screen.height)
                InitGuis();
            if (draggable == true)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (connectedToMouse == false && DebugGui.GetComponent<GUIText>().HitTest((Vector3)Input.mousePosition) == true)
                        connectedToMouse = true;
                    else if (connectedToMouse == true)
                        connectedToMouse = false;
                }

                if (connectedToMouse == true)
                {
                    float posX = DebugGui.transform.position.x;
                    float posY = DebugGui.transform.position.y;
                    posX = Input.mousePosition.x / Screen.width;
                    posY = Input.mousePosition.y / Screen.height;
                    DebugGui.transform.position = new Vector3(posX, posY, 0F);
                }
            }
        }

        #endregion

        // Public
        #region Public

        /// <summary> Initialize GUIs. </summary>
        public void InitGuis()
        {
            float usedLineSpacing = lineSpacing;
            screenHeight = Screen.height;
            if (pixelCorrect)
                usedLineSpacing = 1.0F / screenHeight * usedLineSpacing;

            if (guisCreated == false)
            {
                if (DebugGui == null)  // If an external GUIText is not set, provide the default GUIText
                {
                    DebugGui = new GameObject();
                    DebugGui.AddComponent<GUIText>();
                    DebugGui.name = "DebugGUI(0)";
                    DebugGui.transform.position = defaultGuiPosition;
                    DebugGui.transform.localScale = defaultGuiScale;
                }

                // Create our GUI objects to our maxMessages count
                Vector3 position = DebugGui.transform.position;
                guis.Add(DebugGui);
                int x = 1;

                while (x < maxMessages)
                {
                    position.y -= usedLineSpacing;
                    GameObject clone = null;
                    clone = Instantiate(DebugGui, position, transform.rotation);
                    clone.name = string.Format("DebugGUI({0})", x);
                    guis.Add(clone);
                    position = clone.transform.position;
                    x += 1;
                }

                x = 0;
                while (x < guis.Count)
                {
                    GameObject temp = (GameObject)guis[x];
                    temp.transform.parent = DebugGui.transform;
                    x++;
                }
                guisCreated = true;
            }
            else
            {
                Vector3 position = DebugGui.transform.position;
                for (int x = 0; x < guis.Count; x++)
                {
                    position.y -= usedLineSpacing;
                    GameObject temp = (GameObject)guis[x];
                    temp.transform.position = position;
                }
            }
        }

        /// <summary> Adds a mesage to the list </summary>
        public void AddMessage(string message, string color)
        {
            messages.Add(message);
            colors.Add(color);
            Display();
        }
        /// <summary> Overloads AddMessage to only require one argument(message) </summary>
        public void AddMessage(string message)
        {
            messages.Add(message);
            colors.Add("normal");
            Display();
        }

        /// <summary> Clears the messages from the screen and the lists </summary>
        public void ClearMessages()
        {
            messages.Clear();
            colors.Clear();
            ClearScreen();
        }

        #endregion

        // Public Static
        #region Public Static

        public static void Log(string message, string color)
        {
            Instance.AddMessage(message, color);
        }
        public static void Log(string message)
        {
            Instance.AddMessage(message);
        }

        public static void Clear()
        {
            Instance.ClearMessages();
        }

        #endregion

        // Private
        #region Private

        /// <summary> Clears all output from all GUI objects </summary>
        private void ClearScreen()
        {
            if (guis.Count < maxMessages) { }
            else
            {
                int x = 0;
                while (x < guis.Count)
                {
                    GameObject gui = (GameObject)guis[x];
                    gui.GetComponent<GUIText>().text = "";
                    x += 1;
                }
            }
        }

        /// <summary> Prunes the array to fit within the maxMessages limit </summary>
        private void Prune()
        {
            int diff;
            if (messages.Count > maxMessages)
            {
                if (messages.Count <= 0)
                    diff = 0;
                else
                    diff = messages.Count - maxMessages;
                messages.RemoveRange(0, (int)diff);
                colors.RemoveRange(0, (int)diff);
            }
        }

        /// <summary> Displays the list and handles coloring </summary>
        private void Display()
        {
            if (visible == false)
                ClearScreen();
            else if (visible == true)
            {
                if (messages.Count > maxMessages)
                    Prune();

                // Carry on with display
                int x = 0;
                if (guis.Count < maxMessages) { }
                else
                {
                    while (x < messages.Count)
                    {
                        GameObject gui = (GameObject)guis[x];

                        switch ((string)colors[x])
                        {
                            case "normal":
                                gui.GetComponent<GUIText>().material.color = normal;
                                break;
                            case "warning":
                                gui.GetComponent<GUIText>().material.color = warning;
                                break;
                            case "error":
                                gui.GetComponent<GUIText>().material.color = error;
                                break;
                        }

                        gui.GetComponent<GUIText>().text = (string)messages[x];
                        x += 1;
                    }
                }
            }
        }

        #endregion

    }
}