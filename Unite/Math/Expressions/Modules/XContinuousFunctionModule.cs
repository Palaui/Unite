using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    [ExecuteInEditMode]
    class XContinuousFunctionModule : MonoBehaviour
    {
        // Variables
        #region Variables

        [Header("Graphic Creation Variables")]
        public DoubleV2 limits;
        public List<DoubleV2> points;

        [Header("Graphic Draw Variables")]
        public double advancePerMinute;
        public double beatsPerMinute;

        [Header("Inspector Activation")]
        public bool play;
        public bool draw;

        private List<Polynomial> polynomial = new List<Polynomial>();
        private bool isInitialized = false;
        private bool isPaused = false;

        #endregion

        // Override
        #region Override

        void Update()
        {
            if (play)
            {
                if (!isInitialized)
                    Initialize();
                isPaused = !isPaused;
                play = false;
            }
            if (draw)
            {
                Draw();
                draw = false;
            }

            if (!isPaused && isInitialized)
                Advance();
        }

        #endregion

        // Public
        #region Public

        public void Initialize()
        {
            isInitialized = true;
        }

        public void Advance()
        {
            double advanceDist = (limits.y - limits.x) * (advancePerMinute / 60) * Time.deltaTime;
            for (int i = 0; i < points.Count; i++)
                points[i] = new DoubleV2(points[i].x - advanceDist, points[i].y);
            Draw();
        }

        public void Draw()
        {
            //Polynomial.GetInterpolationWithEndDerivate0(polynomial, points).Draw(limits, Color.blue);
        }

        #endregion

    }
}
