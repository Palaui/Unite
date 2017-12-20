using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Unite
{
    public static class Ext
    {
        // Components
        #region Components

        /// <summary> Makes sure that a gameobjects has a component "T" </summary>
        /// <typeparam name="T"> Type of the component. </typeparam>
        /// <param name="go"> GameObject the we will ensure the component to. </param>
        /// <returns> The component "T" on that gameobject. </returns>
        public static T GetOrAddComponent<T>(GameObject go) where T : Component
        {
            if (go)
            {
                T comp = go.GetComponent<T>();
                if (comp == null)
                    comp = go.AddComponent<T>();
                return comp;
            }
            Debug.LogError("- (MethodExtensions GetOrAddComponent) GameObject passed not found, Adding " + typeof(T) + " Failed");
            return null;
        }
        /// <summary> Makes sure that a gameobjects has a pack of components. </summary>
        /// <param name="go"> GameObject the we will ensure the component to. </param>
        /// <param name="components"> Array of components we want to ensure. </param>
        /// <returns> The types aray we sent as a parameter for further use. </returns>
        public static Type[] GetOrAddComponent(GameObject go, Type[] components)
        {
            if (go)
            {
                foreach (Type type in components)
                {
                    if (!go.GetComponent(type))
                        go.AddComponent(type);
                }
                return components;
            }

            Debug.LogError("- (MethodExtensions GetOrAddComponent) Element passed not found. Aborting");
            return null;
        }

        /// <summary> Sets the default coordinates of a gameobject's transform. </summary>
        /// <typeparam name="T"> Type passed to get the transform from. </typeparam>
        /// <param name="elem"> The "T" typed element. </param>
        /// <param name="useRelativeTransform"> If the default coordinates are ind local or world space. </param>
        public static void ResetTransform<T>(T elem, bool useRelativeTransform = false)
        {
            if (elem == null)
                Debug.LogError("- (MethodExtensions ResetTransform) Element passed not found. Aborting");

            Transform tr = null;
            if (elem is GameObject)
                tr = (elem as GameObject).transform;
            if (elem is Component)
                tr = (elem as Component).transform;

            if (!tr)
                Debug.LogError("- (MethodExtensions ResetTransform) Unable to find element related transform. Aborting");

            if (useRelativeTransform)
            {
                tr.localPosition = Vector3.zero;
                tr.localRotation = Quaternion.identity;
            }
            else
            {
                tr.position = Vector3.zero;
                tr.rotation = Quaternion.identity;
            }
            tr.localScale = Vector3.one;
        }

        /// <summary> Destroys all children of this transform. </summary>
        /// <typeparam name="T"> Type passed to get the transform from. </typeparam>
        /// <param name="elem"> The "T" typed element. </param>
        public static void DestroyChildren<T>(T elem)
        {
            if (elem == null)
            {
                Debug.LogError("- (MethodExtensions DestroyChildren) Element passed not found. Aborting");
                return;
            }

            Transform tr = null;
            if (elem is GameObject)
                tr = (elem as GameObject).transform;
            if (elem is Component)
                tr = (elem as Component).transform;

            if (!tr)
            {
                Debug.LogError("- (MethodExtensions DestroyChildren) Unable to find element related transform. Aborting");
                return;
            }

            while (tr.childCount > 0)
            {
                foreach (Transform child in tr)
                {
                    if (child.parent == tr)
                        UnityEngine.Object.DestroyImmediate(child.gameObject);
                }
            }
        }

        /// <summary> Creates a bounds objects with the combination of a set of GameObjects. </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elems"></param>
        /// <returns> Returns the resulting bounds </returns>
        public static Bounds GenerateCombinedBounds<T>(List<T> elems)
        {
            if (elems.Count == 0)
            {
                Debug.LogError("- (MethodExtensions GenerateCombinedBounds) Elements passed not found. Aborting");
                return new Bounds();
            }

            List<Bounds> boundsList = new List<Bounds>();
            foreach (T elem in elems)
            {
                GameObject go = null;
                if (elem is GameObject)
                    go = elem as GameObject;
                else if (elem is Component)
                    go = (elem as Component).gameObject;

                if (go)
                {
                    if (go.GetComponent<Renderer>())
                        boundsList.Add(go.GetComponent<Renderer>().bounds);
                }
            }
            return GenerateCombinedBounds(boundsList);
        }
        /// <summary> Creates a bounds objects with the combination of a set of bounds. </summary>
        /// <param name="boundsList"> Set of bounds. </param>
        /// <returns> The created bound. </returns>
        public static Bounds GenerateCombinedBounds(List<Bounds> boundsList)
        {
            if (boundsList.Count == 0)
            {
                Debug.LogError("- (MethodExtensions GenerateCombinedBounds) Elements passed not found. Aborting");
                return new Bounds();
            }

            Bounds combination = boundsList[0];
            foreach (Bounds bounds in boundsList)
                combination.Encapsulate(bounds);

            return combination;
        }

        #endregion

        // Method Call
        #region Methods Call

        /// <summary> Method added to button listeners capable of calling other methods by their name. </summary>
        /// <param name="ob"> The class instance where we want to fire the method from. </param>
        /// <param name="str"></param>
        public static void ButtonListener(object ob, string str)
        {
            if (ob.GetType().GetMethod(str) != null)
                ob.GetType().GetMethod(str).Invoke(ob, null);
        }

        /// <summary> Applies a method to all the list items passed. </summary>
        /// <typeparam name="T"> Type of the params of the method. </typeparam>
        /// <param name="method"> Method to call for each member in the list param. </param>
        /// <param name="list"> params of the method to call. </param>
        public static void ApplyForeach<T>(Action<T> method, List<T> list)
        {
            foreach (T element in list)
                method(element);
        }
        /// <summary> Applies a set of methods to all the list items passed. </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodsList"></param>
        /// <param name="list"></param>
        public static void ApplyForeach<T>(List<Action<T>> methodsList, List<T> list)
        {
            foreach (Action<T> method in methodsList)
                ApplyForeach(method, list);
        }
        public static List<U> ApplyForeach<T, U>(Func<T, U> method, List<T> list)
        {
            List<U> results = new List<U>();
            foreach (T element in list)
                results.Add(method(element));
            return results;
        }
        public static List<List<U>> ApplyForeach<T, U>(List<Func<T, U>> methodsList, List<T> list)
        {
            List<List<U>> results = new List<List<U>>();
            foreach (Func<T, U> method in methodsList)
                results.Add(ApplyForeach(method, list));
            return results;
        }
        public static List<S> ApplyForeach<T, U, S>(Func<T, U> methodA, Func<U, S> methodB, List<T> list)
        {
            List<U> midResults = new List<U>();
            List<S> results = new List<S>();
            foreach (T element in list)
                midResults.Add(methodA(element));
            foreach (U element in midResults)
                results.Add(methodB(element));
            return results;
        }

        public static void ApplyConditionForeach<T>(Action<T> method, List<T> list, bool condition)
        {
            foreach (T element in list)
            {
                if (condition)
                    method(element);
            }
        }
        public static void ApplyConditionForeach<T>(List<Action<T>> methodsList, List<T> list, bool condition)
        {
            foreach (T element in list)
            {
                if (condition)
                {
                    foreach (Action<T> method in methodsList)
                        method(element);
                }
            }
        }

        #endregion

        // Dictionaries, Lists, Arrays
        #region Dictionaries, Lists, Arrays

        public static Dictionary<T, U> CreateDictionary<T, U>(List<T> keys, List<U> values)
        {
            Dictionary<T, U> dictionary = new Dictionary<T, U>();
            int limit = Mathf.Max(keys.Count, values.Count);
            for (int i = 0; i < limit; i++)
                dictionary.Add(keys[i], values[i]);
            return dictionary;
        }

        public static List<T> CreateListFromArray<T>(T[] array)
        {
            List<T> list = new List<T>();
            foreach (T element in array)
                list.Add(element);
            return list;
        }
        public static List<List<T>> CreateListFromArray<T>(T[,] array)
        {
            List<List<T>> list = new List<List<T>>();
            for (int i = 0; i < array.GetLength(0); i++)
            {
                list.Add(new List<T>());
                for (int j = 0; j < array.GetLength(1); j++)
                    list[i].Add(array[i, j]);
            }
            return list;
        }

        public static T[] CreateArrayFromList<T>(List<T> list)
        {
            T[] array = new T[list.Count];
            for (int i = 0; i < list.Count; i++)
                array[i] = list[i];
            return array;
        }
        public static T[,] CreateArrayFromList<T>(List<List<T>> list)
        {
            T[,] array = new T[list.Count, list[0].Count];
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list[0].Count; j++)
                    array[i, j] = list[i][j];
            }

            return array;
        }

        public static List<T> ConcatenateLists<T>(List<T> aList, List<T> bList)
        {
            List<T> list = new List<T>();
            foreach (T elem in aList)
                list.Add(elem);
            foreach (T elem in bList)
                list.Add(elem);
            return list;
        }

        public static Dictionary<T, U> ConcatenateDictionaries<T, U>(Dictionary<T, U> aDictionary, Dictionary<T, U> bDictionary)
        {
            Dictionary<T, U> dictionary = new Dictionary<T, U>();
            foreach (KeyValuePair<T, U> elem in aDictionary)
                dictionary.Add(elem.Key, elem.Value);
            foreach (KeyValuePair<T, U> elem in bDictionary)
                dictionary.Add(elem.Key, elem.Value);
            return dictionary;
        }

        public static T GetKeyAtIndex<T, U>(Dictionary<T, U> dictionary, int index)
        {
            int i = -1;
            foreach (T key in dictionary.Keys)
            {
                i++;
                if (index == i)
                    return key;
            }

            Debug.LogError("Ext GetKeyAtIndex: Index was outside the bounds");
            return default(T);
        }

        public static U GetValueAtIndex<T, U>(Dictionary<T, U> dictionary, int index)
        {
            int i = -1;
            foreach (U value in dictionary.Values)
            {
                i++;
                if (index == i)
                    return value;
            }

            Debug.LogError("Ext GetValueAtIndex: Index was outside the bounds");
            return default(U);
        }

        #endregion

        // Conversions
        #region Conversions

        public static List<Vector3> ConvertList2DTo3D(List<Vector2> list2D, float zValue)
        {
            List<Vector3> list3D = new List<Vector3>();
            foreach (Vector2 elem in list2D)
                list3D.Add(new Vector3(elem.x, elem.y, zValue));
            return list3D;
        }
        public static List<DoubleV3> ConvertList2DTo3D(List<DoubleV2> list2D, double zValue)
        {
            List<DoubleV3> list3D = new List<DoubleV3>();
            foreach (DoubleV2 elem in list2D)
                list3D.Add(new DoubleV3(elem.x, elem.y, zValue));
            return list3D;
        }

        public static List<Vector2> ConvertList3DTo2D(List<Vector3> list3D)
        {
            List<Vector2> list2D = new List<Vector2>();
            foreach (Vector3 elem in list3D)
                list2D.Add(new Vector2(elem.x, elem.y));
            return list2D;
        }
        public static List<DoubleV2> ConvertList3DTo2D(List<DoubleV3> list3D)
        {
            List<DoubleV2> list2D = new List<DoubleV2>();
            foreach (DoubleV3 elem in list3D)
                list2D.Add(new DoubleV2(elem.x, elem.y));
            return list2D;
        }

        public static IEnumerable<T> GetEnumValues<T>()
        {
            return (T[])Enum.GetValues(typeof(T));
        }

        public static List<string> GetEnumAsStringList<T>()
        {
            List<string> list = new List<string>();
            foreach (var entry in Enum.GetValues(typeof(T)))
                list.Add(entry.ToString());

            return null;
        }

        public static Color GetColorFromString(string value, bool useColor32)
        {
            value = value.Replace(",", "").Replace("\t", "").Trim();
            string[] cValues = value.Split(new char[] { ' ' });
            if (cValues.Length != 4)
            {
                Debug.LogError("Unable to parse " + value + " to Color");
                return Color.white;
            }

            Color color;
            if (useColor32)
                color = new Color32(byte.Parse(cValues[0]), byte.Parse(cValues[1]), byte.Parse(cValues[2]), byte.Parse(cValues[3]));
            else
                color = new Color(float.Parse(cValues[0]), float.Parse(cValues[1]), float.Parse(cValues[2]), float.Parse(cValues[3]));

            return color;
        }

        public static ColorBlock GetColorBlockFromString(string value, bool useColor32)
        {
            value = value.Replace(",", "").Replace("\t", "").Trim();
            string[] cValues = value.Split(new char[] { ' ' });
            if (cValues.Length != 16)
            {
                Debug.LogError("Unable to parse " + value + " to ColorBlock");
                return new ColorBlock();
            }

            ColorBlock block = new ColorBlock();
            if (useColor32)
            {
                block.normalColor = new Color32(byte.Parse(cValues[0]), byte.Parse(cValues[1]), byte.Parse(cValues[2]), byte.Parse(cValues[3]));
                block.highlightedColor = new Color32(byte.Parse(cValues[4]), byte.Parse(cValues[5]), byte.Parse(cValues[6]), byte.Parse(cValues[7]));
                block.pressedColor = new Color32(byte.Parse(cValues[8]), byte.Parse(cValues[9]), byte.Parse(cValues[10]), byte.Parse(cValues[11]));
                block.disabledColor = new Color32(byte.Parse(cValues[12]), byte.Parse(cValues[13]), byte.Parse(cValues[14]), byte.Parse(cValues[15]));
            }
            else
            {
                block.normalColor = new Color(float.Parse(cValues[0]), float.Parse(cValues[1]), float.Parse(cValues[2]), float.Parse(cValues[3]));
                block.highlightedColor = new Color(float.Parse(cValues[4]), float.Parse(cValues[5]), float.Parse(cValues[6]), float.Parse(cValues[7]));
                block.pressedColor = new Color(float.Parse(cValues[8]), float.Parse(cValues[9]), float.Parse(cValues[10]), float.Parse(cValues[11]));
                block.disabledColor = new Color(float.Parse(cValues[12]), float.Parse(cValues[13]), float.Parse(cValues[14]), float.Parse(cValues[15]));
            }
            block.colorMultiplier = 1;
            block.fadeDuration = 0.2f;

            return block;
        }

        #endregion

        // Math
        #region Math

        public static double Clamp(double var, double min, double max)
        {
            if (var < min)
                var = min;
            else if (var > max)
                var = max;

            return var;
        }

        #endregion

    }
}
