using System.Collections.Generic;
using Unite;
using UnityEngine;

namespace UniteCore
{
    public class Protocol : MonoBehaviour
    {
        // Enums
        #region Enums

        protected enum ProtocolFlag { NotStarted, Started, Processing, NonCriticalError, CriticalError, Alive, Closed }

        #endregion

        // Variables
        #region Variables

        protected static JSon protocolsJSon;
        protected static GameObject container;
        protected bool required;
        protected bool isAlertOn;

        private static List<string> protocolsStatus = new List<string>();
        private static List<string> protocolsErrors = new List<string>();

        #endregion

        // Properties
        #region Properties

        protected ProtocolFlag Flag
        {
            set { protocolsStatus.Add(GetType().ToString() + ": " + value.ToString()); }
        }

        protected string NonCriticalError
        {
            set
            {
                string str = GetType().ToString() + ": NonCriticalError " + value;
                protocolsStatus.Add(str);
                protocolsErrors.Add(str);
                Debug.Log(str);
            }
        }

        protected string CriticalError
        {
            set
            {
                string str = GetType().ToString() + ": CriticalError " + value;
                protocolsStatus.Add(str);
                protocolsErrors.Add(str);
                Debug.Log(str);
            }
        }

        #endregion

        // Override
        #region Override

        void Awake()
        {
            DestroyImmediate(this);
        }

        #endregion

        // Protected
        #region Protected

        /// <summary> Ends a protocol. </summary>
        /// <param name="protocol"> Protocol to close. </param>
        protected void CloseProtocol(Protocol protocol)
        {
            protocol.Flag = ProtocolFlag.Closed;
            Destroy(protocol, 0.2f);
        }

        /// <summary> Displays a native alert on the system. </summary>
        protected void DisplayNativeAlert()
        {
            isAlertOn = true;
            Debug.Log("Alert On");
        }

        struct Encode { public string name, value; }
        /// <summary> Cloases a native alert. </summary>
        protected void CloseNativeALert()
        {
            isAlertOn = false;
        }

        #endregion

        // Public Static
        #region Public Static

        /// <summary> Begins a protocol. </summary>
        /// <typeparam name="T"> Type of the protocol to begin. </typeparam>
        public static void StartProtocol<T>() where T : Protocol
        {
            if (protocolsJSon == null)
            {
                protocolsJSon = new JSon(Resources.Load("Engine/Data/Protocols") as TextAsset);
                container = Container.GetContainer();
            }

            Ext.GetOrAddComponent<T>(container);
        }

        /// <summary> Gets a protocol. </summary>
        /// <typeparam name="T"> Type of the protocol to get. </typeparam>
        public static T GetProtocol<T>() where T : Protocol
        {
            return Container.GetContainer().GetComponent<T>();
        }

        /// <summary> Displays on console the statuses of every called protocol. </summary>
        /// <param name="onlyErrors"> If show only the error messages. </param>
        public static void DebugProtocols(bool onlyErrors)
        {
            if (!onlyErrors)
            {
                foreach (string str in protocolsStatus)
                    Debug.Log(str);
            }
            else
            {
                foreach (string str in protocolsErrors)
                    Debug.Log(str);
            }
        }

        #endregion

    }
}