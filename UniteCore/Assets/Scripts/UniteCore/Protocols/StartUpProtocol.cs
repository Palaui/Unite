using System.Collections.Generic;
using Unite;
using UnityEngine;

namespace UniteCore
{
    public class StartUpProtocol : Protocol
    {
        // Variables
        #region Variables

        private Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
        private float minimumPreLoadScreenTime = 0.5f;
        private float timeCounter;
        private bool loadTextureFound;
        private bool preLoadScreenLoading;

        #endregion

        // Override
        #region Override

        void Awake()
        {
            Flag = ProtocolFlag.Started;
            foreach (KeyValuePair<string, string> entry in protocolsJSon["StartUp"].GetKeyValueValues("Textures"))
                textures.Add(entry.Key, Resources.Load(entry.Value) as Texture2D);

            loadTextureFound = textures.ContainsKey("Load");
            if (!loadTextureFound)
            {
                Flag = ProtocolFlag.NonCriticalError;
                NonCriticalError = "No loading texture detected";
            }

            Flag = ProtocolFlag.Processing;

            SetLoadScreen();
        }

        void Update()
        {
            if (preLoadScreenLoading)
            {
                if (!DynamicLoader.IsLoading && timeCounter >= minimumPreLoadScreenTime)
                {
                    preLoadScreenLoading = false;
                    GameManager.UIController.OverlayToolsController.DeactivateFading();
                    SetBasicScreen();
                    DynamicLoader.ShowGUI = true;
                    CloseProtocol(this);
                }
                else
                    timeCounter += Time.deltaTime;
            }
        }

        #endregion

        // Private
        #region Private

        /// <summary> Prepares the load screen to load the initial setup of the application. </summary>
        private void SetLoadScreen()
        {
            if (!loadTextureFound)
                Screen.SetResolution((Screen.height / 2), (Screen.height / 2), false);
            else
            {
                float ratio = (float)textures["Load"].width / textures["Load"].height;
                Screen.SetResolution((int)((Screen.height / 2) * ratio), (Screen.height / 2), false);
            }

            DynamicLoader.LoadScenes(protocolsJSon["StartUp"].GetValues("Scenes"), new List<string>() { });
            DynamicLoader.ShowGUI = false;
            GameManager.UIController.OverlayToolsController.ActivateSplash(textures["Load"]);

            timeCounter = 0;
            preLoadScreenLoading = true;
        }


        /// <summary> Prepares the load screen to the original size of the application. </summary>
        private void SetBasicScreen()
        {
            Screen.SetResolution(Screen.width - 100, Screen.height - 100, false);
        }

        #endregion

    }
}