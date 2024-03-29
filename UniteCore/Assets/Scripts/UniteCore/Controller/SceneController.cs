﻿using System.Collections.Generic;
using Unite;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UniteCore
{
    public class SceneController : MonoBehaviour
    {
        // Variables
        #region Variables

        private List<string> activeScenes = new List<string>();

        #endregion

        // Public
        #region Public

        public void LoadScene(List<string> sceneNames) { Ext.ApplyForeach(LoadScene, sceneNames); }
        public void LoadScene(string sceneName)
        {
            if (!activeScenes.Contains(sceneName))
            {
                DynamicLoader.LoadScene(sceneName, LoadSceneMode.Additive);
                activeScenes.Add(sceneName);
            }
        }

        public void UnloadScene(List<string> sceneNames) { Ext.ApplyForeach(UnloadScene, sceneNames); }
        public void UnloadScene(string sceneName)
        {
            if (activeScenes.Contains(sceneName))
            {
                DynamicLoader.UnloadScene(sceneName);
                activeScenes.Remove(sceneName);
            }
        }

        public void ToggleSceneLoadState(List<string> sceneNames) { Ext.ApplyForeach(ToggleSceneLoadState, sceneNames); }
        public void ToggleSceneLoadState(string sceneName)
        {
            if (activeScenes.Contains(sceneName))
            {
                DynamicLoader.UnloadScene(sceneName);
                activeScenes.Remove(sceneName);
            }
            else
            {
                DynamicLoader.LoadScene(sceneName, LoadSceneMode.Additive);
                activeScenes.Add(sceneName);
            }
        }

        #endregion

    }
}