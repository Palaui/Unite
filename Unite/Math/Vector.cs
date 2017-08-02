using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    public class Vector
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

        public Vector(Vector2 pointA)
        {
            p.Add(pointA.x);
            p.Add(pointA.y);
            dimension = 2;
        }

        public Vector(Vector3 pointA)
        {
            p.Add(pointA.x);
            p.Add(pointA.y);
            p.Add(pointA.z);
            dimension = 3;
        }

        public Vector(List<float> point)
        {
            p = new List<float>(point);
            dimension = p.Count;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            Vector p = obj as Vector;
            if ((object)p == null)
                return false;

            return (p == this);
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        #endregion

        // Public
        #region Public

        public float Module()
        {
            float distSquared = 0;
            for (int i = 0; i < dimension; i++)
                distSquared += Mathf.Pow(p[i], 2);

            return Mathf.Sqrt(distSquared);
        }

        public Vector Normalized()
        {
            List<float> comps = new List<float>();
            float module = Module();

            for (int i = 0; i < dimension; i++)
                comps.Add(p[i] / module);

            return new Vector(comps);
        }

        public Point AsPoint()
        {
            return new Point(p);
        }

        #endregion

        // Operators
        #region Operators

        public static Vector operator +(Vector vecA, Vector vecB)
        {
            List<float> comps = new List<float>();
            int dim = Mathf.Max(vecA.dimension, vecB.dimension);

            for (int i = 0; i < dim; i++)
                comps.Add(vecA[i] + vecB[i]);

            return new Vector(comps);
        }

        public static Vector operator -(Vector vecA, Vector vecB)
        {
            List<float> comps = new List<float>();
            int dim = Mathf.Max(vecA.dimension, vecB.dimension);

            for (int i = 0; i < dim; i++)
                comps.Add(vecA[i] - vecB[i]);

            return new Vector(comps);
        }

        public static Vector operator -(Vector vec)
        {
            List<float> comps = new List<float>();

            for (int i = 0; i < vec.dimension; i++)
                comps.Add(-vec[i]);

            return new Vector(comps);
        }

        public static bool operator ==(Vector vecA, Vector vecB)
        {
            if (vecA.dimension != vecB.dimension)
                return false;

            for (int i = 0; i < vecA.dimension; i++)
            {
                if (vecA[i] != vecB[i])
                    return false;
            }

            return true;
        }

        public static bool operator !=(Vector vecA, Vector vecB)
        {
            return !(vecA == vecB);
        }

        #endregion
    }
}
