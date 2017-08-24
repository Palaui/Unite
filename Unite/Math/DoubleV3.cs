using System;
using UnityEngine;

namespace Unite
{
    [System.Serializable]
    public struct DoubleV3
    {
        // Variables
        #region Variables

        public double x;
        public double y;
        public double z;

        #endregion

        // Override
        #region Override

        public DoubleV3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public DoubleV3(double x, double y)
        {
            this.x = x;
            this.y = y;
            z = 0;
        }

        #endregion

        // Public
        #region Public

        public double Module()
        {
            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));
        }

        public DoubleV3 Normalize()
        {
            return new DoubleV3(x, y, z) / Module();
        }

        #endregion

        // Public Static
        #region Public Static

        public static double Distance(DoubleV3 v1, DoubleV3 v2)
        {
            return (v2 - v1).Module();
        }

        #endregion

        // Operators
        #region Operators

        public static DoubleV3 operator +(DoubleV3 v1, DoubleV3 v2) { return new DoubleV3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z); }
        public static DoubleV3 operator -(DoubleV3 v1, DoubleV3 v2) { return new DoubleV3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z); }
        public static DoubleV3 operator *(DoubleV3 v, double k) { return new DoubleV3(v.x * k, v.y * k, v.z * k); }
        public static DoubleV3 operator /(DoubleV3 v, double k) { return new DoubleV3(v.x / k, v.y / k, v.z / k); }

        public static implicit operator Vector3(DoubleV3 d) { return new Vector3((float)d.x, (float)d.y, (float)d.z); }

        #endregion

    }
}
