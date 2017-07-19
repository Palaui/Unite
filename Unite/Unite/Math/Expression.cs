using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    public class Expression
    {
        // Variables
        #region Variables

        private enum Operation { None, Add, Sub, Mul, Div, Pot, Sqrt }

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
            Operation op = Operation.None;
            string str = "";
            string numberStr = "";
            float value = 0;
            float subValue = 0;
            int count = 0;
            for (int i = 0; i < solveExpression.Length; i++)
            {
                count++;

                if (count >= 30)
                    return;
                if ((solveExpression[i] == '+') || (solveExpression[i] == '-') || (solveExpression[i] == '/') || (solveExpression[i] == '*'))
                {
                    if (str != "")
                        solveExpression = solveExpression.Replace(str, value.ToString());
                    if (op != Operation.None)
                        i = 0;
                    op = Operation.None;
                    numberStr = "";
                    str = "";
                    continue;
                }

                str += solveExpression[i].ToString();
                numberStr += solveExpression[i].ToString();

                if (solveExpression[i] == '^') { op = Operation.Pot; numberStr = ""; continue; }
                if (solveExpression[i] == 's') { op = Operation.Sqrt; numberStr = ""; continue; }

                float.TryParse(numberStr, out subValue);
                if (solveExpression[i] == 'x') { subValue = x; numberStr = ""; }

                switch (op)
                {
                    case Operation.None: value = subValue; break;
                    case Operation.Pot: value = Mathf.Pow(value, subValue); break;
                    case Operation.Sqrt: value = Mathf.Sqrt(subValue); break;
                }
            }
            if (str != "")
                solveExpression = solveExpression.Replace(str, value.ToString());
        }

        private void GeometricPass(float x)
        {
            Operation op = Operation.None;
            string str = "";
            string numberStr = "";
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
                    numberStr = "";
                    str = "";
                    continue;
                }

                str += solveExpression[i].ToString();
                numberStr += solveExpression[i].ToString();

                if (solveExpression[i] == '*') { op = Operation.Mul; numberStr = ""; continue; }
                if (solveExpression[i] == '/') { op = Operation.Div; numberStr = ""; continue; }

                float.TryParse(numberStr, out subValue);
                if (solveExpression[i] == 'x') { subValue = x; numberStr = ""; }
                    
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
            string numberStr = "";
            float value = 0;
            float subValue = 0;

            for (int i = 0; i < solveExpression.Length; i++)
            {
                str += solveExpression[i].ToString();
                numberStr += solveExpression[i].ToString();

                if (solveExpression[i] == '+') { op = Operation.Add; numberStr = ""; continue; }
                if (solveExpression[i] == '-') { op = Operation.Sub; numberStr = ""; continue; }

                float.TryParse(numberStr, out subValue);
                if (solveExpression[i] == 'x') { subValue = x; numberStr = ""; }

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
