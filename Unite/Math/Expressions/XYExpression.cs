using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    class XYExpression : BaseXYExpression
    {
        // Variables
        #region Variables

        private List<string> elements = new List<string>();

        private string solveExpression;
        private string element = "";

        #endregion

        // Override
        #region Override

        public XYExpression(string expression)
        {
            this.expression = expression;
        }

        #endregion

        // Public
        #region Public

        // Base
        #region Base

        public void AssignExpression(string expression)
        {
            this.expression = expression;
        }

        #endregion

        // Calculus
        #region Calculus

        public override double Evaluate(double x, double y)
        {
            solveExpression = expression.Trim();

            while (solveExpression.Contains(" "))
                solveExpression = solveExpression.Replace(" ", "");
            while (solveExpression.Contains("x"))
                solveExpression = solveExpression.Replace("x", x.ToString());
            while (solveExpression.Contains("y"))
                solveExpression = solveExpression.Replace("y", y.ToString());

            ConvertElements(x, y);

            double result;
            double.TryParse(SolveElements(), out result);

            return result;
        }

        #endregion

        #endregion

        // Private
        #region Private

        // Evaluator
        #region Evaluator

        private void ConvertElements(double x, double y)
        {
            InitializePass();
            SubExpressionsPass(x, y);

            for (int index = 0; index < solveExpression.Length; index++)
            {
                element += solveExpression[index].ToString();
                CheckOperation();
                index = CheckDouble(index);
            }
        }

        private string SolveElements()
        {
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i] == "Operation_-")
                {
                    if (elements[i - 1].Contains("Operation_"))
                    {
                        elements[i + 1] = (double.Parse(elements[i + 1]) * -1).ToString();
                        elements.RemoveAt(i);
                        i--;
                        continue;
                    }
                    elements[i] = "Operation_+";

                    if (elements[i + 1] == "Operation_-")
                        elements[i + 1] = "Operation_+";
                    else if (elements[i + 1] == "Operation_+")
                        elements[i + 1] = "Operation_-";
                    else
                        elements[i + 1] = (double.Parse(elements[i + 1]) * -1).ToString();
                }
            }

            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i] == "Operation_^")
                {
                    elements[i - 1] = Math.Pow(double.Parse(elements[i - 1]), double.Parse(elements[i + 1])).ToString();
                    elements.RemoveAt(i);
                    elements.RemoveAt(i);
                    i--;
                }
                else if (elements[i] == "Operation_Sqrt")
                {
                    elements[i] = Math.Sqrt(double.Parse(elements[i + 1])).ToString();
                    elements.RemoveAt(i + 1);
                }
            }

            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i] == "Operation_*")
                {
                    elements[i - 1] = (double.Parse(elements[i - 1]) * double.Parse(elements[i + 1])).ToString();
                    elements.RemoveAt(i);
                    elements.RemoveAt(i);
                    i--;
                }
                else if (elements[i] == "Operation_/")
                {
                    elements[i - 1] = (double.Parse(elements[i - 1]) / double.Parse(elements[i + 1])).ToString();
                    elements.RemoveAt(i);
                    elements.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i] == "Operation_+")
                {
                    elements[i - 1] = (double.Parse(elements[i - 1]) + double.Parse(elements[i + 1])).ToString();
                    elements.RemoveAt(i);
                    elements.RemoveAt(i);
                    i--;
                }
            }

            return elements[elements.Count - 1];
        }

        private void SubExpressionsPass(double x, double y)
        {
            string subExpression;
            int level;
            int beginIndex;
            bool subExpressionFound = true;

            while (subExpressionFound)
            {
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
                            XYExpression ex = new XYExpression(subExpression);
                            solveExpression = solveExpression.Replace(subExpression, ex.Evaluate(x, y).ToString());
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

        #endregion

        // Checkers
        #region Checkers

        private void InitializePass()
        {
            elements.Clear();
            elements.Add("0");

            element = "";
        }

        private void CheckOperation()
        {
            string[] operations = new string[] { "+", "-", "*", "/", "^", "Sqrt" };

            foreach (string str in operations)
            {
                if (str == element)
                {
                    elements.Add("Operation_" + str);
                    element = "";
                    break;
                }
            }
        }

        private int CheckDouble(int index)
        {
            string segment = "";
            double value = 0;
            double currentSubValue;
            if (double.TryParse(element, out currentSubValue))
            {
                while (double.TryParse(element, out currentSubValue))
                {
                    value = currentSubValue;
                    if (solveExpression.Length > index + 1)
                    {
                        index++;
                        element += solveExpression[index];
                        segment = solveExpression[index].ToString();
                    }
                    else
                    {
                        elements.Add(value.ToString());
                        element = "";
                        return index;
                    }
                }
                elements.Add(value.ToString());
                element = "";
                return index - 1;
            }
            return index;
        }

        #endregion

        #endregion

    }
}
