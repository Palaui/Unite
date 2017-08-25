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

        [Header("Inspector Activation")]
        public bool drawPolynomial;
        public bool drawXExpression;

        private Polynomial polynomial;
        private XExpression xExpression;
        private bool polynomialDrawn = false;
        private bool xExpressionDrawn = false;

        #endregion

        // Override
        #region Override

        void Awake()
        {
            polynomial = new Polynomial(new double[] { });
            xExpression = new XExpression("");
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
        }

        #endregion

        // Private
        #region Private

        private void DrawPolynomial()
        {
            if (!polynomialDrawn)
            {
                Polynomial.GetInterpolationWithEndDerivate0(polynomial, points).Draw(limits, Color.blue);
                polynomialDrawn = true;
            }
            else
                Polynomial.GetInterpolationWithEndDerivate0(polynomial, points).UpdateDraw(limits, 3);
        }

        private void DrawXExpression()
        {
            if (!xExpressionDrawn)
            {
                xExpression.AssignExpression(xExpressionString).Draw(limits, Color.blue);
                xExpressionDrawn = true;
            }
            else
                xExpression.AssignExpression(xExpressionString).UpdateDraw(limits, 3);
        }

        #endregion

    }
}
