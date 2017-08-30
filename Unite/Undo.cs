using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    public static class Undo
    {
        // Variables
        #region Variables

        private static List<Action> cachedMethods = new List<Action>();

        #endregion

        // Public Static
        #region Public Static

        public static void Add(Action inverseLambdaMethod) 
        {
            cachedMethods.Add(inverseLambdaMethod);
        }

        public static void Last()
        {
            if (cachedMethods.Count < 1)
                Debug.LogError("Undo Last: Unable to Undo");

            cachedMethods[cachedMethods.Count - 1]();
            cachedMethods.RemoveAt(cachedMethods.Count - 1);
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
