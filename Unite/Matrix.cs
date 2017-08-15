
using System.Collections.Generic;
using UnityEngine;

namespace Unite
{
    public class Matrix
    {
        // Variables
        #region Variables

        private int rows;
        private int columns;

        private float[,] values;
        private float[,] lu;

        #endregion

        // Properties
        #region Properties

        public int Rows { get { return rows; } }
        public int Columns { get { return columns; } }
        public float[,] Values { get { return values; } }

        public float this[int x, int y]
        {
            get
            {
                if (rows > y && columns > x)
                    return values[x, y];
                else
                    return 0;
            }
            set
            {
                if (rows > y && columns > x)
                    values[x, y] = value;
            }
        }

        #endregion

        // Override
        #region Override

        public Matrix(int inRows, int inColumns)
        {
            rows = inRows;
            columns = inColumns;

            values = new float[rows, columns];

            for (int i = 0; i < columns; i++)
                for (int j = 0; j < rows; j++)
                    values[i, j] = 0;
        }

        public Matrix(float[,] matrix)
        {
            rows = matrix.GetLength(0);
            columns = matrix.GetLength(1);
            values = matrix;
        }

        #endregion

        // Public
        #region Public

        public float[] Solve(float[] results)
        {
            if (results.Length != rows)
            {
                Debug.LogError("Matrix Solve: passed results length is different from the matrix order, Aborting");
                return null;
            }

            if (lu == null)
                LU();

            if (!IsSquare())
                return null;

            // Result for Ly = results
            float[] y = new float[rows];
            float sum;
            for (int i = 0; i < rows; i++)
            {
                sum = 0;
                for (int k = 0; k < i; k++)
                    sum += lu[i, k] * y[k];
                y[i] = results[i] - sum;
            }

            // Results for Ux = y
            float[] x = new float[rows];
            for (int i = rows - 1; i >= 0; i--)
            {
                sum = 0;
                for (int k = i + 1; k < rows; k++)
                    sum += lu[i, k] * x[k];
                x[i] = (1 / lu[i, i]) * (y[i] - sum);
            }

            return x;
        }

        // Fillers
        #region FIllers

        public void FillRow(float[] inRow, int index)
        {
            if (index >= rows)
            {
                Debug.LogError("Matrix Fill Row: Unable to fill row, index passed is bigger than the number of rows, Aborting");
                return;
            }

            if (inRow.Length != columns)
            {
                Debug.LogError("Matrix Fill Row: passed row length is different than the number of columns, Aborting");
                return;
            }

            for (int j = 0; j < columns; j++)
                values[index, j] = inRow[j];
        }

        public void FillColumn(float[] inColumn, int index)
        {
            if (index >= columns)
            {
                Debug.LogError("Matrix Fill Column: Unable to fill column, index passed is bigger than the number of columns, Aborting");
                return;
            }

            if (inColumn.Length != rows)
            {
                Debug.LogError("Matrix Fill Column: passed column length is different than the number of rows, Aborting");
                return;
            }

            for (int i = 0; i < columns; i++)
                values[i, index] = inColumn[i];
        }

        #endregion

        // Operations
        #region Operations

        public bool IsSquare()
        {
            return (rows == columns);
        }

        public float[,] LU()
        {
            if (!IsSquare())
            {
                Debug.LogError("Matrix LU: Unable to decompose matrix in LU, the matrix is not square");
                return null;
            }

            lu = new float[rows, columns];
            float sum = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = i; j < rows; j++)
                {
                    sum = 0;
                    for (int k = 0; k < i; k++)
                        sum += lu[i, k] * lu[k, j];
                    lu[i, j] = values[i, j] - sum;
                }
                for (int j = i + 1; j < rows; j++)
                {
                    sum = 0;
                    for (int k = 0; k < i; k++)
                        sum += lu[j, k] * lu[k, i];
                    lu[j, i] = (1 / lu[i, i]) * (values[j, i] - sum);
                }
            }

            return lu;
        }

        public float Determinant()
        {
            if (!IsSquare())
            {
                Debug.LogError("Matrix Determinant: Unable to process determinant of a non square matrix, Aborting");
                return 0;
            }

            if (lu == null)
                LU();

            float det = 1;
            for (int i = 0; i < columns; i++)
                det *= lu[i, i];

            return det;
        }

        public void Traspose()
        {
            if (!IsSquare())
            {
                Debug.LogError("Matrix Traspose: Unable to traspose a non square matrix, Aborting");
                return;
            }

            float aux;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < i - 1; j++)
                {
                    aux = values[i, j];
                    values[i, j] = values[j, i];
                    values[j, i] = aux;
                }
            }
        }

        #endregion

        // Calculus
        #region Calculus

        public void CalculateInterpolatedGraphicPoints(Vector3[,] positions, int numberOfSteps)
        {
            Vector3[,] interpolatedPositions = new Vector3[numberOfSteps, numberOfSteps];
            Vector3[,] unheighedPositions = new Vector3[positions.GetLength(0), positions.GetLength(1)];

            List<Expression> xExpressions = new List<Expression>();
            List<Expression> zExpressions = new List<Expression>();
            List<Vector3> points = new List<Vector3>();

            // Get positions without height for distance calculations
            for (int i = 0; i < positions.GetLength(0); i++)
                for (int j = 0; j < positions.GetLength(1); j++)
                    unheighedPositions[i, j] = new Vector3(positions[i, j].x, 0, positions[i, j].z);

            Vector2 totalDistance = new Vector2(
                Vector3.Distance(unheighedPositions[unheighedPositions.GetLength(0) - 1, 0], unheighedPositions[0, 0]),
                Vector3.Distance(unheighedPositions[0, unheighedPositions.GetLength(1) - 1], unheighedPositions[0, 0]));

            Vector2 clusterDistance = totalDistance / numberOfSteps;

            // Calculate basic expressions and assign position lists
            for (int i = 0; i < positions.GetLength(1); i++)
            {
                for (int j = 0; j < positions.GetLength(0); j++)
                    points.Add(positions[i, j]);

                xExpressions.Add(GetInterpolationPolynomialWithDerivate0(points, points[0], points[points.Count - 1]));
                points.Clear();
            }

            for (int j = 0; j < positions.GetLength(1); j++)
            {
                for (int i = 0; i < positions.GetLength(0); i++)
                    points.Add(positions[i, j]);

                zExpressions.Add(GetInterpolationPolynomialWithDerivate0(points, points[0], points[points.Count - 1]));
                points.Clear();
            }

            // Fill interpolated points
            for (int i = 0; i < interpolatedPositions.GetLength(0); i++)
            {
                float posX = Mathf.Lerp(0, interpolatedPositions.GetLength(0), i) * totalDistance.x;
                float posY = 0;
                for (int k = 1; k < positions.GetLength(0); k++)
                {
                    if (positions[k, 0].x > posX)
                    {
                        float minusDist = (posX - positions[k - 1, 0].x) / clusterDistance.x;
                        float regularDist = (positions[k, 0].x - posX) / clusterDistance.x;
                        float minus1 = xExpressions[k - 1].Evaluate();
                        float regular = xExpressions[k].Evaluate();
                        posY = minusDist * minus1 + regularDist * regular;
                        break;
                    }
                }

                for (int j = 0; j < interpolatedPositions.GetLength(1); j++)
                {
                    float posZ = Mathf.Lerp(0, interpolatedPositions.GetLength(1), j) * totalDistance.y;
                    for (int k = 1; k < positions.GetLength(1); k++)
                    {
                        if (positions[k, 0].x > posX)
                        {
                            float minusDist = (posX - positions[0, k - 1].y) / clusterDistance.y;
                            float regularDist = (positions[0, k].y - posZ) / clusterDistance.y;
                            float minus1 = zExpressions[k - 1].Evaluate();
                            float regular = zExpressions[k].Evaluate();
                            posY = (posY + minusDist * minus1 + regularDist * regular) / 2;
                            break;
                        }
                    }
                    interpolatedPositions[i, j] = new Vector3(posX, posY, posZ);
                }
            }
        }

        #endregion

        // Draw
        #region Draw

        public GameObject Draw(Vector3[,] positions)
        {
            return null;
        }

        #endregion

        #endregion

        // Public Static
        #region Public Static

        public static Expression GetInterpolationPolynomial(List<Vector3> points)
        {
            List<float> list = new List<float>();
            Matrix matrix = new Matrix(points.Count, points.Count);

            for (int i = 0; i < points.Count; i++)
            {
                for (int j = 0; j < points.Count; j++)
                    list.Add(Mathf.Pow(points[i].x, (points.Count - j) - 1));
                matrix.FillRow(Ext.CreateArrayFromList(list), i);
                list.Clear();
            }

            for (int i = 0; i < points.Count; i++)
                list.Add(points[i].y);

            float[] sol = matrix.Solve(Ext.CreateArrayFromList(list));

            return new Expression(sol);
        }

        public static Expression GetInterpolationPolynomialWithDerivate0(List<Vector3> points, Vector3 derivateA, Vector3 derivateB)
        {
            List<float> list = new List<float>();
            Matrix matrix = new Matrix(points.Count + 2, points.Count + 2);

            for (int i = 0; i < points.Count; i++)
            {
                for (int j = 0; j < points.Count + 2; j++)
                    list.Add(Mathf.Pow(points[i].x, points.Count + 1 - j));
                matrix.FillRow(Ext.CreateArrayFromList(list), i);
                list.Clear();
            }

            for (int i = 0; i < points.Count + 1; i++)
                list.Add(Mathf.Pow(derivateA.x, points.Count - i) * ((points.Count - i) + 1));
            list.Add(0);
            matrix.FillRow(Ext.CreateArrayFromList(list), points.Count);
            list.Clear();

            for (int i = 0; i < points.Count + 1; i++)
                list.Add(Mathf.Pow(derivateB.x, points.Count - i) * ((points.Count - i) + 1));
            list.Add(0);
            matrix.FillRow(Ext.CreateArrayFromList(list), points.Count + 1);
            list.Clear();

            for (int i = 0; i < points.Count; i++)
                list.Add(points[i].y);
            list.Add(0);
            list.Add(0);

            float[] sol = matrix.Solve(Ext.CreateArrayFromList(list));

            return new Expression(sol);
        }



        #endregion

    }
}
