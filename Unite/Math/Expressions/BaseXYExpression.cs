using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    public abstract class BaseXYExpression : BaseExpression
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

        public abstract double Evaluate(double x, double y);

        public DoubleV3[,] GetGraphicPoints(DoubleV2 xDomain, DoubleV2 yDomain, int side)
        { return GetGraphicPoints(xDomain, yDomain, Math.Abs(yDomain.x - xDomain.x) / side, Math.Abs(yDomain.y - xDomain.y) / side); }
        public DoubleV3[,] GetGraphicPoints(DoubleV2 xDomain, DoubleV2 yDomain, double inStepX = 0.0675f, double inStepY = 0.0675f)
        {
            List<List<DoubleV3>> points = new List<List<DoubleV3>>();
            List<DoubleV3> subPoints = new List<DoubleV3>();
            double x, y;
            inStepX = Ext.Clamp(inStepX, 0.001f, 1000);
            inStepY = Ext.Clamp(inStepY, 0.001f, 1000);

            for (double i = xDomain.x; i <= xDomain.y; i += inStepX)
            {
                if (i > -0.001f && i < 0.001f) { i = 0; }
                x = i;
                subPoints = new List<DoubleV3>();

                for (double j = yDomain.x; j <= yDomain.y; j += inStepY)
                {
                    if (j > -0.001f && j < 0.001f) { j = 0; }
                    y = j;
                    subPoints.Add(new DoubleV3(i, Evaluate(x, y), j));
                }
                points.Add(subPoints);
            }

            return Ext.CreateArrayFromList(points);
        }

        #endregion

        // Draw
        #region Draw

        public GameObject Draw(DoubleV2 xDomain, DoubleV2 yDomain, Color color,  double inStep = 0.0625f, bool eraseReference = true)
        {
            if (eraseReference && drawReference)
                UnityEngine.Object.DestroyImmediate(drawReference);

            DoubleV3[,] graphicsPoints = GetGraphicPoints(xDomain, yDomain, inStep, inStep);
            drawReference = ProcMesh.BuildPlane(graphicsPoints);
            Ext.ResetTransform(drawReference);

            currentStep = inStep;
            hasBeenDrawn = true;

            return drawReference;
        }

        public GameObject UpdateDraw(DoubleV2 xDomain, DoubleV2 yDomain, double time = 2.5f)
        {
            if (!hasBeenDrawn)
            {
                Debug.LogError("BaseXExpression UpdateDraw, Unable to update draw of an object not previously drawn");
                return drawReference;
            }

            DoubleV3[,] graphicsPoints = GetGraphicPoints(xDomain, yDomain, currentStep, currentStep);
            ProcMesh.UpdatePlane(drawReference, graphicsPoints, time);
            Ext.ResetTransform(drawReference);
            return drawReference;
        }

        #endregion

        #endregion
    }
}
