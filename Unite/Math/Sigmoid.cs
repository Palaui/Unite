using UnityEngine;

namespace Unite
{
    public class Sigmoid
    {
        public static double Output(double x)
        {
            if (x < -45.0)
                return 0;
            if (x > 45.0)
                return 1;

            return 1.0 / (1.0 + Mathf.Exp((float)-x));
        }

        public static double Derivative(double x)
        {
            return x * (1 - x);
        }
    }
}
