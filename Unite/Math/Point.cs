using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    public class Point
    {
        // Variables
        #region Variables

        private List<float> p = new List<float>();
        private int dimension;

        #endregion

        // Properties
        #region Properties

        public List<float> P { get { return p; } }
        public int Dimension { get { return dimension; } }

        public float this[int index]
        {
            get
            {
                return p[index];
            }
        }

        #endregion

        // Overide
        #region Overide

        public Point(Vector2 pointA)
        {
            p.Add(pointA.x);
            p.Add(pointA.y);
            dimension = 2;
        }

        public Point(Vector3 pointA)
        {
            p.Add(pointA.x);
            p.Add(pointA.y);
            p.Add(pointA.z);
            dimension = 3;
        }

        public Point(List<float> point)
        {
            p = new List<float>(point);
            dimension = p.Count;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            Point p = obj as Point;
            if ((object)p == null)
                return false;

            return (p == this);
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString()
        {
            string str = "Point:";
            foreach (float f in p)
                str += " " + f;
            return str;
        }

        #endregion

        // Public
        #region Public

        public void AddDimension(float f)
        {
            p.Add(f);
            dimension++;
        }

        public void RemoveDimension()
        {
            p.RemoveAt(p.Count - 1);
            dimension--;
        }

        public void SetDimension(int dim)
        {
            if (dimension == dim)
                return;

            while (dimension < dim)
                AddDimension(0);
            while (dimension > dim)
                RemoveDimension();  
        }

        public Vector AsVector()
        {
            return new Vector(p);
        }

        #endregion

        // Public Static
        #region Public Static

        public static Point GetRedimensionedPoint(Point referencePoint, int dim)
        {
            Point point = new Point(referencePoint.p);

            if (referencePoint.dimension == dim)
                return point;

            while (point.dimension < dim)
                point.AddDimension(0);
            while (point.dimension > dim)
                point.RemoveDimension();

            return point;
        }

        public static float Dist(Point pointA, Point pointB)
        {
            int dim = Mathf.Max(pointA.dimension, pointB.dimension);

            float distSquared = 0;
            for (int i = 0; i < dim; i++)
                distSquared += Mathf.Pow(pointB[i] - pointA[i], 2);

            return Mathf.Sqrt(distSquared);
        }

        #endregion

        // Operators
        #region Operators

        public static Point operator +(Point pointA, Point pointB)
        {
            List<float> comps = new List<float>();
            int dim = Mathf.Max(pointA.dimension, pointB.dimension);

            for (int i = 0; i < dim; i++)
                comps.Add(pointA[i] + pointB[i]);

            return new Point(comps);
        }

        public static Point operator -(Point pointA, Point pointB)
        {
            List<float> comps = new List<float>();
            int dim = Mathf.Max(pointA.dimension, pointB.dimension);

            for (int i = 0; i < dim; i++)
                comps.Add(pointA[i] - pointB[i]);

            return new Point(comps);
        }

        public static Point operator -(Point vec)
        {
            List<float> comps = new List<float>();

            for (int i = 0; i < vec.dimension; i++)
                comps.Add(-vec[i]);

            return new Point(comps);
        }

        public static bool operator ==(Point pointA, Point pointB)
        {
            if (pointA.dimension != pointB.dimension)
                return false;
            
            for (int i = 0; i < pointA.dimension; i++)
            {
                if (pointA[i] != pointB[i])
                    return false;
            }

            return true;
        }

        public static bool operator !=(Point pointA, Point pointB)
        {
            return !(pointA == pointB);
        }

        #endregion
    }
}
