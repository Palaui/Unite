using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    public class XExpression : BaseXExpression
    {
        // Variables
        #region Variables

        private Dictionary<string, float> parameters = new Dictionary<string, float>();
        private List<string> elements = new List<string>();

        private string solveExpression;
        private string element = "";
        private double eValue;

        #endregion

        // Override
        #region Override

        public XExpression(string expression)
        {
            this.expression = expression;
        }

        #endregion

        // Public
        #region Public

        // Base
        #region Base

        public XExpression AssignExpression(string expression)
        {
            this.expression = expression;
            return this;
        }

        #endregion

        // Calculus
        #region Calculus

        public override double Evaluate(double x)
        {
            solveExpression = expression.Trim();

            while (solveExpression.Contains(" "))
                solveExpression = solveExpression.Replace(" ", "");
            foreach (KeyValuePair<string, float> entry in parameters)
            {
                while (solveExpression.Contains(entry.Key))
                    solveExpression = solveExpression.Replace(entry.Key, entry.Value.ToString());
            }
            while (solveExpression.Contains("x"))
                solveExpression = solveExpression.Replace("x", x.ToString());

            eValue = x;
            ConvertElements();

            double result;
            double.TryParse(SolveElements(), out result);

            return result;
        }

        public List<DoubleV2> GetDerivatePoints(List<DoubleV2> graphicPoints)
        {
            List<DoubleV2> derivatePoints = new List<DoubleV2>();
            double numerator;
            double denominator;

            for (int i = 0; i < graphicPoints.Count - 1; i++)
            {
                numerator = graphicPoints[i + 1].y - graphicPoints[i].y;
                denominator = graphicPoints[i + 1].x - graphicPoints[i].x;
                derivatePoints.Add(new DoubleV2(graphicPoints[i + 1].x, numerator / denominator));
            }

            return derivatePoints;
        }

        public List<DoubleV2> GetPrimitivePoints(List<DoubleV2> graphicPoints)
        {
            List<DoubleV2> primitivePoints = new List<DoubleV2>();
            double meanHeight;
            double interval;
            double cumulative = 0;
            double midPoint;

            for (int i = 0; i < graphicPoints.Count - 1; i++)
            {
                meanHeight = (graphicPoints[i + 1].y + graphicPoints[i].y) / 2;
                interval = graphicPoints[i + 1].x - graphicPoints[i].x;
                cumulative += meanHeight * interval;
                primitivePoints.Add(new DoubleV2(graphicPoints[i + 1].x, cumulative));
            }

            midPoint = primitivePoints[(int)Math.Floor(primitivePoints.Count / 2.0f)].y;
            for (int i = 0; i < graphicPoints.Count - 1; i++)
                primitivePoints[i] = new DoubleV2(primitivePoints[i].x, primitivePoints[i].y - midPoint);

            return primitivePoints;
        }

        #endregion

        // Dictionaries Manipulation
        #region Dictionaries Manipulation

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

        #endregion

        // Private
        #region Private

        // Evaluator
        #region Evaluator

        private void ConvertElements()
        {
            InitializePass();
            SubExpressionsPass();

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

        private void SubExpressionsPass()
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
                            XExpression ex = new XExpression(subExpression);
                            solveExpression = solveExpression.Replace(subExpression, ex.Evaluate(eValue).ToString());
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
