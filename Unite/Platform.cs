using UnityEngine;

namespace Unite
{
    public static class Platform
    {
        // Editor
        #region Editor

        public static bool IsEditor()
        {

#if UNITY_EDITOR

            return true;

#endif

            return false;

        }

        public static bool IsWindowsEditor()
        {

#if UNITY_EDITOR_WIN

            return true;

#endif

            return false;

        }

        public static bool IsMacOSEditor()
        {

#if UNITY_EDITOR_OSX

            return true;

#endif

            return false;

        }

        #endregion

        // Desktop
        #region Desktop

        public static bool IsDesktop()
        {

#if UNITY_STANDALONE

            return true;

#endif

            return false;

        }

        public static bool IsWindows()
        {

#if UNITY_STANDALONE_WIN

            return true;

#endif

            return false;

        }

        public static bool IsMacOS()
        {

#if UNITY_STANDALONE_OSX

            return true;

#endif

            return false;

        }

        public static bool IsLinux()
        {

#if UNITY_STANDALONE_LINUX

            return true;

#endif

            return false;

        }

        #endregion

        // Mobile
        #region Mobile

        public static bool IsAndroid()
        {

#if UNITY_ANDROID

            return true;

#endif

            return false;

        }

        public static bool IsIOS()
        {

#if UNITY_IOS

            return true;

#endif

            return false;

        }

        #endregion

    }
}
