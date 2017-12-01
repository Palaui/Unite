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

        /// <summary>
        /// Adds a lambda method to be called in the future and an option to automatically call the current one.
        /// </summary>
        /// <param name="baseLambdaMethod">
        /// Main method to be fired, if not called from inside the method to invert, use call = true.
        /// </param>
        /// <param name="inverseLambdaMethod"> Inverse method, will be called with Last(). </param>
        /// <param name="call"> If true, will fire baseLambdaMethod immediately. </param>
        public static void Add(Action baseLambdaMethod, Action inverseLambdaMethod, bool call)
        {
            for (int i = cachedInverseMethods.Count - 1; i > index; i--)
            {
                cachedBaseMethods.RemoveAt(i);
                cachedInverseMethods.RemoveAt(i);
            }

            cachedBaseMethods.Add(baseLambdaMethod);
            cachedInverseMethods.Add(inverseLambdaMethod);
            index++;

            if (call)
                cachedBaseMethods[index]();
        }

        /// <summary> Tries to call the last added method to the undo pile. </summary>
        /// <returns> Returns if we've been able to call a method from the pile. </returns>
        public static bool Last()
        {
            if (index < 0)
                return false;

            cachedInverseMethods[index]();
            index--;
            return true;
        }
        /// <summary>
        /// Tries to call the "n" last added methods to the undo pile, where n is the param passed. 
        /// </summary>
        /// <param name="n"> Number of times we want to call Last(). </param>
        public static void Last(int n)
        {
            for (int i = 0; i < n; i++)
            {
                if (!Last())
                    return;
            }
        }

        /// <summary> Tries to call every method in the pile, undoing every registered call. </summary>
        public static void LastAll()
        {
            while(Last()) { }
        }

        /// <summary> Tries to call the last undone method called by Last(). </summary>
        /// <returns> Returns if we've been able to call a method from the pile. </returns>
        public static bool Next()
        {
            if ((index + 1) >= cachedInverseMethods.Count)
                return false;

            index++;
            cachedBaseMethods[index]();
            return true;
        }
        /// <summary> Tries to call the "n" last undone methods in the pile, where n is the param passed. </summary>
        /// <param name="n"> Number of times we want to call Next(). </param>
        public static void Next(int n)
        {
            for (int i = 0; i < n; i++)
            {
                if (!Next())
                    return;
            }
        }

        /// <summary> Tries to call every undone method in the pile, undoing every registered call. </summary>
        public static void NextAll()
        {
            while (Next()) { }
        }

        /// <summary> Clears all cached methods, erasing all data till this moment. </summary>
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

        // This method calls A and creates an undo for itself (In order to avoid A creating an undo)
        public static void SomeMethod(string str)
        {
            Undo.Add(() => BaseMethod(str), () => InverseA(str), true);
        }

        // A is called and creates an undo for itself
        public static void A(string str)
        {
            Undo.Add(() => BaseMethod(str), () => InverseA(str), false);
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
