using UnityEngine;

namespace Unite
{
    public class Chronometer
    {
        // Variables
        #region Variables

        private float startTime;
        private float stoppedTime;
        private bool isPaused = true;

        #endregion

        // Properties
        #region Properties

        public float Value
        {
            get
            {
                if (isPaused)
                    return stoppedTime;
                return Time.realtimeSinceStartup - startTime;
            }
        }

        #endregion

        // Public
        #region Public

        public void Start()
        {
            startTime = Time.realtimeSinceStartup;
            stoppedTime = 0;
            isPaused = false;
        }

        public void Pause()
        {
            stoppedTime = Time.realtimeSinceStartup - startTime;
            isPaused = true;
        }

        public void Stop()
        {
            stoppedTime = 0;
            isPaused = true;
        }

        public void Resume()
        {
            startTime = Time.realtimeSinceStartup - stoppedTime;
            isPaused = false;
        }

        public void Reset()
        {
            startTime = Time.realtimeSinceStartup;
            stoppedTime = 0;
        }

        public void DebugValue(string prefix = "")
        {
            Debug.Log(prefix + " " + Value);
        }

        #endregion

    }
}
