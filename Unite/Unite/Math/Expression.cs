using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    public class Expression
    {
        // Variables
        #region Variables

        private enum Operation { None, Add, Sub, Mul, Div, Pot, Sqrt }

        private string expression;
        private string solveExpression;

        private Operation operation;
        private string segment = "";
        private string element = "";
        private float value = 0;
        private float subValue = 0;

        #endregion

        // Public
        #region Public

        public Expression(string expression)
        {
            this.expression = expression;
        }

        public float Solve(float x)
        {
            solveExpression = expression.Trim();
            while (solveExpression.Contains(" "))
                solveExpression = solveExpression.Replace(" ", "");

            PotentialPass(x);
            GeometricPass(x);
            ArithmeticPass(x);

            float value = 0;
            float.TryParse(solveExpression, out value);
            return value;
        }

        #endregion

        // Private
        #region Private

        private void PotentialPass(float x)
        {
            operation = Operation.None;
            segment = "";
            element = "";
            value = 0;
            subValue = 0;
            for (int i = 0; i < solveExpression.Length; i++)
            {
                if ((solveExpression[i] == '+') || (solveExpression[i] == '-') || (solveExpression[i] == '/') || (solveExpression[i] == '*'))
                {
                    if (segment != "")
                        solveExpression = solveExpression.Replace(segment, value.ToString());
                    if (operation != Operation.None)
                        i = 0;
                    operation = Operation.None;
                    element = "";
                    segment = "";
                    continue;
                }

                segment += solveExpression[i].ToString();
                element += solveExpression[i].ToString();

                if (element == "^") { operation = Operation.Pot; element = ""; continue; }
                if (element == "sqrt") { operation = Operation.Sqrt; element = ""; continue; }

                if (float.TryParse(element, out subValue)) { element = ""; }
                if (solveExpression[i] == 'x') { subValue = x; element = ""; }

                switch (operation)
                {
                    case Operation.None: value = subValue; break;
                    case Operation.Pot: value = Mathf.Pow(value, subValue); break;
                    case Operation.Sqrt: value = Mathf.Sqrt(subValue); break;
                }
            }
            if (segment != "")
                solveExpression = solveExpression.Replace(segment, value.ToString());
        }

        private void GeometricPass(float x)
        {
            operation = Operation.None;
            segment = "";
            element = "";
            value = 0;
            subValue = 0;
            for (int i = 0; i < solveExpression.Length; i++)
            {
                if ((solveExpression[i] == '+') || (solveExpression[i] == '-'))
                {
                    if (segment != "")
                        solveExpression = solveExpression.Replace(segment, value.ToString());
                    if (operation != Operation.None)
                        i = 0;
                    operation = Operation.None;
                    element = "";
                    segment = "";
                    continue;
                }

                segment += solveExpression[i].ToString();
                element += solveExpression[i].ToString();

                if (solveExpression[i] == '*') { operation = Operation.Mul; element = ""; continue; }
                if (solveExpression[i] == '/') { operation = Operation.Div; element = ""; continue; }

                float.TryParse(element, out subValue);
                if (solveExpression[i] == 'x') { subValue = x; element = ""; }
                    
                switch (operation)
                {
                    case Operation.None: value = subValue; break;
                    case Operation.Mul: value *= subValue; break;
                    case Operation.Div: value /= subValue; break;
                }
            }
            if (segment != "")
                solveExpression = solveExpression.Replace(segment, value.ToString());
        }

        private void ArithmeticPass(float x)
        {
            operation = Operation.None;
            segment = "";
            element = "";
            value = 0;
            subValue = 0;

            for (int i = 0; i < solveExpression.Length; i++)
            {
                bool ok = false;
                segment += solveExpression[i].ToString();
                element += solveExpression[i].ToString();

                if (IsSign(new string[] { "+", "-" } , new Operation[] { Operation.Add, Operation.Sub }))
                    continue;


                if (float.TryParse(element, out subValue)) { element = ""; ok = true; }
                if (solveExpression[i] == 'x') { subValue = x; element = ""; ok = true; }

                if (ok)
                switch (operation)
                {
                    case Operation.None: value = subValue; break;
                    case Operation.Add: value += subValue; break;
                    case Operation.Sub: value -= subValue; break;
                }
            }
            if (segment != "")
                solveExpression = solveExpression.Replace(segment, value.ToString());
        }

        #endregion

        private bool IsSign(string[] signs, Operation[] ops)
        {
            for (int i = 0; i < signs.Length; i++)
            {
                if (element == signs[i])
                {
                    operation = ops[i];
                    element = "";
                    return true;
                }
            }

            return false;
        }

        private void CheckLowerLevelElement(string comparer, string[] lowerLevelElements)
        {

        }

    }
}
