﻿using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    public class Expression
    {
        // Variables
        #region Variables

        private enum Operation { None, Add, Sub, Mul, Div, Pot, Sqrt }

        private Dictionary<string, float> parameters = new Dictionary<string, float>();
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

        public float Solve()
        {
            solveExpression = expression.Trim();

            while (solveExpression.Contains(" "))
                solveExpression = solveExpression.Replace(" ", "");

            SubExpressionsPass();
            PotentialPass();
            GeometricPass();
            ArithmeticPass();

            float result;
            float.TryParse(solveExpression, out result);
            return result;
        }

        public void AddOrChangeParameter(string key, float value)
        {
            RemoveParameter(key);
            parameters.Add(key, value);
        }

        public void RemoveParameter(string key)
        {
            if (parameters.ContainsKey(key))
                parameters.Remove(key);
        }

        public void RemovaAllParameters()
        {
            parameters.Clear();
        }

        #endregion

        // Private
        #region Private

        // Passes
        #region Passes

        private void SubExpressionsPass()
        {
            string subExpression;
            int level;
            int beginIndex;
            bool subExpressionFound = true;

            int cut = 0;

            while (subExpressionFound)
            {
                cut++;
                if (cut > 30)
                    break;
                subExpression = "";
                level = 0;
                beginIndex = -1;
                subExpressionFound = false;

                for (int i = 0; i < solveExpression.Length; i++)
                {
                    if (solveExpression[i] == ')')
                    {
                        level--;
                        if (level == 0)
                        {
                            solveExpression = solveExpression.Remove(i, 1);
                            solveExpression = solveExpression.Remove(beginIndex, 1);
                            Expression ex = new Expression(subExpression);
                            ex.parameters = parameters;
                            solveExpression = solveExpression.Replace(subExpression, ex.Solve().ToString());
                            break;
                        }
                    }
                    if (level > 0)
                        subExpression += solveExpression[i].ToString();
                    if (solveExpression[i] == '(')
                    {
                        level++;
                        if (beginIndex == -1)
                            beginIndex = i;
                        subExpressionFound = true;
                    }
                }

                if (level > 0)
                {
                    Debug.LogError("Expression incorrectly parsed");
                    break;
                }
            }
        }

        private void PotentialPass()
        {
            InitializePass();

            for (int index = 0; index < solveExpression.Length; index++)
            {
                index = CheckLowerLevelElement(new string[] { "+", "-", "*", "/" }, index);
                UpdateStrings(index);
                if (IsSign(new string[] { "^", "Sqrt" }, new Operation[] { Operation.Pot, Operation.Sqrt }))
                    continue;

                CheckParameter();
                index = CheckNumber(index);
            }
            if (segment != "" && operation != Operation.None)
                solveExpression = solveExpression.Replace(segment, value.ToString());
        }

        private void GeometricPass()
        {
            InitializePass();

            for (int index = 0; index < solveExpression.Length; index++)
            {
                index = CheckLowerLevelElement(new string[] { "+", "-" }, index);
                UpdateStrings(index);
                if (IsSign(new string[] { "*", "/" }, new Operation[] { Operation.Mul, Operation.Div }))
                    continue;

                CheckParameter();
                index = CheckNumber(index);
            }
            if (segment != "" && operation != Operation.None)
                solveExpression = solveExpression.Replace(segment, value.ToString());
        }

        private void ArithmeticPass()
        {
            InitializePass();

            for (int index = 0; index < solveExpression.Length; index++)
            {
                UpdateStrings(index);
                if (IsSign(new string[] { "+", "-" } , new Operation[] { Operation.Add, Operation.Sub }))
                    continue;

                CheckParameter();
                index = CheckNumber(index);
            }
            if (segment != "")
                solveExpression = solveExpression.Replace(segment, value.ToString());
        }

        #endregion

        // Region
        #region Checkers

        private void InitializePass()
        {
            operation = Operation.None;
            segment = "";
            element = "";
            value = 0;
            subValue = 0;
        }

        private void UpdateStrings(int index)
        {
            segment += solveExpression[index].ToString();
            element += solveExpression[index].ToString();
        }

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

        private int CheckNumber(int index)
        {
            string currentSubSegment = "";
            float currentSubValue;
            if (float.TryParse(element, out currentSubValue))
            {
                while (float.TryParse(element, out currentSubValue))
                {
                    segment += currentSubSegment;
                    subValue = currentSubValue;
                    if (solveExpression.Length > index + 1)
                    {
                        index++;
                        element += solveExpression[index];
                        currentSubSegment = solveExpression[index].ToString();
                    }
                    else
                    {
                        AssignSubValue();
                        return index;
                    }
                }
                AssignSubValue();
                return index - 1;
            }
            return index;
        }

        private void CheckParameter()
        {
            foreach (KeyValuePair<string, float> entry in parameters)
            {
                if (element == entry.Key)
                {
                    subValue = entry.Value;
                    AssignSubValue();
                    break;
                }
            }
        }

        private void AssignSubValue()
        {
            switch (operation)
            {
                case Operation.None: value = subValue; break;
                case Operation.Add: value += subValue; break;
                case Operation.Sub: value -= subValue; break;
                case Operation.Mul: value *= subValue; break;
                case Operation.Div: value /= subValue; break;
                case Operation.Pot: value = Mathf.Pow(value, subValue); break;
                case Operation.Sqrt: value = Mathf.Sqrt(subValue); break;
            }
            subValue = 0;
            element = "";
        }

        private int CheckLowerLevelElement(string[] lowerLevelElements, int index)
        {
            foreach(string str in lowerLevelElements)
            {
                if (str == element)
                {
                    if (operation != Operation.None)
                    {
                        if (segment != "")
                            solveExpression = solveExpression.Replace(segment, value.ToString() + str);
                        index = 0;
                        operation = Operation.None;
                    }
                    element = "";
                    segment = "";
                    break;
                }
            }

            return index;
        }

        #endregion

        #endregion

    }
}
