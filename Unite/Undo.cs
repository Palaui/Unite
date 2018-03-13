using System;
using System.Collections.Generic;

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

        /// <summary> Adds an action and an inverse one to be able to undo and redo actions. </summary>
        /// <param name="baseAction"> Base method. </param>
        /// <param name="inverseAction"> Inverse method. </param>
        /// <param name="call"> If true, will fire baseAction immediately. </param>
        public static void Add(Action baseAction, Action inverseAction, bool call)
        {
            for (int i = cachedInverseMethods.Count - 1; i > index; i--)
            {
                cachedBaseMethods.RemoveAt(i);
                cachedInverseMethods.RemoveAt(i);
            }

            cachedBaseMethods.Add(baseAction);
            cachedInverseMethods.Add(inverseAction);
            
            if (call)
            {
                baseAction.Invoke();
                index++;
            }
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

    }
}
