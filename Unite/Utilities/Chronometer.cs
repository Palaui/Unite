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
                return Time.fixedUnscaledTime - startTime;
            }
        }

        #endregion

        // Public
        #region Public

        public void Start()
        {
            startTime = Time.fixedUnscaledTime;
            stoppedTime = 0;
            isPaused = false;
        }

        public void Pause()
        {
            stoppedTime = Time.fixedUnscaledTime - startTime;
            isPaused = true;
        }

        public void Resume()
        {
            startTime = Time.fixedUnscaledTime - stoppedTime;
            isPaused = false;
        }

        public void Reset()
        {
            startTime = Time.fixedUnscaledTime;
            stoppedTime = 0;
        }

        #endregion

    }
}
