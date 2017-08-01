using UnityEngine;

namespace Unite
{
    class Line
    {
        // Variables
        #region Variables

        private Point p;
        private Point q;

        #endregion

        // Properties
        #region Properties

        public Point P { get { return p; } }
        public Point Q { get { return q; } }

        #endregion

        // Overide
        #region Overide

        public Line(Point pointA, Point pointB)
        {
            Reallocate(pointA, pointB);
        }

        public override string ToString()
        {
            return "Point P: " + p + "   Point Q: " + q;
        }

        #endregion

        // Public
        #region Public

        public Vector GetDirectorVector()
        {
            return (p - q).AsVector().Normalized();
        }

        public void Reallocate(Point pointA, Point pointB)
        {
            p = pointA;
            q = pointB;
        }

        public bool IsCoincident(Line line)
        {
            return (IsParallel(line) && IsParallel(new Line(p, line.p)) && IsParallel(new Line(p, line.q)));
        }

        public bool IsParallel(Line line)
        {
            return (GetDirectorVector() == line.GetDirectorVector() || GetDirectorVector() == -line.GetDirectorVector());
        }

        #endregion

    }
}
