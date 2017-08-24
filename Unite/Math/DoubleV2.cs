using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Unite
{
    [System.Serializable]
    public struct DoubleV2
    {
        // Variables
        #region Variables

        public double x;
        public double y;

        #endregion

        // Override
        #region Override

        public DoubleV2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        #endregion

        // Public
        #region Public

        public double Module()
        {
            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
        }

        public DoubleV2 Normalize()
        {
            return new DoubleV2(x, y) / Module();
        }

        #endregion

        // Public Static
        #region Public Static

        public static double Distance(DoubleV2 v1, DoubleV2 v2)
        {
            return (v2 - v1).Module();
        }

        #endregion

        // Operators
        #region Operators

        public static DoubleV2 operator +(DoubleV2 v1, DoubleV2 v2) { return new DoubleV2(v1.x + v2.x, v1.y + v2.y); }
        public static DoubleV2 operator -(DoubleV2 v1, DoubleV2 v2) { return new DoubleV2(v1.x - v2.x, v1.y - v2.y); }
        public static DoubleV2 operator *(DoubleV2 v, double k) { return new DoubleV2(v.x * k, v.y * k); }
        public static DoubleV2 operator /(DoubleV2 v, double k) { return new DoubleV2(v.x / k, v.y / k); }

        public static implicit operator DoubleV3(DoubleV2 d) { return new DoubleV3(d.x, d.y, 0); }
        public static implicit operator Vector2(DoubleV2 d) { return new Vector2((float)d.x, (float)d.y); }

        #endregion
    }
}
