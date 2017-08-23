using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    public class XExpression : BaseXExpression
    {
        // Variables
        #region Variables

        //private Dictionary<string, float> parameters = new Dictionary<string, float>();
        private List<string> elements = new List<string>();

        private string solveExpression;
        private string element = "";
        private float eValue;

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

        // Basic
        #region Basic

        public void AssignExpression(string expression)
        {
            this.expression = expression;
        }

        //public float Evaluate()
        //{
        //    solveExpression = expression.Trim();

        //    while (solveExpression.Contains(" "))
        //        solveExpression = solveExpression.Replace(" ", "");

        //    ConvertElements();
            
        //    float result;
        //    float.TryParse(SolveElements(), out result);

        //    return result;
        //}

        public override float Evaluate(float x)
        {
            solveExpression = expression.Trim();

            while (solveExpression.Contains(" "))
                solveExpression = solveExpression.Replace(" ", "");
            while (solveExpression.Contains("x"))
                solveExpression = solveExpression.Replace("x", x.ToString());

            this.eValue = x;
            ConvertElements();

            float result;
            float.TryParse(SolveElements(), out result);

            return result;
        }

        #endregion

        // Dictionaries Manipulation
        #region Dictionaries Manipulation

        //public void AddOrChangeParameter(string key, float value)
        //{
        //    RemoveParameter(key);
        //    parameters.Add(key, value);
        //}

        //public void RemoveParameter(string key)
        //{
        //    if (parameters.ContainsKey(key))
        //        parameters.Remove(key);
        //}

        //public void RemovaAllParameters()
        //{
        //    parameters.Clear();
        //}

        #endregion

        // Calculus
        #region Calculus

        //public Vector3[,] GetGraphicPoints(Vector2 xDomain, Vector2 yDomain, float inStepX = 0.0675f, float inStepY = 0.0675f)
        //{
        //    List<List<Vector3>> points = new List<List<Vector3>>();
        //    List<Vector3> subPoints = new List<Vector3>();
        //    inStepX = Mathf.Clamp(inStepX, 0.001f, 1000);
        //    inStepY = Mathf.Clamp(inStepY, 0.001f, 1000);

        //    for (float x = xDomain.x; x <= xDomain.y; x += inStepX)
        //    {
        //        if (x > -0.001f && x < 0.001f) { x = 0; }
        //        AddOrChangeParameter("x", x);
        //        subPoints = new List<Vector3>();

        //        for (float y = yDomain.x; y <= yDomain.y; y += inStepY)
        //        {
        //            if (y > -0.001f && y < 0.001f) { y = 0; }
        //            AddOrChangeParameter("y", y);
        //            subPoints.Add(new Vector3(x, Evaluate(), y));
        //        }
        //        points.Add(subPoints);
        //    }

        //    return Ext.CreateArrayFromList(points);
        //}

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

        // Draw
        #region Draw

        //public GameObject Draw(Vector2 limits, float inStep = 0.0625f, float z = 0, bool eraseReference = true)
        //{
        //    if (eraseReference)
        //        Object.DestroyImmediate(drawReference);

        //    XExpression ex = new XExpression(ToString());
        //    List<Vector2> graphicsPoints = ex.GetGraphicPoints(new Vector2(limits.x, limits.y), 0.0625f);
        //    drawReference = ProcMesh.BuildLine(Ext.CreateArrayFromList(Ext.ConvertList2DTo3D(graphicsPoints, 0)), Color.blue, 0.005f);
        //    Ext.ResetTransform(drawReference);
        //    drawReference.transform.position = new Vector3(0, 0, z);
        //    return drawReference;
        //}

        //public GameObject Draw(Vector2 xDomain, Vector2 yDomain, float inStepX, float inStepY)
        //{
        //    return ProcMesh.BuildPlane(GetGraphicPoints(xDomain, yDomain, inStepX, inStepY));
        //}

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
                //CheckParameter();
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

        //private void SubExpressionsPass()
        //{
        //    string subExpression;
        //    int level;
        //    int beginIndex;
        //    bool subExpressionFound = true;

        //    while (subExpressionFound)
        //    {
        //        subExpression = "";
        //        level = 0;
        //        beginIndex = -1;
        //        subExpressionFound = false;

        //        for (int i = 0; i < solveExpression.Length; i++)
        //        {
        //            if (solveExpression[i] == ')')
        //            {
        //                level--;
        //                if (level == 0)
        //                {
        //                    solveExpression = solveExpression.Remove(i, 1);
        //                    solveExpression = solveExpression.Remove(beginIndex, 1);
        //                    XExpression ex = new XExpression(subExpression);
        //                    ex.parameters = parameters;
        //                    solveExpression = solveExpression.Replace(subExpression, ex.Evaluate().ToString());
        //                    break;
        //                }
        //            }
        //            if (level > 0)
        //                subExpression += solveExpression[i].ToString();
        //            if (solveExpression[i] == '(')
        //            {
        //                level++;
        //                if (beginIndex == -1)
        //                    beginIndex = i;
        //                subExpressionFound = true;
        //            }
        //        }

        //        if (level > 0)
        //        {
        //            Debug.LogError("Expression incorrectly parsed");
        //            break;
        //        }
        //    }
        //}

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

        //private void CheckParameter()
        //{
        //    foreach (KeyValuePair<string, float> entry in parameters)
        //    {
        //        if (element == entry.Key)
        //        {
        //            elements.Add(entry.Value.ToString());
        //            element = "";
        //            break;
        //        }
        //    }
        //}

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

        #endregion

    }
}
