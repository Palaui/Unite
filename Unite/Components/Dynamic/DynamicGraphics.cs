using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    public class DynamicGraphics : MonoBehaviour
    {
        // Variables
        #region Variables

        private static Vector2 bounds = new Vector2(26, 56);
        private static int framesMeanPack;
        private static int framesThreshold;
        private static int framesBelow;
        private static int framesAbove;
        private static int currentQuality = 0;
        private static bool expensiveQualityUsed;

        private static List<float> frameTimes = new List<float>();
        private static float meanTime;
        private static float timeCounter;

        private static List<float> frameTimesDraw = new List<float>();
        private static string signDraw = "+";
        private static float fpsIntervalDraw;
        private static float meanTimeDraw;
        private static float timeCounterDraw;
        private static bool isDrawingAllowed = false;

        #endregion

        // Properties
        #region Properties

        public static int QualityLevel
        {
            get { return QualitySettings.GetQualityLevel(); }
        }

        public static float QualityLevel01
        {
            get { return Mathf.Lerp(0, 1, (float)QualitySettings.GetQualityLevel() / (QualitySettings.names.Length - 1)); }
        }

        #endregion

        // Override
        #region Override

        void Update()
        {
            CheckThresholds();
            CheckDrawTimes();
        }

        void OnGUI()
        {
            if (isDrawingAllowed)
            {
                if (meanTimeDraw < 0.1f || meanTimeDraw > 99999)
                    return;
                GUI.color = Color.red;
                GUI.Label(new Rect(10, 10, 500, 30), "FPS " + meanTimeDraw.ToString());
                GUI.Label(new Rect(10, 30, 500, 30), "Quality " + (QualitySettings.GetQualityLevel() + 1).ToString() + 
                    " / " + QualitySettings.names.Length + " " + signDraw);
            }
        }

        #endregion

        // Public Static
        #region Public Static

        public static void Activate(int lowerFPS = 26, int riseFPS = 56, int inFramesThreshold = 6, int inFramesMeanPack = 3)
        {
            Container.GetComponent<DynamicGraphics>();

            bounds = new Vector2(lowerFPS, riseFPS);
            framesMeanPack = inFramesMeanPack;
            framesThreshold = inFramesThreshold;
            timeCounter = 0;
            framesBelow = 0;
            framesAbove = 0;

            currentQuality = QualitySettings.GetQualityLevel();
            expensiveQualityUsed = true;
        }

        public static void Deactivate()
        {
            Container.RemoveComponent<DynamicGraphics>();
        }

        public static void BeginDrawFPS(float inFpsIntervalDraw = 0.4f)
        {
            fpsIntervalDraw = inFpsIntervalDraw;
            timeCounterDraw = 0;
            isDrawingAllowed = true;
        }

        public static void StopDrawFPS()
        {
            isDrawingAllowed = false;
        }

        #endregion

        // Private Static
        #region Private Static

        private static void CheckThresholds()
        {
            if (timeCounter < framesThreshold)
                frameTimes.Add(Time.deltaTime);
            else
            {
                timeCounter = 0;
                meanTime = 0;
                foreach (float frameTIme in frameTimes)
                    meanTime += frameTIme;
                meanTime = 1.0f / (meanTime / frameTimes.Count);
                frameTimes.Clear();

                if (meanTime < bounds.x)
                {
                    framesBelow++;
                    framesAbove = 0;

                    if (framesBelow >= framesMeanPack)
                        LowerGraphics();
                }
                else if (meanTime > bounds.y)
                {
                    framesBelow = 0;
                    framesAbove++;

                    if (framesAbove >= framesMeanPack)
                        RiseGraphics();
                }
                else
                {
                    framesBelow = 0;
                    framesAbove = 0;
                }
            }
            timeCounter++;
        }

        private static void LowerGraphics()
        {
            if (currentQuality > 0 || !expensiveQualityUsed)
            {
                if (expensiveQualityUsed)
                {
                    QualitySettings.SetQualityLevel(currentQuality - 1, false);
                    expensiveQualityUsed = false;
                    currentQuality--;
                    signDraw = "+";
                }
                else
                {
                    QualitySettings.SetQualityLevel(currentQuality, true);
                    expensiveQualityUsed = true;
                    signDraw = "-";
                }

                framesBelow = 0;
                framesAbove = 0;
            }
        }

        private static void RiseGraphics()
        {
            if (currentQuality < QualitySettings.names.Length - 1 || !expensiveQualityUsed)
            {
                if (!expensiveQualityUsed)
                {
                    QualitySettings.SetQualityLevel(currentQuality, true);
                    expensiveQualityUsed = true;
                    signDraw = "+";
                }
                else
                {
                    QualitySettings.SetQualityLevel(currentQuality + 1, false);
                    expensiveQualityUsed = false;
                    currentQuality++;
                    signDraw = "-";
                }

                framesBelow = 0;
                framesAbove = 0;
            }
        }

        private void CheckDrawTimes()
        {
            if (isDrawingAllowed)
            {
                if (timeCounterDraw < fpsIntervalDraw)
                    frameTimesDraw.Add(Time.deltaTime);
                else
                {
                    timeCounterDraw = 0;
                    meanTimeDraw = 0;
                    foreach (float frameTIme in frameTimesDraw)
                        meanTimeDraw += frameTIme;
                    meanTimeDraw = 1.0f / (meanTimeDraw / frameTimesDraw.Count);
                    frameTimesDraw.Clear();
                }
                timeCounterDraw += Time.deltaTime;
            }
        }

        #endregion

    }
}
