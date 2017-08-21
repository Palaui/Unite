﻿using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    public class Polynomial
    {
        // Variables
        #region Variables

        private float[] coeficients;

        #endregion

        // Override
        #region Override

        public Polynomial(float[] polynomialCoeficients)
        {
            coeficients = polynomialCoeficients;
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

        public void AssignExpression(float[] coeficients)
        {
            this.coeficients = coeficients;
        }

        public float[] GetCoeficients()
        {
            return coeficients;
        }

        #endregion

        // Calculus
        #region Calculus

        public float Evaluate(float x)
        {
            float result = 0;
            for (int i = 0; i < coeficients.Length; i++)
                result += coeficients[i] * Mathf.Pow(x, (coeficients.Length - i) - 1);
            return result;
        }

        public Polynomial GetDerivate()
        {
            float[] coef = new float[coeficients.Length - 1];
            for (int i = 0; i < coeficients.Length - 1; i++)
                coef[i] = ((coeficients.Length - i) - 1) * coeficients[i];
            return new Polynomial(coef);
        }

        public Polynomial GetPrimitive()
        {
            float[] coef = new float[coeficients.Length + 1];
            for (int i = 0; i < coeficients.Length; i++)
                coef[i] = coeficients[i] / (coeficients.Length - i);
            coef[coeficients.Length] = 0;
            return new Polynomial(coef);
        }

        #endregion

        // Public Static
        #region Public Static

        public static Polynomial GetInterpolation(List<Vector2> points)
        {
            List<float> list = new List<float>();
            Matrix matrix = new Matrix(points.Count, points.Count);

            for (int i = 0; i < points.Count; i++)
            {
                for (int j = 0; j < points.Count; j++)
                    list.Add(Mathf.Pow(points[i].x, (points.Count - j) - 1));
                matrix.FillRow(Ext.CreateArrayFromList(list), i);
                list.Clear();
            }

            for (int i = 0; i < points.Count; i++)
                list.Add(points[i].y);

            return new Polynomial(matrix.Solve(Ext.CreateArrayFromList(list)));
        }

        public static Polynomial GetInterpolationWithEndDerivate0(List<Vector2> points)
        {
            List<float> list = new List<float>();
            Matrix matrix = new Matrix(points.Count + 2, points.Count + 2);

            for (int i = 0; i < points.Count; i++)
            {
                for (int j = 0; j < points.Count + 2; j++)
                    list.Add(Mathf.Pow(points[i].x, points.Count + 1 - j));
                matrix.FillRow(Ext.CreateArrayFromList(list), i);
                list.Clear();
            }

            for (int i = 0; i < points.Count + 1; i++)
                list.Add(Mathf.Pow(points[0].x, points.Count - i) * ((points.Count - i) + 1));
            list.Add(0);
            matrix.FillRow(Ext.CreateArrayFromList(list), points.Count);
            list.Clear();

            for (int i = 0; i < points.Count + 1; i++)
                list.Add(Mathf.Pow(points[points.Count - 1].x, points.Count - i) * ((points.Count - i) + 1));
            list.Add(0);
            matrix.FillRow(Ext.CreateArrayFromList(list), points.Count + 1);
            list.Clear();

            for (int i = 0; i < points.Count; i++)
                list.Add(points[i].y);
            list.Add(0);
            list.Add(0);

            return new Polynomial(matrix.Solve(Ext.CreateArrayFromList(list)));
        }

        #endregion

        #endregion
    }
}
