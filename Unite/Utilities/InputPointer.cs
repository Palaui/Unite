
using UnityEngine;

namespace Unite
{
    public class InputPointer
    {
        // Public Static
        #region Public static

        /// <summary> Checks if a mouse buttons has been just clicked or mobile touch has just started. </summary>
        /// <param name="buttonIndex"> Mouse button index or touch number. </param>
        public static bool IsButtonJustDown(int buttonIndex)
        {
            switch (buttonIndex)
            {
                case 0: return Input.GetKeyDown(KeyCode.Mouse0) || TouchState(TouchPhase.Began, 0);
                case 1: return Input.GetKeyDown(KeyCode.Mouse1) || TouchState(TouchPhase.Began, 1);
                case 2: return Input.GetKeyDown(KeyCode.Mouse2) || TouchState(TouchPhase.Began, 2);
                case 3: return Input.GetKeyDown(KeyCode.Mouse3) || TouchState(TouchPhase.Began, 3);
                case 4: return Input.GetKeyDown(KeyCode.Mouse4) || TouchState(TouchPhase.Began, 4);
                case 5: return Input.GetKeyDown(KeyCode.Mouse5) || TouchState(TouchPhase.Began, 5);
                case 6: return Input.GetKeyDown(KeyCode.Mouse6) || TouchState(TouchPhase.Began, 6);
            }

            return false;
        }

        /// <summary> Checks if a mouse button is being hold or mobile touch is touching. </summary>
        /// <param name="buttonIndex"> Mouse button index or touch number. </param>
        public static bool IsButtonDown(int buttonIndex)
        {
            switch (buttonIndex)
            {
                case 0:
                    return Input.GetKey(KeyCode.Mouse0) || TouchState(TouchPhase.Moved, 0) || TouchState(TouchPhase.Stationary, 0);
                case 1:
                    return Input.GetKey(KeyCode.Mouse1) || TouchState(TouchPhase.Moved, 1) || TouchState(TouchPhase.Stationary, 1);
                case 2:
                    return Input.GetKey(KeyCode.Mouse2) || TouchState(TouchPhase.Moved, 2) || TouchState(TouchPhase.Stationary, 2);
                case 3:
                    return Input.GetKey(KeyCode.Mouse3) || TouchState(TouchPhase.Moved, 3) || TouchState(TouchPhase.Stationary, 3);
                case 4:
                    return Input.GetKey(KeyCode.Mouse4) || TouchState(TouchPhase.Moved, 4) || TouchState(TouchPhase.Stationary, 4);
                case 5:
                    return Input.GetKey(KeyCode.Mouse5) || TouchState(TouchPhase.Moved, 5) || TouchState(TouchPhase.Stationary, 5);
                case 6:
                    return Input.GetKey(KeyCode.Mouse6) || TouchState(TouchPhase.Moved, 6) || TouchState(TouchPhase.Stationary, 6);
            }

            return false;
        }

        /// <summary> Checks if a mouse button has just stopped clicking or mobile touch has just end. </summary>
        /// <param name="buttonIndex"> Mouse button index or touch number. </param>
        public static bool IsButtonJustUp(int buttonIndex)
        {
            switch (buttonIndex)
            {
                case 0: return Input.GetKeyUp(KeyCode.Mouse0) || TouchState(TouchPhase.Ended, 0);
                case 1: return Input.GetKeyUp(KeyCode.Mouse1) || TouchState(TouchPhase.Ended, 1);
                case 2: return Input.GetKeyUp(KeyCode.Mouse2) || TouchState(TouchPhase.Ended, 2);
                case 3: return Input.GetKeyUp(KeyCode.Mouse3) || TouchState(TouchPhase.Ended, 3);
                case 4: return Input.GetKeyUp(KeyCode.Mouse4) || TouchState(TouchPhase.Ended, 4);
                case 5: return Input.GetKeyUp(KeyCode.Mouse5) || TouchState(TouchPhase.Ended, 5);
                case 6: return Input.GetKeyUp(KeyCode.Mouse6) || TouchState(TouchPhase.Ended, 6);
            }

            return false;
        }

        /// <summary> Checks if a mouse button has just stopped clicking or mobile touch has just end. </summary>
        /// <param name="buttonIndex"> Mouse button index or touch number. </param>
        public static Vector2 Location(int buttonIndex)
        {
            if (!Input.touchSupported || Application.platform == RuntimePlatform.WebGLPlayer)
                return Input.mousePosition;

            if (Input.touchCount <= buttonIndex)
                return Vector2.one * -1;

            switch (buttonIndex)
            {
                case 0: return Input.touches[0].position;
                case 1: return Input.touches[1].position;
                case 2: return Input.touches[2].position;
                case 3: return Input.touches[3].position;
                case 4: return Input.touches[4].position;
                case 5: return Input.touches[5].position;
                case 6: return Input.touches[6].position;
            }

            return Vector2.one * -1;
        }

        #endregion

        // Private Static
        #region Private Static

        /// <summary> Checks if a Touch index exists and if so, if is in a certain phase. </summary>
        /// <param name="phase"> Phase to check the index with. </param>
        /// <param name="index"> Index to check if it's in a specific phase. </param>
        private static bool TouchState(TouchPhase phase, int index)
        {
            if (Input.touchCount <= index)
                return false;

            if (Input.touches[index].phase == phase)
                return true;

            return false;
        }

        #endregion

    }
}
