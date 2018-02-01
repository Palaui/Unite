using System.Collections;
using Unite;
using UnityEngine;

namespace UniteCore
{
    public class DeviceConnectivityProtocol : Protocol
    {
        // Properties
        #region Properties

        public bool DeviceConnection { get { return Application.internetReachability != NetworkReachability.NotReachable; } }
        public bool Required { get { return required; } }

        #endregion

        // Override
        #region Override

        void Awake()
        {
            Flag = ProtocolFlag.Started;
        }

        #endregion

        // Coroutines
        #region Coroutines

        private IEnumerator KeepAlive()
        {
            Flag = ProtocolFlag.Alive;

            while (true)
            {
                yield return new WaitForSeconds(2);
                if (required && !isAlertOn && !DeviceConnection)
                    DisplayNativeAlert();
            }
        }

        #endregion

    }
}