using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    public class ExpressionModule : MonoBehaviour
    {
        // Variables
        #region Variables

        [Header("Graphic Creation Variables")]
        public DoubleV2 limits;
        public List<DoubleV2> points;
        public string xExpressionString;
        public string xyExpressionString;

        [Header("Inspector Activation")]
        public bool drawPolynomial;
        public bool drawXExpression;
        public bool drawXYExpression;

        private Polynomial polynomial;
        private XExpression xExpression;
        private XYExpression xyExpression;

        private bool polynomialDrawn = false;
        private bool xExpressionDrawn = false;
        private bool xyExpressionDrawn = false;

        #endregion

        // Override
        #region Override

        void Awake()
        {
            polynomial = new Polynomial(new double[] { });
            xExpression = new XExpression("");
            xyExpression = new XYExpression("");
        }

        void Update()
        {
            if (drawPolynomial)
            {
                DrawPolynomial();
                drawPolynomial = false;
            }

            if (drawXExpression)
            {
                DrawXExpression();
                drawXExpression = false;
            }

            if (drawXYExpression)
            {
                DrawXYExpression();
                drawXYExpression = false;
            }
        }

        #endregion

        // Private
        #region Private

        private void DrawPolynomial()
        {
            if (!polynomialDrawn)
            {
                Polynomial.GetInterpolationWithEndDerivate0(polynomial, points).Draw(limits, Color.green, 0.0675, 0.015)
                    .transform.position = Camera.main.transform.position + Camera.main.transform.forward * 4;
                polynomialDrawn = true;
            }
            else
                Polynomial.GetInterpolationWithEndDerivate0(polynomial, points).UpdateDraw(limits, 3)
                    .transform.position = Camera.main.transform.position + Camera.main.transform.forward * 4;
        }

        private void DrawXExpression()
        {
            if (!xExpressionDrawn)
            {
                xExpression.AssignExpression(xExpressionString).Draw(limits, Color.blue, 0.0675, 0.015)
                    .transform.position = Camera.main.transform.position + Camera.main.transform.forward * 4;
                xExpressionDrawn = true;
            }
            else
                xExpression.AssignExpression(xExpressionString).UpdateDraw(limits, 3)
                    .transform.position = Camera.main.transform.position + Camera.main.transform.forward * 4;
        }

        private void DrawXYExpression()
        {
            if (!xyExpressionDrawn)
            {
                xyExpression.AssignExpression(xyExpressionString).Draw(limits, limits, Color.blue, 0.0675)
                    .transform.position = Camera.main.transform.position + Camera.main.transform.forward * 8;
                xExpressionDrawn = true;
            }
            else
                xyExpression.AssignExpression(xyExpressionString).UpdateDraw(limits, limits, 3)
                    .transform.position = Camera.main.transform.position + Camera.main.transform.forward * 8;
        }

        #endregion

    }
}
