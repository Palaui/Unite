using UnityEngine;

namespace Unite
{
    public class Line
    {
        // Variables
        #region Variables

        private Point p;
        private Point q;
        private int dimension;

        #endregion

        // Properties
        #region Properties

        public Point P { get { return p; } }
        public Point Q { get { return q; } }
        public int Dimension { get { return dimension; } }

        #endregion

        // Override
        #region Override

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
            dimension = Mathf.Min(pointA.Dimension, pointB.Dimension);
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
