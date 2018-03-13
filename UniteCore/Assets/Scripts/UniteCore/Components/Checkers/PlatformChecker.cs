using Unite;
using UnityEngine;

namespace UniteCore
{
    public class PlatformChecker : MonoBehaviour
    {
        // Private
        #region Private

        // Editor
        #region Editor

        private bool IsEditorCheck()
        {
            bool answer = false;

#if UNITY_EDITOR

            answer = true;

#endif

            return answer;
        }

        private bool IsWindowsEditorCheck()
        {
            bool answer = false;

#if UNITY_EDITOR_WIN

            answer = true;

#endif

            return answer;
        }

        private bool IsMacOSEditorCheck()
        {
            bool answer = false;

#if UNITY_EDITOR_OSX

            answer = true;

#endif

            return answer;
        }

        #endregion

        // Desktop
        #region Desktop

        private bool IsDesktopCheck()
        {
            bool answer = false;

#if UNITY_STANDALONE

            answer = true;

#endif

            return answer;
        }

        private bool IsWindowsCheck()
        {
            bool answer = false;

#if UNITY_STANDALONE_WIN

            answer = true;

#endif

            return answer;
        }

        private bool IsMacOSCheck()
        {
            bool answer = false;

#if UNITY_STANDALONE_OSX

            answer = true;

#endif

            return answer;
        }

        private bool IsLinuxCheck()
        {
            bool answer = false;

#if UNITY_STANDALONE_LINUX

            answer = true;

#endif

            return answer;
        }

        #endregion

        // Mobile
        #region Mobile

        private bool IsAndroidCheck()
        {
            bool answer = false;

#if UNITY_ANDROID

            answer = true;

#endif

            return answer;
        }

        private bool IsIOSCheck()
        {
            bool answer = false;

#if UNITY_IOS

            answer = true;

#endif

            return answer;
        }

        #endregion

        #endregion

        // Public Static
        #region Public Static

        // Editor
        #region Editor

        public static bool IsEditor()
        {
            return Container.GetComponent<PlatformChecker>().IsEditorCheck();
        }

        public static bool IsWindowsEditor()
        {
            return Container.GetComponent<PlatformChecker>().IsWindowsEditorCheck();
        }

        public static bool IsMacOSEditor()
        {
            return Container.GetComponent<PlatformChecker>().IsMacOSEditorCheck();
        }

        #endregion

        // Desktop
        #region Desktop

        public static bool IsDesktop()
        {
            return Container.GetComponent<PlatformChecker>().IsDesktopCheck();
        }

        public static bool IsWindows()
        {
            return Container.GetComponent<PlatformChecker>().IsWindowsCheck();
        }

        public static bool IsMacOS()
        {
            return Container.GetComponent<PlatformChecker>().IsMacOSCheck();
        }

        public static bool IsLinux()
        {
            return Container.GetComponent<PlatformChecker>().IsLinuxCheck();
        }

        #endregion

        // Mobile
        #region Mobile

        public static bool IsAndroid()
        {
            return Container.GetComponent<PlatformChecker>().IsAndroidCheck();
        }

        public static bool IsIOS()
        {
            return Container.GetComponent<PlatformChecker>().IsIOSCheck();
        }

        #endregion

        #endregion
    }
}
