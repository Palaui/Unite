using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    public abstract class BaseXExpression : BaseExpression
    {
        // Override
        #region Override

        public override string ToString()
        {
            return base.ToString();
        }

        #endregion

        // Public
        #region Public

        // Calculus
        #region Calculus

        public abstract double Evaluate(double x);

        public List<double> GetSolutions(Vector2 bounds, double step, double y, double tol = 0.0675f)
        {
            List<double> list = new List<double>();
            DoubleV2 limits;
            double pass = bounds.x;

            while (pass < bounds.y)
            {
                if (CheckUpAndBelowValues(new DoubleV2(pass, pass + step), y, out limits))
                    list.Add(BisectionRecursion(limits, y, tol));
                pass += step;
            }

            return list;
        }

        public List<DoubleV2> GetGraphicPoints(DoubleV2 domain, double inStep = 0.0675f)
        {
            int numberOfSteps = (int)Math.Floor(Math.Abs(domain.y - domain.x) / inStep);
            return GetGraphicPoints(domain, numberOfSteps);
        }
        public List<DoubleV2> GetGraphicPoints(DoubleV2 domain, int numberOfSteps)
        {
            List<DoubleV2> points = new List<DoubleV2>();
            double length = Math.Abs(domain.y - domain.x);
            double f;

            for (int i = 0; i <= numberOfSteps; i++)
            {
                f = domain.x + (i / (double)numberOfSteps) * length;
                Debug.Log(f);
                if (f > -0.001f && f < 0.001f) { f = 0; }
                points.Add(new DoubleV2(f, Evaluate(f)));
            }

            return points;
        }

        #endregion

        // Draw
        #region Draw

        public GameObject Draw(DoubleV2 domain, Color color, double inStep = 0.0625f, double width = 0.005f, double z = 0, bool eraseReference = true)
        {
            if (eraseReference && drawReference)
                UnityEngine.Object.DestroyImmediate(drawReference);

            List<DoubleV2> graphicsPoints = GetGraphicPoints(domain, inStep);
            drawReference = ProcMesh.BuildLine(Ext.CreateArrayFromList(Ext.ConvertList2DTo3D(graphicsPoints, z)), color, width);
            Ext.ResetTransform(drawReference);

            currentStep = inStep;
            hasBeenDrawn = true;

            return drawReference;
        }

        public GameObject UpdateDraw(DoubleV2 domain, double time = 2.5f, double width = 0.005f, double z = 0)
        {
            if (!hasBeenDrawn)
            {
                Debug.LogError("BaseXExpression UpdateDraw, Unable to update draw of an object not previously drawn");
                return drawReference;
            }

            List<DoubleV2> graphicsPoints = GetGraphicPoints(domain, currentStep);
            ProcMesh.UpdateLine(drawReference, Ext.CreateArrayFromList(Ext.ConvertList2DTo3D(graphicsPoints, z)), time);
            Ext.ResetTransform(drawReference);
            return drawReference;
        }

        #endregion

        #endregion

        // Private
        #region Private

        private bool CheckUpAndBelowValues(DoubleV2 initialBounds, double y, out DoubleV2 resultBounds)
        {
            double xEvalue = Evaluate(initialBounds.x);
            double yEvalue = Evaluate(initialBounds.y);

            if (xEvalue >= y && yEvalue <= y)
            {
                resultBounds.x = initialBounds.y;
                resultBounds.y = initialBounds.x;
                return true;
            }
            else if (xEvalue <= y && yEvalue >= y)
            {
                resultBounds = initialBounds;
                return true;
            }
            resultBounds = new DoubleV2(0, 0);
            return false;
        }

        private double BisectionRecursion(DoubleV2 limits, double y, double tol)
        {
            double param;
            double evalue;
            int counter = 0;
            int maxSteps = 1000;

            do
            {
                param = (limits.x + limits.y) / 2;
                evalue = Evaluate(param);
                if (evalue > y)
                    limits.y = param;
                else
                    limits.x = param;

                counter++;
                if (counter > maxSteps)
                    return param;
            }
            while (Math.Abs(evalue - y) > tol);

            return param;
        }

        #endregion

    }
}
