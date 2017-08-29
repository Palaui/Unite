using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    public static class Ext
    {
        // Components
        #region Components

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

        #endregion

        // Method Call
        #region Methods Call

        public static void ApplyForeach<T>(Action<T> method, List<T> list)
        {
            foreach (T element in list)
                method(element);
        }
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
