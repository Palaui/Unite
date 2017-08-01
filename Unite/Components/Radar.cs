using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    public class Radar : MonoBehaviour
    {
        // Variables
        #region Variables

        private static Dictionary<string, GameObject> dictionary = new Dictionary<string, GameObject>();
        private static GameObject container;

        List<Action<GameObject>> pingMethodsList = new List<Action<GameObject>>();
        private IEnumerator PingCoroutine;

        private GameObject locationGo = null;
        private Vector3 location;
        private float detectionRadius;

        #endregion

        // Properties
        #region Properties

        private GameObject this[string key]
        {
            get
            {
                if (dictionary.ContainsKey(key))
                    return dictionary[key];

                Debug.Log("Radar could not find the specified key " + key);
                return null;
            }
        }

        private Vector3 Location
        {
            get
            {
                if (locationGo)
                    return locationGo.transform.position;
                return location;
            }
        }

        #endregion

        // Public
        #region Public

        public static GameObject Get(string key)
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];

            Debug.Log("Radar could not find the specified key " + key);
            return null;
        }

        public void Place(GameObject go)
        {
            location = go.transform.position;
        }
        public void Place(Vector3 inLocation)
        {
            location = inLocation;
        }

        public void SetDetectionRadius(float radius)
        {
            detectionRadius = radius;
        }

        public void SetPing(Action<GameObject> goMethod = null, float pingTime = 2.0f, bool keepAlive = true)
        {
            List<Action<GameObject>> list = new List<Action<GameObject>>();
            if (goMethod != null)
                list.Add(goMethod);
            SetPing(list, pingTime, keepAlive);
        }
        public void SetPing(List<Action<GameObject>> methodsList, float pingTime = 2.0f, bool keepAlive = true)
        {
            if (PingCoroutine != null)
                StopCoroutine(PingCoroutine);
            PingCoroutine = Ping(methodsList, pingTime, keepAlive);
            StartCoroutine(PingCoroutine);
        }

        public void StopPing()
        {
            if (PingCoroutine != null)
                StopCoroutine(PingCoroutine);
        }

        public GameObject Nearest()
        {
            GameObject goToReturn = null;
            float dist = 99999;
            foreach (GameObject go in dictionary.Values)
            {
                if (Distance(go) < dist)
                {
                    goToReturn = go;
                    dist = Distance(go);
                }
            }

            if (goToReturn)
                return goToReturn;
            return null;
        }

        #endregion

        // Private
        #region Private

        private float Distance(GameObject go)
        {
            return Vector3.Distance(go.transform.position, Location);
        }
        private float Distance(string key)
        {
            if (dictionary.ContainsKey(key))
                return Vector3.Distance(dictionary[key].transform.position, Location);
            else
                return 99999;
        }

        #endregion

        // Public Static
        #region Public Static

        public static Radar Generate(float radius = 50)
        {
            if (!container)
            {
                container = new GameObject("Radar");
                Ext.ResetTransform(container);
            }

            Radar radar = Ext.GetOrAddComponent<Radar>(container);
            radar.detectionRadius = radius;
            return radar;
        }

        #endregion

        // Internal Static
        #region Internal Static

        internal static void AddElement(string key, GameObject go)
        {
            if (dictionary.ContainsKey(key))
                Debug.Log("Trying to add an already existing key to the Radar, key: " + key);
            else
                dictionary.Add(key, go);
        }

        internal static void RemoveElement(string key)
        {
            if (!dictionary.ContainsKey(key))
                Debug.Log("Trying to remove a non existing key to the Radar, key: " + key);
            else
                dictionary.Remove(key);
        }

        #endregion

        // Corroutines
        #region Corroutines

        private IEnumerator Ping(List<Action<GameObject>> methodsList, float pingTime = 2.0f, bool keepAlive = true)
        {
            float currentTime = 0;

            while (true) 
            {
                yield return null;
                currentTime += Time.deltaTime;

                if (currentTime >= pingTime)
                {
                    currentTime -= pingTime;

                    List<GameObject> list = new List<GameObject>();
                    foreach (GameObject go in dictionary.Values)
                    {
                        if (Distance(go) < detectionRadius)
                        {
                            for (int i = 0; i < methodsList.Count; i++)
                                methodsList[i](go);
                        }
                    }

                    if (!keepAlive)
                        StopCoroutine(PingCoroutine);
                }
            }
        }

        #endregion

    }
}
