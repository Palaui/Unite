using UnityEngine;
using System.Linq;
using System.Reflection;
using System;

namespace Unite
{
    public class Reflect
    {
        // Variables
        #region Variables

        private string[] fields;
        private string[] methods;

        #endregion

        // Public
        #region Public

        public void BuildMethodsArray(Type type, string[] ignoredList)
        {
            if (ignoredList != null)
                methods = GetMethodsArray(type, ignoredList);
        }

        public void BuildFieldsArray(Type type, string[] ignoredList)
        {
            if (ignoredList != null)
                fields = GetFieldsArray(type, ignoredList);
        }

        #endregion

        // Public Static
        #region Public Static

        public static string[] GetMethodsArray(Type type, string[] ignoredList)
        {
            if (ignoredList != null)
            {
                string[] methodsArray = type
                    .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .Where(x => x.DeclaringType == type)
                    .Where(x => !ignoredList.Any(n => n == x.Name))
                    .Where(x => x.GetParameters().Length == 0)
                    .Select(x => x.Name)
                    .ToArray();

                return methodsArray;
            }

            Debug.Log("The list of ignored methods cannot be null, pass an empty array instead");
            return null;
        }

        public static MethodInfo[] GetAllMethods(Type type)
        {
            return type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        }

        public static string[] GetFieldsArray(Type type, string[] ignoredList)
        {
            if (ignoredList != null)
            {
                string[] fieldsArray = type
                    .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .Where(x => x.DeclaringType == type)
                    .Where(x => !ignoredList.Any(n => n == x.Name))
                    .Select(x => x.Name)
                    .ToArray();

                return fieldsArray;
            }

            Debug.Log("The list of ignored fields cannot be null, pass an empty array instead");
            return null;
        }

        public static FieldInfo[] GetAllFields(Type type)
        {
            return type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        }

        #endregion
    }
}
