using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    public class Expression
    {
        // Variables
        #region Variables

        private Dictionary<string, float> parameters = new Dictionary<string, float>();
        private List<string> elements = new List<string>();
        private List<Vector2> points = new List<Vector2>();

        private string expression;
        private string solveExpression;

        private string element = "";
        private float step = 0;

        #endregion

        // Public
        #region Public

        public Expression(string expression)
        {
            this.expression = expression;
        }

        public void AssignExpression(string expression)
        {
            this.expression = expression;
        }

        public float Evaluate()
        {
            solveExpression = expression.Trim();

            while (solveExpression.Contains(" "))
                solveExpression = solveExpression.Replace(" ", "");

            ConvertElements();
            
            float result;
            float.TryParse(SolveElements(), out result);

            return result;
        }

        public float Solve(Vector2 bounds, float y, float tol = 0.0675f)
        {
            Vector2 limits = GetUpAndBelowValues(bounds, y);
            if (limits != Vector2.zero)
                return BisectionRecursion(limits, y, tol);
            return 0;
        }

        public List<float> SolveAllInRange(Vector2 bounds, float step, float y, float tol = 0.0675f)
        {
            List<float> list = new List<float>();
            Vector2 limits;
            float pass = bounds.x;

            while (pass < bounds.y)
            {
                if (CheckUpAndBelowValues(new Vector2(pass, pass + step), y, out limits))
                    list.Add(BisectionRecursion(limits, y, tol));
                pass += step;
            }

            return list;
        }

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

        // Calculus
        #region Calculus

        public List<Vector2> GetGraphicPoints(Vector2 domain, float inStep = 0.0675f)
        {
            points.Clear();
            step = inStep;
            step = Mathf.Clamp(step, 0.001f, 1000);

            for (float f = domain.x; f <= domain.y; f += step)
            {
                if (f > -0.001f && f < 0.001f)
                    f = 0;
                AddOrChangeParameter("x", f);
                points.Add(new Vector2(f, Evaluate()));
            }

            return points;
        }

        public List<Vector2> GetDerivatePoints(List<Vector2> graphicPoints)
        {
            List<Vector2> derivatePoints = new List<Vector2>();
            float numerator;
            float denominator;

            for (int i = 0; i < graphicPoints.Count - 1; i++)
            {
                numerator = graphicPoints[i + 1].y - graphicPoints[i].y;
                denominator = graphicPoints[i + 1].x - graphicPoints[i].x;
                derivatePoints.Add(new Vector2(graphicPoints[i + 1].x, numerator / denominator));
            }

            return derivatePoints;
        }

        public List<Vector2> GetPrimitivePoints(List<Vector2> graphicPoints)
        {
            List<Vector2> primitivePoints = new List<Vector2>();
            float meanHeight;
            float interval;
            float cumulative = 0;
            float midPoint;

            for (int i = 0; i < graphicPoints.Count - 1; i++)
            {
                meanHeight = (graphicPoints[i + 1].y + graphicPoints[i].y) / 2;
                interval = graphicPoints[i + 1].x - graphicPoints[i].x;
                cumulative += meanHeight * interval;
                primitivePoints.Add(new Vector2(graphicPoints[i + 1].x, cumulative));
            }

            midPoint = primitivePoints[Mathf.FloorToInt(primitivePoints.Count / 2.0f)].y;
            for (int i = 0; i < graphicPoints.Count - 1; i++)
                primitivePoints[i] = new Vector2(primitivePoints[i].x, primitivePoints[i].y - midPoint);

            return primitivePoints;
        }

        #endregion

        #endregion

        // Public Static
        #region Public Static

        public static string GetPolynomial(float[] coef)
        {
            string poly = "";
            for (int i = 0; i < coef.Length; i++)
            {
                if (i + 1 < coef.Length)
                    poly += coef[i] + " * x ^ " + ((coef.Length - i) - 1) + " + ";
                else
                    poly += coef[i] + " * x ^ " + ((coef.Length - i) - 1);
            }

            return poly;
        }

        public static string GetPolynomialDerivate(float[] coef)
        {
            string poly = "";
            for (int i = 0; i < coef.Length - 1; i++)
            {
                if (i + 2 < coef.Length)
                    poly += ((coef.Length - i) - 1) * coef[i] + " * x ^ " + ((coef.Length - i) - 2) + " + ";
                else
                    poly += ((coef.Length - i) - 1) * coef[i] + " * x ^ " + ((coef.Length - i) - 2);
            }

            return poly;
        }

        public static string GetPolynomialPrimitive(float[] coef)
        {
            string poly = "";
            for (int i = 0; i < coef.Length; i++)
                poly += coef[i] / (coef.Length - i) + " * x ^ " + (coef.Length - i) + " + ";

            return poly + "C";
        }

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
                CheckParameter();
                CheckOperation();
                index = CheckFloat(index);
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
                        elements[i + 1] = (float.Parse(elements[i + 1]) * -1).ToString();
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
                        elements[i + 1] = (float.Parse(elements[i + 1]) * -1).ToString();
                }
            }
            
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i] == "Operation_^")
                {
                    elements[i - 1] = Mathf.Pow(float.Parse(elements[i - 1]), float.Parse(elements[i + 1])).ToString();
                    elements.RemoveAt(i);
                    elements.RemoveAt(i);
                    i--;
                }
                else if (elements[i] == "Operation_Sqrt")
                {
                    elements[i] = Mathf.Sqrt(float.Parse(elements[i + 1])).ToString();
                    elements.RemoveAt(i + 1);
                }
            }

            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i] == "Operation_*")
                {
                    elements[i - 1] = (float.Parse(elements[i - 1]) * float.Parse(elements[i + 1])).ToString();
                    elements.RemoveAt(i);
                    elements.RemoveAt(i);
                    i--;
                }
                else if (elements[i] == "Operation_/")
                {
                    elements[i - 1] = (float.Parse(elements[i - 1]) / float.Parse(elements[i + 1])).ToString();
                    elements.RemoveAt(i);
                    elements.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i] == "Operation_+")
                {
                    elements[i - 1] = (float.Parse(elements[i - 1]) + float.Parse(elements[i + 1])).ToString();
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
                            Expression ex = new Expression(subExpression);
                            ex.parameters = parameters;
                            solveExpression = solveExpression.Replace(subExpression, ex.Evaluate().ToString());
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

        private void CheckParameter()
        {
            foreach (KeyValuePair<string, float> entry in parameters)
            {
                if (element == entry.Key)
                {
                    elements.Add(entry.Value.ToString());
                    element = "";
                    break;
                }
            }
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

        private int CheckFloat(int index)
        {
            string segment = "";
            float value = 0;
            float currentSubValue;
            if (float.TryParse(element, out currentSubValue))
            {
                while (float.TryParse(element, out currentSubValue))
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

        // Solver
        #region Solver

        private Vector2 GetUpAndBelowValues(Vector2 bounds, float y)
        {
            Vector2 limits = Vector2.zero;
            float evalue;
            int counter = 0;
            int maxSteps = 1000;

            do
            {
                if (counter >= maxSteps)
                {
                    Debug.LogError("Expression Solve, unable to find solution");
                    return Vector2.zero;
                }
                limits.y = Random.Range(bounds.x, bounds.y);
                AddOrChangeParameter("x", limits.y);
                evalue = Evaluate();
                counter++;
            }
            while (evalue < y);

            counter = 0;
            do
            {
                if (counter >= maxSteps)
                {
                    Debug.LogError("Expression Solve, unable to find solution");
                    return Vector2.zero;
                }
                limits.x = Random.Range(bounds.x, bounds.y);
                AddOrChangeParameter("x", limits.x);
                evalue = Evaluate();
                counter++;
            }
            while (evalue > y);

            return limits;
        }

        private bool CheckUpAndBelowValues(Vector2 initialBounds, float y, out Vector2 resultBounds)
        {
            AddOrChangeParameter("x", initialBounds.x);
            float xEvalue = Evaluate();
            AddOrChangeParameter("x", initialBounds.y);
            float yEvalue = Evaluate();

            if (xEvalue >= y && yEvalue <= y)
            {
                resultBounds.x = initialBounds.y;
                resultBounds.y = initialBounds.x;
                return true;
            }
            else if (xEvalue <= y && yEvalue >= y)
            {
                resultBounds = initialBounds;
                return true;
            }
            resultBounds = Vector2.zero;
            return false;
        }

        private float BisectionRecursion(Vector2 limits, float y, float tol)
        {
            float param;
            float evalue;
            int counter = 0;
            int maxSteps = 1000;

            do
            {
                param = (limits.x + limits.y) / 2;
                AddOrChangeParameter("x", param);
                evalue = Evaluate();
                if (evalue > y)
                    limits.y = param;
                else
                    limits.x = param;

                counter++;
                if (counter > maxSteps)
                    return param;
            }
            while (Mathf.Abs(evalue - y) > tol);

            return param;
        }

        #endregion

        #endregion

    }
}
