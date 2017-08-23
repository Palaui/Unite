using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    // Enums
    #region Enums

    public enum ExpressionOrder { Basic, Derivative, Primitive }

    #endregion

    public abstract class BaseXExpression
    {
        // Variables
        #region Variables

        protected GameObject drawReference;
        protected string expression;

        #endregion

        // Override
        #region Override

        public override string ToString()
        {
            return expression;
        }

        #endregion

        // Public
        #region Public

        // Calculus
        #region Calculus

        public abstract float Evaluate(float x);

        public List<float> GetSolutions(Vector2 bounds, float step, float y, float tol = 0.0675f)
        {
            List<float> list = new List<float>();
            Vector2 limits;
            float pass = bounds.x;

            while (pass < bounds.y)
            {
                if (CheckUpAndBelowValues(new Vector2(pass, pass + step), y, out limits))
                    list.Add(BisectionRecursion(limits, y, tol));
                pass += step;
            }

            return list;
        }

        public List<Vector2> GetGraphicPoints(Vector2 domain, float inStep = 0.0675f)
        {
            List<Vector2> points = new List<Vector2>();
            inStep = Mathf.Clamp(inStep, 0.001f, 1000);

            for (float f = domain.x; f <= domain.y; f += inStep)
            {
                if (f > -0.001f && f < 0.001f) { f = 0; }
                points.Add(new Vector2(f, Evaluate(f)));
            }

            return points;
        }
        public List<Vector2> GetGraphicPoints(Vector2 domain, int numberOfSteps)
        {
            List<Vector2> points = new List<Vector2>();
            float step = (domain.y - domain.x) / numberOfSteps;
            float f;

            for (int i = 0; i <= numberOfSteps; i++)
            {
                f = domain.x + i * step;
                if (f > -0.001f && f < 0.001f) { f = 0; }
                points.Add(new Vector2(f, Evaluate(f)));
            }

            return points;
        }

        #endregion

        // Draw
        #region Draw

        public GameObject Draw(Vector2 limits, Color color, float inStep = 0.0625f, float width = 0.005f, float z = 0, bool eraseReference = true)
        {
            if (eraseReference)
                Object.DestroyImmediate(drawReference);

            List<Vector2> graphicsPoints = GetGraphicPoints(new Vector2(limits.x, limits.y), inStep);
            drawReference = ProcMesh.BuildLine(Ext.CreateArrayFromList(Ext.ConvertList2DTo3D(graphicsPoints, 0)), color, width);
            Ext.ResetTransform(drawReference);
            drawReference.transform.position = new Vector3(0, 0, z);
            return drawReference;
        }

        #endregion

        #endregion

        // Private
        #region Private

        private bool CheckUpAndBelowValues(Vector2 initialBounds, float y, out Vector2 resultBounds)
        {
            float xEvalue = Evaluate(initialBounds.x);
            float yEvalue = Evaluate(initialBounds.y);

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
            resultBounds = Vector2.zero;
            return false;
        }

        private float BisectionRecursion(Vector2 limits, float y, float tol)
        {
            float param;
            float evalue;
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
            while (Mathf.Abs(evalue - y) > tol);

            return param;
        }

        #endregion

    }
}
