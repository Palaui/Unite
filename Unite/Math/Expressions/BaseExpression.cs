using UnityEngine;

namespace Unite
{
    // Enums
    #region Enums

    public enum ExpressionOrder { Basic, Derivative, Primitive }

    #endregion

    public abstract class BaseExpression
    {
        // Variables
        #region Variables

        protected GameObject drawReference;
        protected string expression;

        // Draw
        protected double currentStep;
        protected bool hasBeenDrawn = false;

        #endregion

        // Override
        #region Override

        public override string ToString()
        {
            return expression;
        }

        #endregion
    }
}
