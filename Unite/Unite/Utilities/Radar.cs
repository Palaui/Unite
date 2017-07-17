using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    class Radar
    {
        // Variables
        #region Variables

        private static Dictionary<string, GameObject> dictionary = new Dictionary<string, GameObject>();

        #endregion

        // Properties
        #region Properties

        public static GameObject Get(string key)
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];

            Debug.Log("Radar could not find the specified key " + key);
            return null;
        }

        #endregion

        // Public
        #region Public

        public static void AddElement(string key, GameObject go)
        {
            if (dictionary.ContainsKey(key))
                Debug.Log("Trying to add an already existing key to the Radar, key: " + key);

            else
                dictionary.Add(key, go);
        }

        public static void RemoveElement(string key)
        {
            if (!dictionary.ContainsKey(key))
                Debug.Log("Trying to remove a non existing key to the Radar, key: " + key);

            else
                dictionary.Remove(key);
        }

        #endregion

    }
}
