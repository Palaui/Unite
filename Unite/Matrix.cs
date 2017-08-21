
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

        public Vector3[,] CalculateInterpolatedGraphicPoints(Vector3[,] positions, int numberOfSteps, ExpressionOrder order)
        {
            Vector3[,] interpolatedPositions = new Vector3[numberOfSteps + 1, numberOfSteps + 1];
            Vector3[,] unheighedPositions = new Vector3[positions.GetLength(0), positions.GetLength(1)];
            List<Polynomial> xPolynomials = new List<Polynomial>();
            List<Polynomial> zPolynomials = new List<Polynomial>();
            double tol = 0.001f;

            // Get positions without height for distance calculations
            for (int i = 0; i < positions.GetLength(0); i++)
                for (int j = 0; j < positions.GetLength(1); j++)
                    unheighedPositions[i, j] = new Vector3(positions[i, j].x, 0, positions[i, j].z);

            Vector2 totalDistance = new Vector2(
                Vector3.Distance(unheighedPositions[unheighedPositions.GetLength(0) - 1, 0], unheighedPositions[0, 0]),
                Vector3.Distance(unheighedPositions[0, unheighedPositions.GetLength(1) - 1], unheighedPositions[0, 0]));

            Vector2 clusterDistance = new Vector2(totalDistance.x / (positions.GetLength(0) - 1), totalDistance.y / (positions.GetLength(1) - 1));

            // Calculate basic expressions and assign position lists
            List<Vector2> points = new List<Vector2>();
            for (int j = 0; j < positions.GetLength(1); j++)
            {
                for (int i = 0; i < positions.GetLength(0); i++)
                    points.Add(new Vector2(positions[i, j].x, positions[i, j].y));

                switch (order)
                {
                    case ExpressionOrder.Derivative:
                        xPolynomials.Add(Polynomial.GetInterpolationWithEndDerivate0(points).GetDerivate());
                        break;
                    case ExpressionOrder.Primitive:
                        xPolynomials.Add(Polynomial.GetInterpolationWithEndDerivate0(points).GetPrimitive());
                        break;
                    default:
                        xPolynomials.Add(Polynomial.GetInterpolationWithEndDerivate0(points));
                        break;
                }
                points = new List<Vector2>();
            }

            for (int i = 0; i < positions.GetLength(0); i++)
            {
                for (int j = 0; j < positions.GetLength(1); j++)
                    points.Add(new Vector2(positions[i, j].z, positions[i, j].y));

                switch (order)
                {
                    case ExpressionOrder.Derivative:
                        zPolynomials.Add(Polynomial.GetInterpolationWithEndDerivate0(points).GetDerivate());
                        break;
                    case ExpressionOrder.Primitive:
                        zPolynomials.Add(Polynomial.GetInterpolationWithEndDerivate0(points).GetPrimitive());
                        break;
                    default:
                        zPolynomials.Add(Polynomial.GetInterpolationWithEndDerivate0(points));
                        break;
                }
                points = new List<Vector2>();
            }

            // Fill interpolated points
            Vector2 minusDist, regularDist, minusValue, regularValue, posY;
            float posX, posZ;
            int kx = 1, kz = 1;
            for (int i = 0; i <= numberOfSteps; i++)
            {
                // Pre x pass
                posX = positions[0, 0].x + ((float)i / numberOfSteps) * totalDistance.x;
                for (int k = 1; k < positions.GetLength(0); k++)
                    if (positions[k, 0].x > posX - tol) { kx = k; break; }
                minusDist.x = (posX - positions[kx - 1, 0].x) / clusterDistance.x;
                regularDist.x = (positions[kx, 0].x - posX) / clusterDistance.x;

                for (int j = 0; j <= numberOfSteps; j++)
                {
                    posZ = positions[0, 0].z + ((float)j / numberOfSteps) * totalDistance.y;
                    for (int k = 1; k < positions.GetLength(1); k++)
                        if (positions[0, k].z > posZ - tol) { kz = k; break; }

                    // X pass
                    minusValue.x = xPolynomials[kz - 1].Evaluate(posX);
                    regularValue.x = xPolynomials[kz].Evaluate(posX);

                    // Z pass
                    minusDist.y = (posZ - positions[0, kz - 1].z) / clusterDistance.y;
                    regularDist.y = (positions[0, kz].z - posZ) / clusterDistance.y;
                    minusValue.y = zPolynomials[kx - 1].Evaluate(posZ);
                    regularValue.y = zPolynomials[kx].Evaluate(posZ);
                    
                    // Find Y
                    posY.x = minusDist.y * regularValue.x + regularDist.y * minusValue.x;
                    posY.y = minusDist.x * regularValue.y + regularDist.x * minusValue.y;
                    interpolatedPositions[i, j] = new Vector3(posX, (posY.x + posY.y) / 2, posZ);
                }
            }

            return interpolatedPositions;
        }

        #endregion

        #endregion

    }
}
