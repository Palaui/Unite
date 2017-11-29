using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unite
{
    public class DynamicLoader : MonoBehaviour
    {
        // Enum
        #region Enum

        public enum LoadingMode { Default }

        #endregion

        // Variables
        #region Variables

        private static List<string> scenesToLoad = new List<string>();
        private static List<string> scenesToUnload = new List<string>();

        private static LoadingMode loadingMode = LoadingMode.Default;

        private static Action loadEndCallback;
        private static Texture2D loadingWheelTex;
        private static Texture2D backgroundTex = Texture2D.whiteTexture;
        private static Texture2D fadeTex = Texture2D.whiteTexture;
        private static Color backgroundTexCol = new Color(0, 0, 0, 0.5f);
        private static Color fadeTexCol = Color.black;
        private static Vector2 stepsRangePerFrame = new Vector2(2, 8);
        private static int size = 60;
        private static bool isLoading = false;

        // Scene change
        private static float fadeTime = 2;
        private static float fadeAlpha = 0;

        // Default loading
        private static int rotationSpeed = 200;
        private static bool showGUI = true;

        #endregion

        // Properties
        #region Properties

        public static Texture2D LoadingTexture
        {
            get { return loadingWheelTex; }
            set { loadingWheelTex = value; }
        }

        public static bool IsLoading
        {
            get { return isLoading || (fadeAlpha > 0); }
        }

        public static bool ShowGUI
        {
            set { showGUI = value; }
            get { return showGUI; }
        }

        #endregion

        // Override
        #region Override

        void OnGUI()
        {
            if (showGUI)
            {
                if (isLoading)
                {
                    if (!loadingWheelTex)
                        loadingWheelTex = ImageUtility.BitmapToTexture2D(Properties.Resources.LoadingWheel);

                    switch (loadingMode)
                    {
                        case LoadingMode.Default:
                            DefaultLoading();
                            break;
                    }
                }

                if (fadeAlpha > 0)
                {
                    GUI.color = new Color(fadeTexCol.r, fadeTexCol.g, fadeTexCol.b, fadeAlpha);
                    GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTex);
                    GUI.color = new Color(1, 1, 1, 1);
                }
            }
        }

        #endregion

        // Public Static
        #region Public Static

        public static void Generate(int minSteps, int maxSteps, int inSizePortion = 10, int inRotationSPeed = 200)
        {
            Ext.GetOrAddComponent<DynamicLoader>(Container.GetContainer());
            stepsRangePerFrame = new Vector2(minSteps, maxSteps);
            stepsRangePerFrame.x = Mathf.Max(1, stepsRangePerFrame.x);
            stepsRangePerFrame.y = Mathf.Max(stepsRangePerFrame.x, stepsRangePerFrame.y);

            size = Mathf.RoundToInt(Mathf.Min(Screen.width, Screen.height) / inSizePortion);
            rotationSpeed = inRotationSPeed;
        }

        public static void LoadActivity(List<GameObject> activation, List<GameObject> deactivation)
        {
            if (IsLoading)
                return;

            List<GameObject> activationToPass = new List<GameObject>();
            foreach (GameObject go in activation)
            {
                if (!go.activeSelf)
                    activationToPass.Add(go);
            }
            List<GameObject> deactivationToPass = new List<GameObject>();
            foreach (GameObject go in deactivation)
            {
                if (go.activeSelf)
                    deactivationToPass.Add(go);
            }

            DynamicLoader loader = Ext.GetOrAddComponent<DynamicLoader>(Container.GetContainer());
            loader.StartCoroutine(loader.LoadActivityCoroutine(activationToPass, deactivationToPass));
        }

        public static void LoadScenes(List<string> activation, List<string> deactivation)
        {
            if (IsLoading)
                return;

            List<string> activationToPass = new List<string>();
            foreach (string sceneName in activation)
            {
                if (SceneManager.GetSceneByName(sceneName) != null)
                {
                    if (!SceneManager.GetSceneByName(sceneName).isLoaded)
                        activationToPass.Add(sceneName);
                }
            }
            List<string> deactivationToPass = new List<string>();
            foreach (string sceneName in deactivation)
            {
                if (SceneManager.GetSceneByName(sceneName) != null)
                {
                    if (SceneManager.GetSceneByName(sceneName).isLoaded)
                        deactivationToPass.Add(sceneName);
                }
            }

            DynamicLoader loader = Ext.GetOrAddComponent<DynamicLoader>(Container.GetContainer());
            loader.StartCoroutine(loader.LoadScenesCoroutine(activationToPass, deactivationToPass));
        }

        public static void LoadScene(string sceneName, LoadSceneMode loadSceneMode)
        {
            if (IsLoading)
            {
                if (loadSceneMode == LoadSceneMode.Additive)
                    scenesToLoad.Add(sceneName);
                return;
            }

            DynamicLoader loader = Ext.GetOrAddComponent<DynamicLoader>(Container.GetContainer());
            loader.StartCoroutine(loader.LoadSceneCoroutine(sceneName, loadSceneMode));
        }

        public static void UnloadScene(string sceneName)
        {
            if (IsLoading)
            {
                scenesToUnload.Add(sceneName);
                return;
            }

            DynamicLoader loader = Ext.GetOrAddComponent<DynamicLoader>(Container.GetContainer());
            loader.StartCoroutine(loader.UnloadSceneCoroutine(sceneName));
        }

        public static void SetCallbackOnLoadSceneEnd(Action action)
        {
            loadEndCallback = action;
        }

        #endregion

        // Private Static
        #region Private Static

        private static void DefaultLoading()
        {
            GUI.color = backgroundTexCol;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), backgroundTex);
            GUI.color = new Color(1, 1, 1, 1);

            if (fadeAlpha == 0)
            {
                GUIUtility.RotateAroundPivot(rotationSpeed * Time.unscaledTime, new Vector2(Screen.width / 2, Screen.height / 2));
                GUI.DrawTexture(new Rect((Screen.width - size) / 2, (Screen.height - size) / 2, size, size), loadingWheelTex);
                GUI.matrix = Matrix4x4.identity;
            }
        }

        private static void ResolveSceneLoad()
        {
            if (scenesToUnload.Count > 0)
            {
                DynamicLoader loader = Ext.GetOrAddComponent<DynamicLoader>(Container.GetContainer());
                loader.StartCoroutine(loader.UnloadSceneCoroutine(scenesToUnload[0]));
                scenesToUnload.RemoveAt(0);
            }
            else if (scenesToLoad.Count > 0)
            {
                DynamicLoader loader = Ext.GetOrAddComponent<DynamicLoader>(Container.GetContainer());
                loader.StartCoroutine(loader.LoadSceneCoroutine(scenesToLoad[0], LoadSceneMode.Additive));
                scenesToLoad.RemoveAt(0);
            }
            else
            {
                if (loadEndCallback != null)
                {
                    loadEndCallback();
                    loadEndCallback = null;
                }
                isLoading = false;
            }
        }

        #endregion

        // Coroutines
        #region Coroutines

        private IEnumerator LoadActivityCoroutine(List<GameObject> activation, List<GameObject> deactivation)
        {
            int currentSteps = 0;
            bool deactivating = true;

            isLoading = true;

            while (isLoading)
            {
                if (deactivating)
                {
                    currentSteps = Mathf.Min(
                        Mathf.RoundToInt(stepsRangePerFrame.x +
                        DynamicGraphicsModule.QualityLevel01 * (stepsRangePerFrame.y - stepsRangePerFrame.x)),
                        deactivation.Count);
                    for (int i = currentSteps - 1; i >= 0; i--)
                    {
                        deactivation[i].SetActive(false);
                        deactivation.RemoveAt(i);
                    }
                    if (deactivation.Count == 0)
                        deactivating = false;
                }
                else
                {
                    currentSteps = Mathf.Min(
                        Mathf.RoundToInt(stepsRangePerFrame.x +
                        DynamicGraphicsModule.QualityLevel01 * (stepsRangePerFrame.y - stepsRangePerFrame.x)),
                        activation.Count);
                    for (int i = currentSteps - 1; i >= 0; i--)
                    {
                        activation[i].SetActive(true);
                        activation.RemoveAt(i);
                    }
                    if (activation.Count == 0)
                        isLoading = false;
                }
                yield return null;
            }
        }

        private IEnumerator LoadScenesCoroutine(List<string> activation, List<string> deactivation)
        {
            isLoading = true;

            foreach (string sceneName in deactivation)
            {
                AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(sceneName);
                while (!asyncLoad.isDone)
                    yield return null;
            }

            foreach (string sceneName in activation)
            {
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                while (!asyncLoad.isDone)
                    yield return null;
            }

            ResolveSceneLoad();
        }

        private IEnumerator LoadSceneCoroutine(string sceneName, LoadSceneMode loadSceneMode)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
            float lastProgress = -1;

            if (loadSceneMode == LoadSceneMode.Single)
                asyncLoad.allowSceneActivation = false;
            fadeAlpha = 0;
            isLoading = true;

            switch (loadSceneMode)
            {
                case LoadSceneMode.Single:
                    while (!asyncLoad.isDone)
                    {
                        while (lastProgress < asyncLoad.progress && asyncLoad.progress > 0.85f)
                        {
                            lastProgress = asyncLoad.progress;
                            yield return null;
                        }
                        while (fadeAlpha < 1)
                        {
                            fadeAlpha += Time.deltaTime / fadeTime;
                            if (fadeAlpha >= 1)
                            {
                                fadeAlpha = 1;
                                asyncLoad.allowSceneActivation = true;
                                isLoading = false;
                            }
                            yield return null;
                        }
                    }

                    while (fadeAlpha > 0)
                    {
                        fadeAlpha -= Time.deltaTime / fadeTime;
                        if (fadeAlpha < 0)
                            fadeAlpha = 0;
                        yield return null;
                    }

                    scenesToLoad.Clear();
                    scenesToUnload.Clear();

                    break;
                case LoadSceneMode.Additive:
                    while (!asyncLoad.isDone)
                        yield return null;

                    ResolveSceneLoad();
                    break;
            }
        }

        private IEnumerator UnloadSceneCoroutine(string sceneName)
        {
            AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(sceneName);
            isLoading = true;

            while (!asyncLoad.isDone)
                yield return null;

            ResolveSceneLoad();
        }

        #endregion
    }
}
