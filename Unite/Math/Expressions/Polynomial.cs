using System;
using System.Collections.Generic;

namespace Unite
{
    public class Polynomial : BaseXExpression
    {
        // Variables
        #region Variables

        private double[] coeficients;

        #endregion

        // Override
        #region Override

        public Polynomial(double[] polynomialCoeficients)
        {
            coeficients = polynomialCoeficients;
            expression = ToString();
        }

        public override string ToString()
        {
            string poly = "";
            for (int i = 0; i < coeficients.Length; i++)
            {
                if (i + 1 < coeficients.Length)
                    poly += coeficients[i] + " * x ^ " + ((coeficients.Length - i) - 1) + " + ";
                else
                    poly += coeficients[i] + " * x ^ " + ((coeficients.Length - i) - 1);
            }

            return poly;
        }

        #endregion

        // Public
        #region Public

        // Base
        #region Base

        public void AssignExpression(double[] coeficients)
        {
            this.coeficients = coeficients;
            expression = ToString();
        }

        public double[] GetCoeficients()
        {
            return coeficients;
        }

        #endregion

        // Calculus
        #region Calculus

        public override double Evaluate(double x)
        {
            double result = 0;
            for (int i = 0; i < coeficients.Length; i++)
                result += coeficients[i] * Math.Pow(x, (coeficients.Length - i) - 1);
            return result;
        }

        public Polynomial GetDerivate()
        {
            double[] coef = new double[coeficients.Length - 1];
            for (int i = 0; i < coeficients.Length - 1; i++)
                coef[i] = ((coeficients.Length - i) - 1) * coeficients[i];
            return new Polynomial(coef);
        }

        public Polynomial GetPrimitive()
        {
            double[] coef = new double[coeficients.Length + 1];
            for (int i = 0; i < coeficients.Length; i++)
                coef[i] = coeficients[i] / (coeficients.Length - i);
            coef[coeficients.Length] = 0;
            return new Polynomial(coef);
        }

        #endregion

        // Public Static
        #region Public Static

        public static Polynomial GetInterpolation(List<DoubleV2> points)
        { return GetInterpolation(new Polynomial(new double[] { }), points); }
        public static Polynomial GetInterpolation(Polynomial polynomial, List<DoubleV2> points)
        {
            List<double> list = new List<double>();
            Matrix matrix = new Matrix(points.Count, points.Count);

            for (int i = 0; i < points.Count; i++)
            {
                for (int j = 0; j < points.Count; j++)
                    list.Add(Math.Pow(points[i].x, (points.Count - j) - 1));
                matrix.FillRow(Ext.CreateArrayFromList(list), i);
                list.Clear();
            }

            for (int i = 0; i < points.Count; i++)
                list.Add(points[i].y);

            polynomial.AssignExpression(matrix.Solve(Ext.CreateArrayFromList(list)));
            return polynomial;
        }

        public static Polynomial GetInterpolationWithEndDerivate0(List<DoubleV2> points)
        { return GetInterpolationWithEndDerivate0(new Polynomial(new double[] { }), points); }
        public static Polynomial GetInterpolationWithEndDerivate0(Polynomial polynomial, List<DoubleV2> points)
        {
            List<double> list = new List<double>();
            Matrix matrix = new Matrix(points.Count + 2, points.Count + 2);

            for (int i = 0; i < points.Count; i++)
            {
                for (int j = 0; j < points.Count + 2; j++)
                    list.Add(Math.Pow(points[i].x, points.Count + 1 - j));
                matrix.FillRow(Ext.CreateArrayFromList(list), i);
                list.Clear();
            }

            for (int i = 0; i < points.Count + 1; i++)
                list.Add(Math.Pow(points[0].x, points.Count - i) * ((points.Count - i) + 1));
            list.Add(0);
            matrix.FillRow(Ext.CreateArrayFromList(list), points.Count);
            list.Clear();

            for (int i = 0; i < points.Count + 1; i++)
                list.Add(Math.Pow(points[points.Count - 1].x, points.Count - i) * ((points.Count - i) + 1));
            list.Add(0);
            matrix.FillRow(Ext.CreateArrayFromList(list), points.Count + 1);
            list.Clear();

            for (int i = 0; i < points.Count; i++)
                list.Add(points[i].y);
            list.Add(0);
            list.Add(0);

            polynomial.AssignExpression(matrix.Solve(Ext.CreateArrayFromList(list)));
            return polynomial;
        }

        #endregion

        #endregion
    }
}
