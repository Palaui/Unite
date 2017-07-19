using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    public class Expression
    {
        // Variables
        #region Variables

        private enum Operation { None, Add, Sub, Mul, Div }

        private string enteringExpression;
        private string solveExpression;

        #endregion

        // Public
        #region Public

        public Expression(string expression)
        {
            enteringExpression = expression;
        }

        public float Solve(float x)
        {
            solveExpression = enteringExpression.Trim();
            while (solveExpression.Contains(" "))
                solveExpression = solveExpression.Replace(" ", "");

            GeometricPass(x);
            ArithmeticPass(x);

            float value = 0;
            float.TryParse(solveExpression, out value);
            return value;
        }

        #endregion

        // Private
        #region Private

        private void GeometricPass(float x)
        {
            Operation op = Operation.None;
            string str = "";
            float value = 0;
            float subValue = 0;
            int count = 0;
            for (int i = 0; i < solveExpression.Length; i++)
            {
                count++;

                if (count >= 30)
                    return;
                if ((solveExpression[i] == '+') || (solveExpression[i] == '-'))
                {
                    if (str != "")
                        solveExpression = solveExpression.Replace(str, value.ToString());
                    if (op != Operation.None)
                        i = 0;
                    op = Operation.None;
                    str = "";
                    continue;
                }

                str += solveExpression[i].ToString();
                if (solveExpression[i] == '*') { op = Operation.Mul; continue; }
                if (solveExpression[i] == '/') { op = Operation.Div; continue; }

                float.TryParse(solveExpression[i].ToString(), out subValue);
                if (solveExpression[i] == 'x') { subValue = x; }
                    
                switch (op)
                {
                    case Operation.None: value = subValue; break;
                    case Operation.Mul: value *= subValue; break;
                    case Operation.Div: value /= subValue; break;
                }
            }
            if (str != "")
                solveExpression = solveExpression.Replace(str, value.ToString());
        }

        private void ArithmeticPass(float x)
        {
            Operation op = Operation.None;
            string str = "";
            float value = 0;
            float subValue = 0;

            for (int i = 0; i < solveExpression.Length; i++)
            {
                str += solveExpression[i].ToString();
                if (solveExpression[i] == '+') { op = Operation.Add; continue; }
                if (solveExpression[i] == '-') { op = Operation.Sub; continue; }

                float.TryParse(solveExpression[i].ToString(), out subValue);
                if (solveExpression[i] == 'x') { subValue = x; }

                switch (op)
                {
                    case Operation.None: value = subValue; break;
                    case Operation.Add: value += subValue; break;
                    case Operation.Sub: value -= subValue; break;
                }
            }
            if (str != "")
                solveExpression = solveExpression.Replace(str, value.ToString());
        }

        #endregion

    }
}
