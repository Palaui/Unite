using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    public class DynamicLoader : MonoBehaviour
    {
        // Variables
        #region Variables

        private static Vector2 stepsRangePerFrame = new Vector2(2, 8);
        private static bool isLoading = false;

        #endregion

        // Properties
        #region Properties

        public static bool IsLoading
        {
            get { return isLoading; }
        }

        #endregion

        // Overiide
        #region Overiide

        void Update()
        {
            if (isLoading)
            {

            }
        }

        #endregion

        // Public Static
        #region Public Static

        public static void Generate(Vector2 inStepsPerFrame)
        {
            Ext.GetOrAddComponent<DynamicLoader>(Container.GetContainer());
            stepsRangePerFrame = inStepsPerFrame;
            stepsRangePerFrame.x = Mathf.Max(1, stepsRangePerFrame.x);
            stepsRangePerFrame.y = Mathf.Max(stepsRangePerFrame.x, stepsRangePerFrame.y);
        }

        public static void LoadActivity(List<GameObject> activation, List<GameObject> deactivation)
        {
            if (isLoading)
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
            loader.StartCoroutine(loader.Load(activationToPass, deactivationToPass));
        }

        #endregion

        // Coroutines
        #region Coroutines

        private IEnumerator Load(List<GameObject> activation, List<GameObject> deactivation)
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
                    Debug.Log(currentSteps);
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

        #endregion
    }
}
