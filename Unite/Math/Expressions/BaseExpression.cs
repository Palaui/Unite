using UnityEngine;

namespace Unite
{
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
