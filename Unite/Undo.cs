using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    public static class Undo
    {
        // Variables
        #region Variables

        private static List<Action> cachedBaseMethods = new List<Action>();
        private static List<Action> cachedInverseMethods = new List<Action>();
        private static int index = -1;

        #endregion

        // Public Static
        #region Public Static

        public static void Add(Action baseLambdaMethod, Action inverseLambdaMethod, bool call)
        {
            for (int i = cachedInverseMethods.Count - 1; i > index; i--)
                cachedInverseMethods.RemoveAt(i);

            cachedBaseMethods.Add(baseLambdaMethod);
            cachedInverseMethods.Add(inverseLambdaMethod);
            index++;

            if (call)
                cachedBaseMethods[index]();
        }

        public static bool Last()
        {
            if (index < 0)
                return false;

            cachedInverseMethods[index]();
            index--;
            return true;
        }
        public static void Last(int n)
        {
            for (int i = 0; i < n; i++)
            {
                if (!Last())
                    return;
            }
        }

        public static void LastAll()
        {
            while(Last()) { }
        }

        public static bool Next()
        {
            if ((index + 1) >= cachedInverseMethods.Count)
                return false;

            index++;
            cachedBaseMethods[index]();
            return true;
        }
        public static void Next(int n)
        {
            for (int i = 0; i < n; i++)
            {
                if (!Next())
                    return;
            }
        }

        public static void NextAll()
        {
            while (Next()) { }
        }

        public static void Clear()
        {
            cachedBaseMethods = new List<Action>();
            cachedInverseMethods = new List<Action>();
            index = -1;
        }

        #endregion

        // Usage Example
        #region Usage Example

        /*

        // A is called and creates an undo for itself
        public static void A(string str)
        {
            cachedMethods.Add(() => InverseA(str));
        }

        public static void ControlZPressed()
        {
            Undo.Last();
        }

        public static void InverseA(string str)
        {
            // Does something to revert A
        }

        */

        #endregion

    }
}
